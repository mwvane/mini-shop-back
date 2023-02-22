using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace minishopapi.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherAmounttoCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(name:"VoucherAmount", table:"CartItems", type: "float", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "VoucherAmount", table: "CartItems");

        }
    }
}
