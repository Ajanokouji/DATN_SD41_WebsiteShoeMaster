using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_db13_5_2_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "BillDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SKU",
                table: "BillDetails");
        }
    }
}
