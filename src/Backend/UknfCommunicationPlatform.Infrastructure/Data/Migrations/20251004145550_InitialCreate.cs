using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "supervised_entities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    UKNFCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LEI = table.Column<string>(type: "text", nullable: true),
                    NIP = table.Column<string>(type: "text", nullable: true),
                    KRS = table.Column<string>(type: "text", nullable: true),
                    Street = table.Column<string>(type: "text", nullable: false),
                    BuildingNumber = table.Column<string>(type: "text", nullable: false),
                    ApartmentNumber = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RegistryNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Sector = table.Column<string>(type: "text", nullable: true),
                    Subsector = table.Column<string>(type: "text", nullable: true),
                    IsCrossBorder = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_supervised_entities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    LastName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    PESEL = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_users_supervised_entities_SupervisedEntityId",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Subject = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<long>(type: "bigint", nullable: false),
                    RecipientId = table.Column<long>(type: "bigint", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RelatedEntityId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedReportId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_messages_users_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_messages_users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReportNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    ReportingPeriod = table.Column<string>(type: "text", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValidationResultPath = table.Column<string>(type: "text", nullable: true),
                    ErrorDescription = table.Column<string>(type: "text", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsCorrection = table.Column<bool>(type: "boolean", nullable: false),
                    OriginalReportId = table.Column<long>(type: "bigint", nullable: true),
                    SupervisedEntityId = table.Column<long>(type: "bigint", nullable: false),
                    SubmittedByUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reports_reports_OriginalReportId",
                        column: x => x.OriginalReportId,
                        principalTable: "reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_reports_supervised_entities_SupervisedEntityId",
                        column: x => x.SupervisedEntityId,
                        principalTable: "supervised_entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reports_users_SubmittedByUserId",
                        column: x => x.SubmittedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_messages_RecipientId",
                table: "messages",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_SenderId",
                table: "messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_OriginalReportId",
                table: "reports",
                column: "OriginalReportId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_ReportNumber",
                table: "reports",
                column: "ReportNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reports_SubmittedByUserId",
                table: "reports",
                column: "SubmittedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_reports_SupervisedEntityId",
                table: "reports",
                column: "SupervisedEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_supervised_entities_UKNFCode",
                table: "supervised_entities",
                column: "UKNFCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_SupervisedEntityId",
                table: "users",
                column: "SupervisedEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "supervised_entities");
        }
    }
}
