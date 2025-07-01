using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PriceComparer.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSortParametersToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortId",
                table: "Requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrderId",
                table: "Requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sort",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sort", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SortOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortOrder", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "м'ясо", -1 },
                columns: new[] { "SortId", "SortOrderId" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "овочі", -1 },
                columns: new[] { "SortId", "SortOrderId" },
                values: new object[] { 0, 0 });

            migrationBuilder.UpdateData(
                table: "Requests",
                keyColumns: new[] { "Name", "UserId" },
                keyValues: new object[] { "хліб", -1 },
                columns: new[] { "SortId", "SortOrderId" },
                values: new object[] { 0, 0 });

            migrationBuilder.InsertData(
                table: "Sort",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Name" },
                    { 1, "UnifiedPrice" }
                });

            migrationBuilder.InsertData(
                table: "SortOrder",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Asc" },
                    { 1, "Desc" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SortId",
                table: "Requests",
                column: "SortId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SortOrderId",
                table: "Requests",
                column: "SortOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_SortOrder_SortOrderId",
                table: "Requests",
                column: "SortOrderId",
                principalTable: "SortOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Sort_SortId",
                table: "Requests",
                column: "SortId",
                principalTable: "Sort",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_SortOrder_SortOrderId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Sort_SortId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "Sort");

            migrationBuilder.DropTable(
                name: "SortOrder");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SortId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SortOrderId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SortId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SortOrderId",
                table: "Requests");
        }
    }
}
