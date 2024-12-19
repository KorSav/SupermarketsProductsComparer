using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace program.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePKOfProductToFullProductLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "FullLinkProduct");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                columns: new[] { "Name", "ShopId" });
        }
    }
}
