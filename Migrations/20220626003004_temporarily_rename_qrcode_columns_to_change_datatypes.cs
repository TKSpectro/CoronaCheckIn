using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoronaCheckIn.Migrations
{
    public partial class temporarily_rename_qrcode_columns_to_change_datatypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "TMP_QrCode",
                table: "Rooms",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TMP_QrCodeTimestamp",
                table: "Rooms",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TMP_QrCode",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TMP_QrCodeTimestamp",
                table: "Rooms");
        }
    }
}
