using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalInfoAboutProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "PriceUnified");

            migrationBuilder.AlterColumn<string>(
                name: "FullLinkProduct",
                table: "Products",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FullLinkImage",
                table: "Products",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceInitial",
                table: "Products",
                type: "numeric(14,2)",
                precision: 14,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Measures",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "M");

            migrationBuilder.InsertData(
                table: "Measures",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "No" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash" },
                values: new object[] { -1, "admin@gmail.com", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Name", "UserId" },
                values: new object[,]
                {
                    { "м'ясо", -1 },
                    { "овочі", -1 },
                    { "хліб", -1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Measures",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "м'ясо", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овочі", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "хліб", -1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "PriceInitial",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PriceUnified",
                table: "Products",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "FullLinkProduct",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "FullLinkImage",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2048)",
                oldMaxLength: 2048);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.UpdateData(
                table: "Measures",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "No");
        }
    }
}
