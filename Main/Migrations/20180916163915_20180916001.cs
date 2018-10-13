using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Main.Migrations
{
    public partial class _20180916001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "SignalrConnection",
                table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    LastActivityTime = table.Column<double>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_SignalrConnection", x => x.ClientId); });

            migrationBuilder.CreateTable(
                "SignalrConnectionGroup",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    Group = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_SignalrConnectionGroup", x => x.Id); });

            migrationBuilder.CreateTable(
                "User",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
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
                constraints: table => { table.PrimaryKey("PK_User", x => x.Id); });

            migrationBuilder.CreateTable(
                "AccessToken",
                table => new
                {
                    Code = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    IssuedTime = table.Column<double>(nullable: false),
                    ExpiredTime = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessToken", x => new {x.Code, x.OwnerId});
                    table.ForeignKey(
                        "FK_AccessToken_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "ActivationToken",
                table => new
                {
                    Code = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    IssuedTime = table.Column<double>(nullable: false),
                    ExpiredTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationToken", x => new {x.Code, x.OwnerId});
                    table.ForeignKey(
                        "FK_ActivationToken_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "CategoryGroup",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
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
                        "FK_CategoryGroup_User_CreatorId",
                        x => x.CreatorId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "NotificationMessage",
                table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false),
                    ExtraInfo = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessage", x => x.Id);
                    table.ForeignKey(
                        "FK_NotificationMessage_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Category",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
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
                        "FK_Category_CategoryGroup_CategoryGroupId",
                        x => x.CategoryGroupId,
                        "CategoryGroup",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Category_User_CreatorId",
                        x => x.CreatorId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "FollowCategory",
                table => new
                {
                    FollowerId = table.Column<int>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowCategory", x => new {x.FollowerId, x.CategoryId});
                    table.ForeignKey(
                        "FK_FollowCategory_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_FollowCategory_User_FollowerId",
                        x => x.FollowerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Topic",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
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
                        "FK_Topic_CategoryGroup_CategoryGroupId",
                        x => x.CategoryGroupId,
                        "CategoryGroup",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Topic_Category_CategoryId",
                        x => x.CategoryId,
                        "Category",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Topic_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "FollowTopic",
                table => new
                {
                    FollowerId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedTime = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowTopic", x => new {x.FollowerId, x.TopicId});
                    table.ForeignKey(
                        "FK_FollowTopic_User_FollowerId",
                        x => x.FollowerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_FollowTopic_Topic_TopicId",
                        x => x.TopicId,
                        "Topic",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Reply",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
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
                        "FK_Reply_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Reply_Topic_TopicId",
                        x => x.TopicId,
                        "Topic",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "ReportTopic",
                table => new
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
                    table.PrimaryKey("PK_ReportTopic", x => new {x.TopicId, x.ReporterId});
                    table.ForeignKey(
                        "FK_ReportTopic_User_OwnerId",
                        x => x.OwnerId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_ReportTopic_User_ReporterId",
                        x => x.ReporterId,
                        "User",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_ReportTopic_Topic_TopicId",
                        x => x.TopicId,
                        "Topic",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_AccessToken_OwnerId",
                "AccessToken",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_ActivationToken_OwnerId",
                "ActivationToken",
                "OwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                "IX_Category_CategoryGroupId",
                "Category",
                "CategoryGroupId");

            migrationBuilder.CreateIndex(
                "IX_Category_CreatorId",
                "Category",
                "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_CategoryGroup_CreatorId",
                "CategoryGroup",
                "CreatorId");

            migrationBuilder.CreateIndex(
                "IX_FollowCategory_CategoryId",
                "FollowCategory",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_FollowTopic_TopicId",
                "FollowTopic",
                "TopicId");

            migrationBuilder.CreateIndex(
                "IX_NotificationMessage_OwnerId",
                "NotificationMessage",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_Reply_OwnerId",
                "Reply",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_Reply_TopicId",
                "Reply",
                "TopicId");

            migrationBuilder.CreateIndex(
                "IX_ReportTopic_OwnerId",
                "ReportTopic",
                "OwnerId");

            migrationBuilder.CreateIndex(
                "IX_ReportTopic_ReporterId",
                "ReportTopic",
                "ReporterId");

            migrationBuilder.CreateIndex(
                "IX_Topic_CategoryGroupId",
                "Topic",
                "CategoryGroupId");

            migrationBuilder.CreateIndex(
                "IX_Topic_CategoryId",
                "Topic",
                "CategoryId");

            migrationBuilder.CreateIndex(
                "IX_Topic_OwnerId",
                "Topic",
                "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "AccessToken");

            migrationBuilder.DropTable(
                "ActivationToken");

            migrationBuilder.DropTable(
                "FollowCategory");

            migrationBuilder.DropTable(
                "FollowTopic");

            migrationBuilder.DropTable(
                "NotificationMessage");

            migrationBuilder.DropTable(
                "Reply");

            migrationBuilder.DropTable(
                "ReportTopic");

            migrationBuilder.DropTable(
                "SignalrConnection");

            migrationBuilder.DropTable(
                "SignalrConnectionGroup");

            migrationBuilder.DropTable(
                "Topic");

            migrationBuilder.DropTable(
                "Category");

            migrationBuilder.DropTable(
                "CategoryGroup");

            migrationBuilder.DropTable(
                "User");
        }
    }
}