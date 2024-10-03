using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasterPiece.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnToUserOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "otp",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "otp",
                table: "Users");
        }
    }
}
