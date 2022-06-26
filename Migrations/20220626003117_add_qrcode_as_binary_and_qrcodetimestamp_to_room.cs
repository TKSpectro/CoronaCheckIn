using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoronaCheckIn.Migrations
{
    public partial class add_qrcode_as_binary_and_qrcodetimestamp_to_room : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "QrCodeTimestamp",
                table: "Rooms");

            migrationBuilder.RenameColumn(
                name: "TMP_QrCodeTimestamp",
                table: "Rooms",
                newName: "QrCodeTimestamp");

            migrationBuilder.RenameColumn(
                name: "TMP_QrCode",
                table: "Rooms",
                newName: "QrCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Rooms",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCode",
                table: "Rooms",
                type: "bigint",
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "QrCodeTimestamp",
                table: "Rooms",
                newName: "TMP_QrCodeTimestamp");

            migrationBuilder.RenameColumn(
                name: "QrCode",
                table: "Rooms",
                newName: "TMP_QrCode");
        }
    }
}
