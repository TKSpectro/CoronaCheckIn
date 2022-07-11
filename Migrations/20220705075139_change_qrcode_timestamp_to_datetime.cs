using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoronaCheckIn.Migrations
{
    public partial class change_qrcode_timestamp_to_datetime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeTimestamp",
                table: "Rooms");

            migrationBuilder.AddColumn<DateTime>(
                name: "QrCodeCreatedAt",
                table: "Rooms",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeCreatedAt",
                table: "Rooms");

            migrationBuilder.AddColumn<long>(
                name: "QrCodeTimestamp",
                table: "Rooms",
                type: "bigint",
                nullable: true);
        }
    }
}
