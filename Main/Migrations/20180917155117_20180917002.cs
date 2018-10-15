using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Main.Migrations
{
    public partial class _20180917002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "KeyName",
                "UserDeviceToken");

            migrationBuilder.AlterColumn<string>(
                "DeviceId",
                "UserDeviceToken",
                nullable: false,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<double>(
                "CreatedTime",
                "UserDeviceToken",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                "UserId",
                "UserDeviceToken",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                "IX_UserDeviceToken_UserId",
                "UserDeviceToken",
                "UserId");

            migrationBuilder.AddForeignKey(
                "FK_UserDeviceToken_User_UserId",
                "UserDeviceToken",
                "UserId",
                "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                "FK_UserDeviceToken_User_UserId",
                "UserDeviceToken");

            migrationBuilder.DropIndex(
                "IX_UserDeviceToken_UserId",
                "UserDeviceToken");

            migrationBuilder.DropColumn(
                "CreatedTime",
                "UserDeviceToken");

            migrationBuilder.DropColumn(
                "UserId",
                "UserDeviceToken");

            migrationBuilder.AlterColumn<Guid>(
                "DeviceId",
                "UserDeviceToken",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                "KeyName",
                "UserDeviceToken",
                nullable: false,
                defaultValue: "");
        }
    }
}