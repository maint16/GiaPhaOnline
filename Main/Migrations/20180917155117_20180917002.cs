using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Main.Migrations
{
    public partial class _20180917002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyName",
                table: "UserDeviceToken");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "UserDeviceToken",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<double>(
                name: "CreatedTime",
                table: "UserDeviceToken",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserDeviceToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserDeviceToken_UserId",
                table: "UserDeviceToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDeviceToken_User_UserId",
                table: "UserDeviceToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDeviceToken_User_UserId",
                table: "UserDeviceToken");

            migrationBuilder.DropIndex(
                name: "IX_UserDeviceToken_UserId",
                table: "UserDeviceToken");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "UserDeviceToken");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserDeviceToken");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "UserDeviceToken",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "KeyName",
                table: "UserDeviceToken",
                nullable: false,
                defaultValue: "");
        }
    }
}
