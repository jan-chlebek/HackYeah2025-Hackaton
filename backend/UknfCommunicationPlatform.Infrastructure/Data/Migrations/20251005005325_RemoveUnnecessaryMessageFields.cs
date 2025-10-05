using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryMessageFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_related_case_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_related_report_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "cancelled_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "related_case_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "related_report_id",
                table: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "cancelled_at",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "related_case_id",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "related_report_id",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_messages_related_case_id",
                table: "messages",
                column: "related_case_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_related_report_id",
                table: "messages",
                column: "related_report_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages",
                column: "related_case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages",
                column: "related_report_id",
                principalTable: "reports",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
