using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Main.Migrations
{
    public partial class _20180117001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(nullable: true),
                    JoinedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PhotoAbsoluteUrl = table.Column<string>(nullable: true),
                    PhotoRelativeUrl = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedTime = table.Column<double>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Photo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Account_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Body = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SignalrConnection",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalrConnection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignalrConnection_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(nullable: true),
                    ExpiredTime = table.Column<double>(nullable: true),
                    IssuedTime = table.Column<double>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowCategory",
                columns: table => new
                {
                    OwnerId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowCategory", x => new { x.OwnerId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_FollowCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowCategory_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categorization",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorization", x => new { x.CategoryId, x.PostId });
                    table.UniqueConstraint("AK_Categorization_PostId_CategoryId", x => new { x.PostId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_Categorization_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categorization_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true),
                    OwnerId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowPost",
                columns: table => new
                {
                    FollowerId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowPost", x => new { x.FollowerId, x.PostId });
                    table.ForeignKey(
                        name: "FK_FollowPost_Account_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowPost_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostNotification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BroadcasterId = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false),
                    PostIndex = table.Column<int>(nullable: false),
                    RecipientId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostNotification_Account_BroadcasterId",
                        column: x => x.BroadcasterId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNotification_Post_PostIndex",
                        column: x => x.PostIndex,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostNotification_Account_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostReport",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false),
                    ReporterId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReport", x => new { x.PostId, x.ReporterId, x.OwnerId });
                    table.UniqueConstraint("AK_PostReport_PostId_ReporterId", x => new { x.PostId, x.ReporterId });
                    table.ForeignKey(
                        name: "FK_PostReport_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostReport_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostReport_Account_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentNotification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BroadcasterId = table.Column<int>(nullable: false),
                    CommentId = table.Column<int>(nullable: false),
                    Created = table.Column<double>(nullable: false),
                    IsSeen = table.Column<bool>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    RecipientId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentNotification_Account_BroadcasterId",
                        column: x => x.BroadcasterId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentNotification_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentNotification_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentNotification_Account_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentReport",
                columns: table => new
                {
                    CommentId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    ReporterId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    Created = table.Column<double>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReport", x => new { x.CommentId, x.PostId, x.ReporterId, x.OwnerId });
                    table.UniqueConstraint("AK_CommentReport_CommentId_OwnerId", x => new { x.CommentId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_CommentReport_Comment_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentReport_Account_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentReport_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentReport_Account_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatorId",
                table: "Category",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_OwnerId",
                table: "Comment",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentNotification_BroadcasterId",
                table: "CommentNotification",
                column: "BroadcasterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentNotification_CommentId",
                table: "CommentNotification",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentNotification_PostId",
                table: "CommentNotification",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentNotification_RecipientId",
                table: "CommentNotification",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReport_OwnerId",
                table: "CommentReport",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReport_PostId",
                table: "CommentReport",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReport_ReporterId",
                table: "CommentReport",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowCategory_CategoryId",
                table: "FollowCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowPost_PostId",
                table: "FollowPost",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_OwnerId",
                table: "Post",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNotification_BroadcasterId",
                table: "PostNotification",
                column: "BroadcasterId");

            migrationBuilder.CreateIndex(
                name: "IX_PostNotification_PostIndex",
                table: "PostNotification",
                column: "PostIndex");

            migrationBuilder.CreateIndex(
                name: "IX_PostNotification_RecipientId",
                table: "PostNotification",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReport_OwnerId",
                table: "PostReport",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReport_ReporterId",
                table: "PostReport",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_SignalrConnection_OwnerId",
                table: "SignalrConnection",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_OwnerId",
                table: "Token",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categorization");

            migrationBuilder.DropTable(
                name: "CommentNotification");

            migrationBuilder.DropTable(
                name: "CommentReport");

            migrationBuilder.DropTable(
                name: "FollowCategory");

            migrationBuilder.DropTable(
                name: "FollowPost");

            migrationBuilder.DropTable(
                name: "PostNotification");

            migrationBuilder.DropTable(
                name: "PostReport");

            migrationBuilder.DropTable(
                name: "SignalrConnection");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
