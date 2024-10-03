using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterPiece.Migrations
{
    /// <inheritdoc />
    public partial class NewCloumnToPaymentPaymentLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentLink",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentLink",
                table: "Payments");
        }
    }
}
