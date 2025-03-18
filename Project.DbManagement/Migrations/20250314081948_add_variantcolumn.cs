using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    public partial class add_variantcolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VariantJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariantJson",
                table: "Products");
        }
    }
}
