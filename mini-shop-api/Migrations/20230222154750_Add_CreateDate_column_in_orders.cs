using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minishopapi.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateDatecolumninorders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SoldProducts",
                table: "SoldProducts");

            migrationBuilder.RenameTable(
                name: "SoldProducts",
                newName: "Orders");

            migrationBuilder.RenameColumn(
                name: "voucherAmount",
                table: "CartItems",
                newName: "VoucherAmount");

            migrationBuilder.AlterColumn<double>(
                name: "VoucherAmount",
                table: "CartItems",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "VoucherId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Orders");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "SoldProducts");

            migrationBuilder.RenameColumn(
                name: "VoucherAmount",
                table: "CartItems",
                newName: "voucherAmount");

            migrationBuilder.AlterColumn<double>(
                name: "voucherAmount",
                table: "CartItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VoucherId",
                table: "SoldProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SoldProducts",
                table: "SoldProducts",
                column: "Id");
        }
    }
}
