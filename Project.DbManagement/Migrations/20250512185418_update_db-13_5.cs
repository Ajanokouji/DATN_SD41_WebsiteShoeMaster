using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    /// <inheritdoc />
    public partial class update_db13_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MetadataJson",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Vouchers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "VoucherProducts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "VoucherDetails",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Provinces",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Products",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "ProductCategoriesRelations",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "PaymentMethods",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "ImageFiles",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Customers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "ContentBase",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Contacts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Categories",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Carts",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "CartDetails",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "Bills",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "Isdeleted",
                table: "BillDetails",
                newName: "IsDeleted");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Vouchers",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "VoucherProducts",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "VoucherDetails",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Users",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Provinces",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Products",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ProductCategoriesRelations",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "PaymentMethods",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ImageFiles",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Customers",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ContentBase",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Contacts",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Categories",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Carts",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "CartDetails",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Bills",
                newName: "Isdeleted");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "BillDetails",
                newName: "Isdeleted");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Users",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "MetadataJson",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
