using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterPiece.Migrations
{
    /// <inheritdoc />
    public partial class addnewColumnToPaymentISNoitification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsNotificationSent",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsNotificationSent",
                table: "Payments");
        }
    }
}
