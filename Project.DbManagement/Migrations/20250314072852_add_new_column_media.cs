using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    public partial class add_new_column_media : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MediasJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediasJson",
                table: "Products");
        }
    }
}
