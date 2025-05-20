using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    /// <inheritdoc />
    public partial class update_ctome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TTLHRelateIdsJson",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "Customers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "TTLHRelateIdsJson",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
