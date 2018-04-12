using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Main.Migrations
{
    public partial class _20180411001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FcmGroup",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    MessagingKey = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FcmGroup", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FcmGroup");
        }
    }
}
