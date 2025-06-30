using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace program.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnumAsLookupTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Measures_MeasureId",
                table: "Products"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products"
            );

            migrationBuilder.DropForeignKey(name: "FK_Products_Shops_ShopId", table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_SortOrder_SortOrderId",
                table: "Requests"
            );

            migrationBuilder.DropForeignKey(name: "FK_Requests_Sort_SortId", table: "Requests");

            migrationBuilder.DropTable(name: "Measures");

            migrationBuilder.DropTable(name: "ProductStatuses");

            migrationBuilder.DropTable(name: "Shops");

            migrationBuilder.DropTable(name: "Sort");

            migrationBuilder.DropTable(name: "SortOrder");

            migrationBuilder.DropPrimaryKey(name: "PK_Requests", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Requests_SortId", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Requests_SortOrderId", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Products_MeasureId", table: "Products");

            migrationBuilder.DropIndex(name: "IX_Products_ProductStatusId", table: "Products");

            migrationBuilder.DropIndex(name: "IX_Products_ShopId", table: "Products");

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "вода", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "зубна паста", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "йогурт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава зерно", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава розчинна", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "ковбаса", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "консерви", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "корм", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "крупа", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "м'ясо", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "макаронні вироби", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "мило", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "молоко", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "морозиво", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "напій", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овоч", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "олія", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "оцет", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "печиво", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "підгузки", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "пральний порошок", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "риба", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сардельки", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сир", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сік", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сіль", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сосиски", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "соус", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "спеції", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "торт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "туалетний папір", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "фрукт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "хліб", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "цукор", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "чай", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шампунь", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шоколад", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "яйця", -1 }
            );

            migrationBuilder.DropColumn(name: "SortId", table: "Requests");

            migrationBuilder.DropColumn(name: "SortOrderId", table: "Requests");

            migrationBuilder.DropColumn(name: "MeasureId", table: "Products");

            migrationBuilder.DropColumn(name: "ProductStatusId", table: "Products");

            migrationBuilder.DropColumn(name: "ShopId", table: "Products");

            migrationBuilder.AddColumn<byte>(
                name: "Sort",
                table: "Requests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddColumn<byte>(
                name: "SortOrder",
                table: "Requests",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddColumn<byte>(
                name: "Measure",
                table: "Products",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddColumn<byte>(
                name: "ProductStatus",
                table: "Products",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddColumn<byte>(
                name: "Shop",
                table: "Products",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                columns: new[] { "Name", "UserId" }
            );

            migrationBuilder.CreateTable(
                name: "MeasureLookup",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(2)",
                        unicode: false,
                        maxLength: 2,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductStatusLookup",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(11)",
                        unicode: false,
                        maxLength: 11,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatusLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ShopLookup",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(5)",
                        unicode: false,
                        maxLength: 5,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SortByLookup",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(12)",
                        unicode: false,
                        maxLength: 12,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortByLookup", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SortOrderLookup",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "smallint", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(4)",
                        unicode: false,
                        maxLength: 4,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortOrderLookup", x => x.Id);
                }
            );

            migrationBuilder.InsertData(
                table: "MeasureLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Kg" },
                    { (byte)1, "L" },
                    { (byte)2, "M" },
                    { (byte)3, "No" },
                }
            );

            migrationBuilder.InsertData(
                table: "ProductStatusLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Updated" },
                    { (byte)1, "NeedRemoval" },
                }
            );

            migrationBuilder.InsertData(
                table: "ShopLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Silpo" },
                    { (byte)1, "Fozzy" },
                    { (byte)2, "Fora" },
                }
            );

            migrationBuilder.InsertData(
                table: "SortByLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Name" },
                    { (byte)1, "UnifiedPrice" },
                    { (byte)2, "Price" },
                }
            );

            migrationBuilder.InsertData(
                table: "SortOrderLookup",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { (byte)0, "Asc" },
                    { (byte)1, "Desc" },
                }
            );

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Name", "UserId", "Sort", "SortOrder" },
                values: new object[,]
                {
                    { "вода", -1, (byte)0, (byte)0 },
                    { "зубна паста", -1, (byte)0, (byte)0 },
                    { "йогурт", -1, (byte)0, (byte)0 },
                    { "кава зерно", -1, (byte)0, (byte)0 },
                    { "кава розчинна", -1, (byte)0, (byte)0 },
                    { "ковбаса", -1, (byte)0, (byte)0 },
                    { "консерви", -1, (byte)0, (byte)0 },
                    { "корм", -1, (byte)0, (byte)0 },
                    { "крупа", -1, (byte)0, (byte)0 },
                    { "м'ясо", -1, (byte)0, (byte)0 },
                    { "макаронні вироби", -1, (byte)0, (byte)0 },
                    { "мило", -1, (byte)0, (byte)0 },
                    { "молоко", -1, (byte)0, (byte)0 },
                    { "морозиво", -1, (byte)0, (byte)0 },
                    { "напій", -1, (byte)0, (byte)0 },
                    { "овоч", -1, (byte)0, (byte)0 },
                    { "олія", -1, (byte)0, (byte)0 },
                    { "оцет", -1, (byte)0, (byte)0 },
                    { "печиво", -1, (byte)0, (byte)0 },
                    { "підгузки", -1, (byte)0, (byte)0 },
                    { "пральний порошок", -1, (byte)0, (byte)0 },
                    { "риба", -1, (byte)0, (byte)0 },
                    { "сардельки", -1, (byte)0, (byte)0 },
                    { "сир", -1, (byte)0, (byte)0 },
                    { "сік", -1, (byte)0, (byte)0 },
                    { "сіль", -1, (byte)0, (byte)0 },
                    { "сосиски", -1, (byte)0, (byte)0 },
                    { "соус", -1, (byte)0, (byte)0 },
                    { "спеції", -1, (byte)0, (byte)0 },
                    { "торт", -1, (byte)0, (byte)0 },
                    { "туалетний папір", -1, (byte)0, (byte)0 },
                    { "фрукт", -1, (byte)0, (byte)0 },
                    { "хліб", -1, (byte)0, (byte)0 },
                    { "цукор", -1, (byte)0, (byte)0 },
                    { "чай", -1, (byte)0, (byte)0 },
                    { "шампунь", -1, (byte)0, (byte)0 },
                    { "шоколад", -1, (byte)0, (byte)0 },
                    { "яйця", -1, (byte)0, (byte)0 },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Sort",
                table: "Requests",
                column: "Sort"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SortOrder",
                table: "Requests",
                column: "SortOrder"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_Measure",
                table: "Products",
                column: "Measure"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductStatus",
                table: "Products",
                column: "ProductStatus"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_Shop",
                table: "Products",
                column: "Shop"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_MeasureLookup_Measure",
                table: "Products",
                column: "Measure",
                principalTable: "MeasureLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductStatusLookup_ProductStatus",
                table: "Products",
                column: "ProductStatus",
                principalTable: "ProductStatusLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ShopLookup_Shop",
                table: "Products",
                column: "Shop",
                principalTable: "ShopLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_SortByLookup_Sort",
                table: "Requests",
                column: "Sort",
                principalTable: "SortByLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_SortOrderLookup_SortOrder",
                table: "Requests",
                column: "SortOrder",
                principalTable: "SortOrderLookup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_MeasureLookup_Measure",
                table: "Products"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductStatusLookup_ProductStatus",
                table: "Products"
            );

            migrationBuilder.DropForeignKey(name: "FK_Products_ShopLookup_Shop", table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_SortByLookup_Sort",
                table: "Requests"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_SortOrderLookup_SortOrder",
                table: "Requests"
            );

            migrationBuilder.DropTable(name: "MeasureLookup");

            migrationBuilder.DropTable(name: "ProductStatusLookup");

            migrationBuilder.DropTable(name: "ShopLookup");

            migrationBuilder.DropTable(name: "SortByLookup");

            migrationBuilder.DropTable(name: "SortOrderLookup");

            migrationBuilder.DropPrimaryKey(name: "PK_Requests", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Requests_Sort", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Requests_SortOrder", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Requests_UserId", table: "Requests");

            migrationBuilder.DropIndex(name: "IX_Products_Measure", table: "Products");

            migrationBuilder.DropIndex(name: "IX_Products_ProductStatus", table: "Products");

            migrationBuilder.DropIndex(name: "IX_Products_Shop", table: "Products");

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "вода", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "зубна паста", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "йогурт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава зерно", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава розчинна", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "ковбаса", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "консерви", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "корм", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "крупа", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "м'ясо", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "макаронні вироби", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "мило", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "молоко", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "морозиво", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "напій", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овоч", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "олія", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "оцет", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "печиво", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "підгузки", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "пральний порошок", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "риба", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сардельки", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сир", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сік", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сіль", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сосиски", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "соус", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "спеції", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "торт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "туалетний папір", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "фрукт", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "хліб", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "цукор", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "чай", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шампунь", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шоколад", -1 }
            );

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "яйця", -1 }
            );

            migrationBuilder.DropColumn(name: "Sort", table: "Requests");

            migrationBuilder.DropColumn(name: "SortOrder", table: "Requests");

            migrationBuilder.DropColumn(name: "Measure", table: "Products");

            migrationBuilder.DropColumn(name: "ProductStatus", table: "Products");

            migrationBuilder.DropColumn(name: "Shop", table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "Requests",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "SortOrderId",
                table: "Requests",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "MeasureId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "ProductStatusId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddPrimaryKey(
                name: "PK_Requests",
                table: "Requests",
                columns: new[] { "UserId", "Name" }
            );

            migrationBuilder.CreateTable(
                name: "Measures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Measures", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStatuses", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(20)",
                        maxLength: 20,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Sort",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sort", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "SortOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(
                        type: "character varying(5)",
                        maxLength: 5,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortOrder", x => x.Id);
                }
            );

            migrationBuilder.InsertData(
                table: "Measures",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Kg" },
                    { 1, "L" },
                    { 2, "M" },
                    { 3, "No" },
                }
            );

            migrationBuilder.InsertData(
                table: "ProductStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Updated" },
                    { 1, "NeedRemoval" },
                }
            );

            migrationBuilder.InsertData(
                table: "Shops",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Silpo" },
                    { 1, "Fozzy" },
                    { 2, "Fora" },
                }
            );

            migrationBuilder.InsertData(
                table: "Sort",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Name" },
                    { 1, "UnifiedPrice" },
                    { 2, "Price" },
                }
            );

            migrationBuilder.InsertData(
                table: "SortOrder",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Asc" },
                    { 1, "Desc" },
                }
            );

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Name", "UserId", "SortId", "SortOrderId" },
                values: new object[,]
                {
                    { "вода", -1, 0, 0 },
                    { "зубна паста", -1, 0, 0 },
                    { "йогурт", -1, 0, 0 },
                    { "кава зерно", -1, 0, 0 },
                    { "кава розчинна", -1, 0, 0 },
                    { "ковбаса", -1, 0, 0 },
                    { "консерви", -1, 0, 0 },
                    { "корм", -1, 0, 0 },
                    { "крупа", -1, 0, 0 },
                    { "м'ясо", -1, 0, 0 },
                    { "макаронні вироби", -1, 0, 0 },
                    { "мило", -1, 0, 0 },
                    { "молоко", -1, 0, 0 },
                    { "морозиво", -1, 0, 0 },
                    { "напій", -1, 0, 0 },
                    { "овоч", -1, 0, 0 },
                    { "олія", -1, 0, 0 },
                    { "оцет", -1, 0, 0 },
                    { "печиво", -1, 0, 0 },
                    { "підгузки", -1, 0, 0 },
                    { "пральний порошок", -1, 0, 0 },
                    { "риба", -1, 0, 0 },
                    { "сардельки", -1, 0, 0 },
                    { "сир", -1, 0, 0 },
                    { "сік", -1, 0, 0 },
                    { "сіль", -1, 0, 0 },
                    { "сосиски", -1, 0, 0 },
                    { "соус", -1, 0, 0 },
                    { "спеції", -1, 0, 0 },
                    { "торт", -1, 0, 0 },
                    { "туалетний папір", -1, 0, 0 },
                    { "фрукт", -1, 0, 0 },
                    { "хліб", -1, 0, 0 },
                    { "цукор", -1, 0, 0 },
                    { "чай", -1, 0, 0 },
                    { "шампунь", -1, 0, 0 },
                    { "шоколад", -1, 0, 0 },
                    { "яйця", -1, 0, 0 },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SortId",
                table: "Requests",
                column: "SortId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SortOrderId",
                table: "Requests",
                column: "SortOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_MeasureId",
                table: "Products",
                column: "MeasureId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductStatusId",
                table: "Products",
                column: "ProductStatusId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopId",
                table: "Products",
                column: "ShopId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Measures_MeasureId",
                table: "Products",
                column: "MeasureId",
                principalTable: "Measures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductStatuses_ProductStatusId",
                table: "Products",
                column: "ProductStatusId",
                principalTable: "ProductStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Shops_ShopId",
                table: "Products",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_SortOrder_SortOrderId",
                table: "Requests",
                column: "SortOrderId",
                principalTable: "SortOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Sort_SortId",
                table: "Requests",
                column: "SortId",
                principalTable: "Sort",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
