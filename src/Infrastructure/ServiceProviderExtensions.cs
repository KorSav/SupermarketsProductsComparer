using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static Infrastructure.RemoveItAndUseSeparateFile;

namespace Infrastructure;

public static class ServiceProviderExtensions
{
    public static async Task ThrowIfDbIsNotAccessibleAsync(this IServiceProvider rootProvider)
    {
        // Simple check to catch invalid startup, enforces db to be accessible in a valid state.
        // There are no any validation for db failure after app started, might be implemented in future
        await using var scope = rootProvider.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        if (!await ctx.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "Make sure that DB is created and running. Can't start application since DB is not reachable"
            );
        }
        var notAppliedMigrations = await ctx.Database.GetPendingMigrationsAsync();
        if (notAppliedMigrations.Any())
        {
            throw new InvalidOperationException(
                "There are pending migrations, DB has old state. Please apply migrations using 'dotnet ef database update'"
            );
        }
        await ctx.Database.ExecuteSqlRawAsync(SQL);
    }
}

file static class RemoveItAndUseSeparateFile
{
    public static string SQL = """
        CREATE EXTENSION IF NOT EXISTS pgcrypto;

        CREATE OR REPLACE PROCEDURE public.merge_bulk_products_from_stage()
        LANGUAGE plpgsql
        AS $$
        DECLARE
            utc_now timestamp without time zone := timezone('UTC', now());
        BEGIN
            -------------------------------------------------------------------------
            -- 0. Prepare deduplicated working stage.
            -------------------------------------------------------------------------
            DROP TABLE IF EXISTS _stage_products;
            CREATE TEMP TABLE _stage_products ON COMMIT DROP AS
            SELECT DISTINCT ON (normalized_name, shop, amount, unit)
                shop,
                name,
                name_suffix,
                normalized_name,
                price,
                unified_price,
                amount,
                unit,
                full_link_product,
                full_link_image
            FROM bulk_product_stage
            ORDER BY
                normalized_name,
                shop,
                amount,
                unit,
                price DESC;

            -------------------------------------------------------------------------
            -- 1. Existing product:
            --    same shop, amount, unit and normalized name.
            --    Update product data, including raw Name, and update/insert price history.
            -------------------------------------------------------------------------
            DROP TABLE IF EXISTS _exact_products;
            CREATE TEMP TABLE _exact_products ON COMMIT DROP AS
            SELECT
                s.*,
                existing.product_id
            FROM _stage_products s
            JOIN LATERAL (
                SELECT
                    p."Id" AS product_id
                FROM "Products" p
                JOIN "ProductGroups" g
                    ON g."Id" = p."ProductGroupId"
                WHERE p."Shop" = s.shop
                AND p."Amount" = s.amount
                AND p."Unit" = s.unit
                AND g."NormalizedName" = s.normalized_name
                ORDER BY p."Id"
                LIMIT 1
            ) existing ON true;

            UPDATE "Products" p
            SET
                "Name" = e.name,
                "NameSuffix" = e.name_suffix,
                "FullLinkProduct" = e.full_link_product,
                "FullLinkImage" = e.full_link_image
            FROM _exact_products e
            WHERE p."Id" = e.product_id
            AND (
                    p."Name" IS DISTINCT FROM e.name
                OR p."NameSuffix" IS DISTINCT FROM e.name_suffix
                OR p."FullLinkProduct" IS DISTINCT FROM e.full_link_product
                OR p."FullLinkImage" IS DISTINCT FROM e.full_link_image
            );

            CREATE TEMP TABLE _exact_history_state ON COMMIT DROP AS
            SELECT
                e.product_id,
                e.price,
                e.unified_price,
                latest."Id" AS latest_history_id,
                latest."Price" AS latest_price,
                COALESCE(history_counter.history_count, 0) AS history_count
            FROM _exact_products e
            LEFT JOIN LATERAL (
                SELECT
                    h."Id",
                    h."Price"
                FROM "PriceHistories" h
                WHERE h."ProductId" = e.product_id
                ORDER BY h."ParsedAt" DESC, h."Id" DESC
                LIMIT 1
            ) latest ON true
            LEFT JOIN LATERAL (
                SELECT COUNT(*)::int AS history_count
                FROM (
                    SELECT 1
                    FROM "PriceHistories" h
                    WHERE h."ProductId" = e.product_id
                    LIMIT 2
                ) x
            ) history_counter ON true;

            UPDATE "PriceHistories" h
            SET "ParsedAt" = utc_now
            FROM _exact_history_state ehs
            WHERE h."Id" = ehs.latest_history_id
            AND ehs.latest_price = ehs.price
            AND ehs.history_count >= 2;

            INSERT INTO "PriceHistories" ("ProductId", "Price", "UnifiedPrice", "ParsedAt")
            SELECT
                ehs.product_id,
                ehs.price,
                ehs.unified_price,
                utc_now
            FROM _exact_history_state ehs
            WHERE ehs.latest_history_id IS NULL
            OR ehs.latest_price IS DISTINCT FROM ehs.price
            OR ehs.history_count < 2;

            DELETE FROM _stage_products s
            USING _exact_products e
            WHERE s.normalized_name = e.normalized_name
            AND s.shop = e.shop
            AND s.amount = e.amount
            AND s.unit = e.unit;


            -------------------------------------------------------------------------
            -- 2. Remaining products with matching existing groups:
            --    create new product entries and price history entries,
            --    referencing the existing product group by exact NormalizedName.
            -------------------------------------------------------------------------
            DROP TABLE IF EXISTS _matching_group_products;
            CREATE TEMP TABLE _matching_group_products ON COMMIT DROP AS
            SELECT
                s.*,
                group_lookup.product_group_id,
                gen_random_uuid() AS new_product_id
            FROM _stage_products s
            JOIN LATERAL (
                SELECT
                    g."Id" AS product_group_id
                FROM "ProductGroups" g
                WHERE g."NormalizedName" = s.normalized_name
                ORDER BY g."Id"
                LIMIT 1
            ) group_lookup ON true;

            DROP TABLE IF EXISTS _matching_group_inserted_products;
            CREATE TEMP TABLE _matching_group_inserted_products ON COMMIT DROP AS
            WITH inserted AS (
                INSERT INTO "Products" (
                    "Id",
                    "ProductGroupId",
                    "Shop",
                    "Name",
                    "NameSuffix",
                    "Amount",
                    "Unit",
                    "FullLinkProduct",
                    "FullLinkImage"
                )
                SELECT
                    new_product_id,
                    product_group_id,
                    shop,
                    name,
                    name_suffix,
                    amount,
                    unit,
                    full_link_product,
                    full_link_image
                FROM _matching_group_products
                ON CONFLICT ("Name", "Shop", "Amount", "Unit") DO NOTHING
                RETURNING "Id"
            )
            SELECT "Id" AS product_id
            FROM inserted;

            INSERT INTO "PriceHistories" ("ProductId", "Price", "UnifiedPrice", "ParsedAt")
            SELECT
                mgp.new_product_id,
                mgp.price,
                mgp.unified_price,
                utc_now
            FROM _matching_group_products mgp
            JOIN _matching_group_inserted_products inserted
                ON inserted.product_id = mgp.new_product_id;

            DELETE FROM _stage_products s
            USING _matching_group_products mgp
            WHERE s.name = mgp.name
            AND s.shop = mgp.shop
            AND s.amount = mgp.amount
            AND s.unit = mgp.unit;


            -------------------------------------------------------------------------
            -- 3. Completely new product names.
            --    Resolve product group by exact NormalizedName match.
            -------------------------------------------------------------------------
            DROP TABLE IF EXISTS _missing_normalized_groups;
            CREATE TEMP TABLE _missing_normalized_groups ON COMMIT DROP AS
            SELECT DISTINCT
                s.normalized_name
            FROM _stage_products s
            WHERE NOT EXISTS (
                SELECT 1
                FROM "ProductGroups" g
                WHERE g."NormalizedName" = s.normalized_name
            )
            ORDER BY s.normalized_name;

            INSERT INTO "ProductGroups" ("NormalizedName")
            SELECT normalized_name
            FROM _missing_normalized_groups;

            DROP TABLE IF EXISTS _normalized_group_lookup;
            CREATE TEMP TABLE _normalized_group_lookup ON COMMIT DROP AS
            SELECT DISTINCT ON (g."NormalizedName")
                g."NormalizedName" AS normalized_name,
                g."Id" AS product_group_id
            FROM "ProductGroups" g
            JOIN (
                SELECT DISTINCT normalized_name
                FROM _stage_products
            ) s
                ON s.normalized_name = g."NormalizedName"
            ORDER BY
                g."NormalizedName",
                g."Id";

            DROP TABLE IF EXISTS _new_products;
            CREATE TEMP TABLE _new_products ON COMMIT DROP AS
            SELECT
                s.*,
                gl.product_group_id,
                gen_random_uuid() AS new_product_id
            FROM _stage_products s
            JOIN _normalized_group_lookup gl
                ON gl.normalized_name = s.normalized_name;

            DROP TABLE IF EXISTS _new_inserted_products;
            CREATE TEMP TABLE _new_inserted_products ON COMMIT DROP AS
            WITH inserted AS (
                INSERT INTO "Products" (
                    "Id",
                    "ProductGroupId",
                    "Shop",
                    "Name",
                    "NameSuffix",
                    "Amount",
                    "Unit",
                    "FullLinkProduct",
                    "FullLinkImage"
                )
                SELECT
                    new_product_id,
                    product_group_id,
                    shop,
                    name,
                    name_suffix,
                    amount,
                    unit,
                    full_link_product,
                    full_link_image
                FROM _new_products
                ON CONFLICT ("Name", "Shop", "Amount", "Unit") DO NOTHING
                RETURNING "Id"
            )
            SELECT "Id" AS product_id
            FROM inserted;

            INSERT INTO "PriceHistories" ("ProductId", "Price", "UnifiedPrice", "ParsedAt")
            SELECT
                np.new_product_id,
                np.price,
                np.unified_price,
                utc_now
            FROM _new_products np
            JOIN _new_inserted_products inserted
                ON inserted.product_id = np.new_product_id;

        END;
        $$;
        """;
}
