using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoronaCheckIn.Migrations
{
    public partial class temporarily_remove_qrcode_from_room : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Rooms");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
