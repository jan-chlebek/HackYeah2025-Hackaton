using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyFaqQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_faq_questions_users_answered_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropForeignKey(
                name: "f_k_faq_questions_users_submitted_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropTable(
                name: "faq_ratings");

            migrationBuilder.DropIndex(
                name: "i_x_faq_questions_answered_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropIndex(
                name: "i_x_faq_questions_published_at",
                table: "faq_questions");

            migrationBuilder.DropIndex(
                name: "i_x_faq_questions_status_category",
                table: "faq_questions");

            migrationBuilder.DropIndex(
                name: "i_x_faq_questions_submitted_at",
                table: "faq_questions");

            migrationBuilder.DropIndex(
                name: "i_x_faq_questions_submitted_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "anonymous_email",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "anonymous_name",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "answer_content",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "answered_at",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "answered_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "average_rating",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "category",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "content",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "published_at",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "rating_count",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "submitted_at",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "submitted_by_user_id",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "title",
                table: "faq_questions");

            migrationBuilder.DropColumn(
                name: "view_count",
                table: "faq_questions");

            migrationBuilder.RenameColumn(
                name: "tags",
                table: "faq_questions",
                newName: "question");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "faq_questions",
                newName: "answer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "question",
                table: "faq_questions",
                newName: "tags");

            migrationBuilder.RenameColumn(
                name: "answer",
                table: "faq_questions",
                newName: "status");

            migrationBuilder.AddColumn<string>(
                name: "anonymous_email",
                table: "faq_questions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "anonymous_name",
                table: "faq_questions",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "answer_content",
                table: "faq_questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "answered_at",
                table: "faq_questions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "answered_by_user_id",
                table: "faq_questions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "average_rating",
                table: "faq_questions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "faq_questions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "faq_questions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "published_at",
                table: "faq_questions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rating_count",
                table: "faq_questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "submitted_at",
                table: "faq_questions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "submitted_by_user_id",
                table: "faq_questions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "faq_questions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "view_count",
                table: "faq_questions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "faq_ratings",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    faq_question_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    rated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_faq_ratings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_faq_ratings_faq_questions_faq_question_id",
                        column: x => x.faq_question_id,
                        principalTable: "faq_questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_faq_ratings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_faq_questions_answered_by_user_id",
                table: "faq_questions",
                column: "answered_by_user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_faq_questions_published_at",
                table: "faq_questions",
                column: "published_at");

            migrationBuilder.CreateIndex(
                name: "i_x_faq_questions_status_category",
                table: "faq_questions",
                columns: new[] { "status", "category" });

            migrationBuilder.CreateIndex(
                name: "i_x_faq_questions_submitted_at",
                table: "faq_questions",
                column: "submitted_at");

            migrationBuilder.CreateIndex(
                name: "i_x_faq_questions_submitted_by_user_id",
                table: "faq_questions",
                column: "submitted_by_user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_faq_ratings_faq_question_id_user_id",
                table: "faq_ratings",
                columns: new[] { "faq_question_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_faq_ratings_user_id",
                table: "faq_ratings",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_questions_users_answered_by_user_id",
                table: "faq_questions",
                column: "answered_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_faq_questions_users_submitted_by_user_id",
                table: "faq_questions",
                column: "submitted_by_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
