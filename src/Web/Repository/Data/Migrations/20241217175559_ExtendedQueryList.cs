using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendedQueryList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овочі", -1 });

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

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
                    { "цукор", -1, 0, 0 },
                    { "чай", -1, 0, 0 },
                    { "шампунь", -1, 0, 0 },
                    { "шоколад", -1, 0, 0 },
                    { "яйця", -1, 0, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "вода", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "зубна паста", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "йогурт", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава зерно", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "кава розчинна", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "ковбаса", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "консерви", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "корм", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "крупа", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "макаронні вироби", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "мило", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "молоко", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "морозиво", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "напій", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овоч", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "олія", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "оцет", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "печиво", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "підгузки", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "пральний порошок", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "риба", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сардельки", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сир", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сік", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сіль", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "сосиски", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "соус", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "спеції", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "торт", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "туалетний папір", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "фрукт", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "цукор", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "чай", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шампунь", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "шоколад", -1 });

            migrationBuilder.DeleteData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "яйця", -1 });

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.InsertData(
                table: "Requests",
                columns: new[] { "Name", "UserId", "SortId", "SortOrderId" },
                values: new object[] { "овочі", -1, 0, 0 });
        }
    }
}
