using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.DbManagement.Migrations
{
    /// <inheritdoc />
    public partial class add_column_address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBases",
                table: "ContentBases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ContentBases");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ContentBases");

            migrationBuilder.RenameTable(
                name: "ContentBases",
                newName: "ContentBase");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinimumOrderAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplaySettings",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxUsagePerCustomer",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedeemCount",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalMaxUsage",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                table: "Provinces",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PathWithType",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParentCode",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameWithType",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "ContentBase",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnDate",
                table: "ContentBase",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBase",
                table: "ContentBase",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "VoucherProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VarientProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedOnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedOnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Isdeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherProducts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBase",
                table: "ContentBase");

            migrationBuilder.DropColumn(
                name: "DisplaySettings",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxDiscountAmount",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxUsagePerCustomer",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "RedeemCount",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "TotalMaxUsage",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "AddressType",
                table: "Provinces");

            migrationBuilder.RenameTable(
                name: "ContentBase",
                newName: "ContentBases");

            migrationBuilder.AlterColumn<int>(
                name: "MinimumOrderAmount",
                table: "Vouchers",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscountAmount",
                table: "Vouchers",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PathWithType",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentCode",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameWithType",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Districts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedOnDate",
                table: "ContentBases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOnDate",
                table: "ContentBases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ContentBases",
                type: "nvarchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "ContentBases",
                type: "nvarchar(128)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBases",
                table: "ContentBases",
                column: "Id");
        }
    }
}
