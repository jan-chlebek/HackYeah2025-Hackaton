using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyReportEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "f_k_reports_reports_original_report_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "i_x_reports_original_report_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "i_x_reports_supervised_entity_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "error_description",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "is_correction",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "original_report_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "report_type",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "validated_at",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "validation_result_path",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "supervised_entity_id",
                table: "reports");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "file_content",
                table: "reports",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "i_x_reports_status",
                table: "reports",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "i_x_reports_submitted_at",
                table: "reports",
                column: "submitted_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_reports_status",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "i_x_reports_submitted_at",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "file_content",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "reports");

            migrationBuilder.AddColumn<string>(
                name: "error_description",
                table: "reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_correction",
                table: "reports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "original_report_id",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "report_type",
                table: "reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "supervised_entity_id",
                table: "reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "validated_at",
                table: "reports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "validation_result_path",
                table: "reports",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_reports_original_report_id",
                table: "reports",
                column: "original_report_id");

            migrationBuilder.CreateIndex(
                name: "i_x_reports_supervised_entity_id",
                table: "reports",
                column: "supervised_entity_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_reports_original_report_id",
                table: "reports",
                column: "original_report_id",
                principalTable: "reports",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
