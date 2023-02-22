using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minishopapi.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucheramountColumninOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VoucherAmount",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherAmount",
                table: "Orders");
        }
    }
}
