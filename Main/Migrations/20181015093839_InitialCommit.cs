using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Main.Migrations
{
    public partial class InitialCommit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SignalrConnection",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    LastActivityTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalrConnection", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(nullable: false),
                    Nickname = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    Photo = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    JoinedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRealTimeGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Group = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRealTimeGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessToken",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    IssuedTime = table.Column<double>(nullable: false),
                    ExpiredTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => new { x.Code, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_AccessToken_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivationToken",
                columns: table => new
                {
                    Code = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    IssuedTime = table.Column<double>(nullable: false),
                    ExpiredTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationToken", x => new { x.Code, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_ActivationToken_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryGroup_User_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    ExtraInfo = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationMessage_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserDeviceToken",
                columns: table => new
                {
                    DeviceId = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeviceToken", x => x.DeviceId);
                    table.ForeignKey(
                        name: "FK_UserDeviceToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatorId = table.Column<int>(nullable: false),
                    CategoryGroupId = table.Column<int>(nullable: false),
                    Photo = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_CategoryGroup_CategoryGroupId",
                        column: x => x.CategoryGroupId,
                        principalTable: "CategoryGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_User_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowCategory",
                columns: table => new
                {
                    FollowerId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowCategory", x => new { x.FollowerId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_FollowCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowCategory_User_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    CategoryGroupId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_CategoryGroup_CategoryGroupId",
                        column: x => x.CategoryGroupId,
                        principalTable: "CategoryGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topic_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Topic_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategorySummary",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    TotalPost = table.Column<int>(nullable: false),
                    TotalFollower = table.Column<int>(nullable: false),
                    LastTopicId = table.Column<int>(nullable: false),
                    LastTopicTitle = table.Column<string>(nullable: true),
                    LastTopicCreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySummary", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_CategorySummary_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategorySummary_Topic_LastTopicId",
                        column: x => x.LastTopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FollowTopic",
                columns: table => new
                {
                    FollowerId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowTopic", x => new { x.FollowerId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_FollowTopic_User_FollowerId",
                        column: x => x.FollowerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FollowTopic_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reply",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    CategoryGroupId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reply_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reply_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportTopic",
                columns: table => new
                {
                    TopicId = table.Column<int>(nullable: false),
                    ReporterId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    LastModifiedTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTopic", x => new { x.TopicId, x.ReporterId });
                    table.ForeignKey(
                        name: "FK_ReportTopic_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportTopic_User_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportTopic_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicSummary",
                columns: table => new
                {
                    TopicId = table.Column<int>(nullable: false),
                    TotalFollower = table.Column<int>(nullable: false),
                    TotalReply = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicSummary", x => x.TopicId);
                    table.ForeignKey(
                        name: "FK_TopicSummary_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessToken_OwnerId",
                table: "AccessToken",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivationToken_OwnerId",
                table: "ActivationToken",
                column: "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_CategoryGroupId",
                table: "Category",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatorId",
                table: "Category",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGroup_CreatorId",
                table: "CategoryGroup",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CategorySummary_LastTopicId",
                table: "CategorySummary",
                column: "LastTopicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FollowCategory_CategoryId",
                table: "FollowCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowTopic_TopicId",
                table: "FollowTopic",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessage_OwnerId",
                table: "NotificationMessage",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_OwnerId",
                table: "Reply",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reply_TopicId",
                table: "Reply",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTopic_OwnerId",
                table: "ReportTopic",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTopic_ReporterId",
                table: "ReportTopic",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CategoryGroupId",
                table: "Topic",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_CategoryId",
                table: "Topic",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_OwnerId",
                table: "Topic",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDeviceToken_UserId",
                table: "UserDeviceToken",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessToken");

            migrationBuilder.DropTable(
                name: "ActivationToken");

            migrationBuilder.DropTable(
                name: "CategorySummary");

            migrationBuilder.DropTable(
                name: "FollowCategory");

            migrationBuilder.DropTable(
                name: "FollowTopic");

            migrationBuilder.DropTable(
                name: "NotificationMessage");

            migrationBuilder.DropTable(
                name: "Reply");

            migrationBuilder.DropTable(
                name: "ReportTopic");

            migrationBuilder.DropTable(
                name: "SignalrConnection");

            migrationBuilder.DropTable(
                name: "TopicSummary");

            migrationBuilder.DropTable(
                name: "UserDeviceToken");

            migrationBuilder.DropTable(
                name: "UserRealTimeGroup");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "CategoryGroup");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
