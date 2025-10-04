using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CommunicationModuleEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_messages_RecipientId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_SenderId",
                table: "messages");

            migrationBuilder.AlterColumn<long>(
                name: "RecipientId",
                table: "messages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Folder",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ParentMessageId",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RelatedCaseId",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ThreadId",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "announcements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcements_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: false),
                    HandlerId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCancelled = table.Column<bool>(type: "boolean", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cases_supervised_entities_SupervisedEntityId",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cases_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cases_users_HandlerId",
                        column: x => x.HandlerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "contact_groups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contact_groups_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Position = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Mobile = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: true),
                    Department = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contacts_supervised_entities_SupervisedEntityId",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contacts_users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "faq_questions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AnswerContent = table.Column<string>(type: "text", nullable: true),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AnsweredByUserId = table.Column<long>(type: "bigint", nullable: true),
                    SubmittedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    AnonymousName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AnonymousEmail = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ViewCount = table.Column<int>(type: "integer", nullable: false),
                    AverageRating = table.Column<decimal>(type: "numeric", nullable: true),
                    RatingCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faq_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_faq_questions_users_AnsweredByUserId",
                        column: x => x.AnsweredByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_faq_questions_users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "file_libraries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsCurrentVersion = table.Column<bool>(type: "boolean", nullable: false),
                    ParentFileId = table.Column<long>(type: "bigint", nullable: true),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    UploadedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_libraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_libraries_file_libraries_ParentFileId",
                        column: x => x.ParentFileId,
                        principalTable: "file_libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_file_libraries_users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "message_attachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MessageId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedByUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_message_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_message_attachments_messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_message_attachments_users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "announcement_attachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementId = table.Column<long>(type: "bigint", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement_attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcement_attachments_announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "announcement_histories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementId = table.Column<long>(type: "bigint", nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ChangedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcement_histories_announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_announcement_histories_users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "announcement_reads",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement_reads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcement_reads_announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_announcement_reads_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "announcement_recipients",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnnouncementId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: true),
                    PodmiotType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement_recipients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_announcement_recipients_announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_announcement_recipients_supervised_entities_SupervisedEntit~",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_announcement_recipients_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_documents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedByUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_documents_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_documents_users_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "case_histories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseId = table.Column<long>(type: "bigint", nullable: false),
                    ChangeType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldStatus = table.Column<int>(type: "integer", nullable: true),
                    NewStatus = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ChangedByUserId = table.Column<long>(type: "bigint", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_histories_cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_case_histories_users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contact_group_members",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactGroupId = table.Column<long>(type: "bigint", nullable: false),
                    ContactId = table.Column<long>(type: "bigint", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_group_members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contact_group_members_contact_groups_ContactGroupId",
                        column: x => x.ContactGroupId,
                        principalTable: "contact_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact_group_members_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "faq_ratings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FaqQuestionId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    RatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faq_ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_faq_ratings_faq_questions_FaqQuestionId",
                        column: x => x.FaqQuestionId,
                        principalTable: "faq_questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_faq_ratings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "file_library_permissions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileLibraryId = table.Column<long>(type: "bigint", nullable: false),
                    PermissionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: true),
                    PodmiotType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CanRead = table.Column<bool>(type: "boolean", nullable: false),
                    CanWrite = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file_library_permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_library_permissions_file_libraries_FileLibraryId",
                        column: x => x.FileLibraryId,
                        principalTable: "file_libraries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_library_permissions_supervised_entities_SupervisedEnti~",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_file_library_permissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messages_ParentMessageId",
                table: "messages",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_RecipientId_IsRead",
                table: "messages",
                columns: new[] { "RecipientId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_RelatedCaseId",
                table: "messages",
                column: "RelatedCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_RelatedEntityId",
                table: "messages",
                column: "RelatedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_RelatedReportId",
                table: "messages",
                column: "RelatedReportId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_SenderId_SentAt",
                table: "messages",
                columns: new[] { "SenderId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_ThreadId",
                table: "messages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_attachments_AnnouncementId",
                table: "announcement_attachments",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_histories_AnnouncementId_ChangedAt",
                table: "announcement_histories",
                columns: new[] { "AnnouncementId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_announcement_histories_ChangedByUserId",
                table: "announcement_histories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_reads_AnnouncementId_UserId",
                table: "announcement_reads",
                columns: new[] { "AnnouncementId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_announcement_reads_ReadAt",
                table: "announcement_reads",
                column: "ReadAt");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_reads_UserId",
                table: "announcement_reads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_recipients_AnnouncementId_RecipientType",
                table: "announcement_recipients",
                columns: new[] { "AnnouncementId", "RecipientType" });

            migrationBuilder.CreateIndex(
                name: "IX_announcement_recipients_SupervisedEntityId",
                table: "announcement_recipients",
                column: "SupervisedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_announcement_recipients_UserId",
                table: "announcement_recipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_CreatedByUserId",
                table: "announcements",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_ExpiresAt",
                table: "announcements",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_announcements_IsPublished_PublishedAt",
                table: "announcements",
                columns: new[] { "IsPublished", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_CaseId",
                table: "case_documents",
                column: "CaseId");

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_UploadedByUserId",
                table: "case_documents",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_case_histories_CaseId_ChangedAt",
                table: "case_histories",
                columns: new[] { "CaseId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_case_histories_ChangedByUserId",
                table: "case_histories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CaseNumber",
                table: "cases",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cases_CreatedAt",
                table: "cases",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CreatedByUserId",
                table: "cases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_HandlerId",
                table: "cases",
                column: "HandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_SupervisedEntityId_Status",
                table: "cases",
                columns: new[] { "SupervisedEntityId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_contact_group_members_ContactGroupId_ContactId",
                table: "contact_group_members",
                columns: new[] { "ContactGroupId", "ContactId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contact_group_members_ContactId",
                table: "contact_group_members",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_groups_CreatedByUserId",
                table: "contact_groups",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_CreatedByUserId",
                table: "contacts",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_Email",
                table: "contacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_SupervisedEntityId_IsPrimary",
                table: "contacts",
                columns: new[] { "SupervisedEntityId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_faq_questions_AnsweredByUserId",
                table: "faq_questions",
                column: "AnsweredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_faq_questions_PublishedAt",
                table: "faq_questions",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_faq_questions_Status_Category",
                table: "faq_questions",
                columns: new[] { "Status", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_faq_questions_SubmittedAt",
                table: "faq_questions",
                column: "SubmittedAt");

            migrationBuilder.CreateIndex(
                name: "IX_faq_questions_SubmittedByUserId",
                table: "faq_questions",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_faq_ratings_FaqQuestionId_UserId",
                table: "faq_ratings",
                columns: new[] { "FaqQuestionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_faq_ratings_UserId",
                table: "faq_ratings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_file_libraries_Category_IsCurrentVersion",
                table: "file_libraries",
                columns: new[] { "Category", "IsCurrentVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_file_libraries_ParentFileId",
                table: "file_libraries",
                column: "ParentFileId");

            migrationBuilder.CreateIndex(
                name: "IX_file_libraries_UploadedAt",
                table: "file_libraries",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_file_libraries_UploadedByUserId",
                table: "file_libraries",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_file_library_permissions_FileLibraryId_PermissionType",
                table: "file_library_permissions",
                columns: new[] { "FileLibraryId", "PermissionType" });

            migrationBuilder.CreateIndex(
                name: "IX_file_library_permissions_SupervisedEntityId",
                table: "file_library_permissions",
                column: "SupervisedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_file_library_permissions_UserId",
                table: "file_library_permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_message_attachments_MessageId",
                table: "message_attachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_message_attachments_UploadedByUserId",
                table: "message_attachments",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_cases_RelatedCaseId",
                table: "messages",
                column: "RelatedCaseId",
                principalTable: "cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages",
                column: "ParentMessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_reports_RelatedReportId",
                table: "messages",
                column: "RelatedReportId",
                principalTable: "reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_supervised_entities_RelatedEntityId",
                table: "messages",
                column: "RelatedEntityId",
                principalTable: "supervised_entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_cases_RelatedCaseId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_reports_RelatedReportId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_supervised_entities_RelatedEntityId",
                table: "messages");

            migrationBuilder.DropTable(
                name: "announcement_attachments");

            migrationBuilder.DropTable(
                name: "announcement_histories");

            migrationBuilder.DropTable(
                name: "announcement_reads");

            migrationBuilder.DropTable(
                name: "announcement_recipients");

            migrationBuilder.DropTable(
                name: "case_documents");

            migrationBuilder.DropTable(
                name: "case_histories");

            migrationBuilder.DropTable(
                name: "contact_group_members");

            migrationBuilder.DropTable(
                name: "faq_ratings");

            migrationBuilder.DropTable(
                name: "file_library_permissions");

            migrationBuilder.DropTable(
                name: "message_attachments");

            migrationBuilder.DropTable(
                name: "announcements");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "contact_groups");

            migrationBuilder.DropTable(
                name: "contacts");

            migrationBuilder.DropTable(
                name: "faq_questions");

            migrationBuilder.DropTable(
                name: "file_libraries");

            migrationBuilder.DropIndex(
                name: "IX_messages_ParentMessageId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_RecipientId_IsRead",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_RelatedCaseId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_RelatedEntityId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_RelatedReportId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_SenderId_SentAt",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_ThreadId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "Folder",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ParentMessageId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "RelatedCaseId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "messages");

            migrationBuilder.AlterColumn<long>(
                name: "RecipientId",
                table: "messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_messages_RecipientId",
                table: "messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_SenderId",
                table: "messages",
                column: "SenderId");
        }
    }
}
