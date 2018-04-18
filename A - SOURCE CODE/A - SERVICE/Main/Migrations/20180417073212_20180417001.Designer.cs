﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SystemConstant.Enumerations;
using SystemDatabase.Models.Contexts;

namespace Main.Migrations
{
    [DbContext(typeof(RelationalDatabaseContext))]
    [Migration("20180417073212_20180417001")]
    partial class _20180417001
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-preview1-28290")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SystemDatabase.Models.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email");

                    b.Property<double>("JoinedTime");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<string>("Nickname");

                    b.Property<string>("Password");

                    b.Property<string>("PhotoAbsoluteUrl");

                    b.Property<string>("PhotoRelativeUrl");

                    b.Property<int>("Role");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Categorization", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("CategoryId");

                    b.Property<double>("CategorizationTime");

                    b.HasKey("PostId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Categorization");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("CreatedTime");

                    b.Property<int>("CreatorId");

                    b.Property<string>("Description");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<string>("Name");

                    b.Property<string>("PhotoAbsoluteUrl");

                    b.Property<string>("PhotoRelativeUrl");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Content");

                    b.Property<double>("CreatedTime");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<int>("OwnerId");

                    b.Property<int>("PostId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PostId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.CommentNotification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BroadcasterId");

                    b.Property<int>("CommentId");

                    b.Property<double>("CreatedTime");

                    b.Property<int>("PostId");

                    b.Property<int>("RecipientId");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BroadcasterId");

                    b.HasIndex("CommentId");

                    b.HasIndex("PostId");

                    b.HasIndex("RecipientId");

                    b.ToTable("CommentNotification");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.CommentReport", b =>
                {
                    b.Property<int>("CommentId");

                    b.Property<int>("OwnerId");

                    b.Property<string>("Body");

                    b.Property<double>("CreatedTime");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<int>("PostId");

                    b.Property<string>("Reason");

                    b.Property<int>("ReporterId");

                    b.Property<int>("Status");

                    b.HasKey("CommentId", "OwnerId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("PostId");

                    b.HasIndex("ReporterId");

                    b.ToTable("CommentReport");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Device", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CreatedTime");

                    b.Property<int?>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.FcmGroup", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CreatedTime");

                    b.Property<string>("MessagingKey")
                        .IsRequired();

                    b.HasKey("Name");

                    b.ToTable("FcmGroup");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.FollowCategory", b =>
                {
                    b.Property<int>("FollowerId");

                    b.Property<int>("CategoryId");

                    b.Property<double>("CreatedTime");

                    b.Property<int>("Status");

                    b.HasKey("FollowerId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("FollowCategory");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.FollowPost", b =>
                {
                    b.Property<int>("FollowerId");

                    b.Property<int>("PostId");

                    b.Property<double>("CreatedTime");

                    b.Property<int>("Status");

                    b.HasKey("FollowerId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("FollowPost");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Body");

                    b.Property<double>("CreatedTime");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<int>("OwnerId");

                    b.Property<int>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.PostNotification", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BroadcasterId");

                    b.Property<double>("CreatedTime");

                    b.Property<int>("PostId");

                    b.Property<int>("RecipientId");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("BroadcasterId");

                    b.HasIndex("PostId");

                    b.HasIndex("RecipientId");

                    b.ToTable("PostNotification");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.PostReport", b =>
                {
                    b.Property<int>("PostId");

                    b.Property<int>("ReporterId");

                    b.Property<string>("Body");

                    b.Property<double>("CreatedTime");

                    b.Property<double?>("LastModifiedTime");

                    b.Property<int>("OwnerId");

                    b.Property<string>("Reason");

                    b.Property<int>("Status");

                    b.HasKey("PostId", "ReporterId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ReporterId");

                    b.ToTable("PostReport");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.SignalrConnection", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("CreatedTime");

                    b.Property<int>("OwnerId");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("SignalrConnection");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<double?>("ExpiredTime");

                    b.Property<double>("IssuedTime");

                    b.Property<int>("OwnerId");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Token");
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Categorization", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Category", "Category")
                        .WithMany("Categorizations")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("Categorizations")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Category", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Creator")
                        .WithMany("InitializedCategories")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Comment", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Owner")
                        .WithMany("Comments")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.CommentNotification", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Broadcaster")
                        .WithMany("BroadcastedCommentNotifications")
                        .HasForeignKey("BroadcasterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Comment", "Comment")
                        .WithMany("CommentNotifications")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("CommentNotifications")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "Recipient")
                        .WithMany("ReceivedCommentNotifications")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.CommentReport", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Comment", "Comment")
                        .WithMany("CommentReports")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "CommentOwner")
                        .WithMany("OwnedCommentReports")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("ReportedComments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "CommentReporter")
                        .WithMany("ReportedComments")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Device", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Owner")
                        .WithMany("Devices")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.FollowCategory", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Category", "Category")
                        .WithMany("FollowCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "Follower")
                        .WithMany("FollowCategories")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.FollowPost", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Follower")
                        .WithMany("FollowPosts")
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("FollowPosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Post", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Owner")
                        .WithMany("Posts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.PostNotification", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Broadcaster")
                        .WithMany("BroadcastedPostNotifications")
                        .HasForeignKey("BroadcasterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("PostNotifications")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "Recipient")
                        .WithMany("ReceivedPostNotifications")
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.PostReport", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "PostOwner")
                        .WithMany("OwnedPostReports")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Post", "Post")
                        .WithMany("PostReports")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SystemDatabase.Models.Entities.Account", "PostReporter")
                        .WithMany("ReportedPosts")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.SignalrConnection", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Owner")
                        .WithMany("SignalrConnections")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("SystemDatabase.Models.Entities.Token", b =>
                {
                    b.HasOne("SystemDatabase.Models.Entities.Account", "Owner")
                        .WithMany("Tokens")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
