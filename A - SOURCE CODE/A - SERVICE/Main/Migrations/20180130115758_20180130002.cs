using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Main.Migrations
{
    public partial class _20180130002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "PhotoAbsoluteUrl",
                table: "Category",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoRelativeUrl",
                table: "Category",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoAbsoluteUrl",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "PhotoRelativeUrl",
                table: "Category");

            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Category",
                nullable: true);
        }
    }
}
