using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace program.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class SortByPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sort",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Price" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sort",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
