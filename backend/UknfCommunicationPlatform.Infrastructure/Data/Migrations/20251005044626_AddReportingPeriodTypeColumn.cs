using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReportingPeriodTypeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "i_x_reports_supervised_entity_id",
                table: "reports");

            migrationBuilder.DropIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_related_case_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_related_report_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_thread_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "supervised_entity_id",
                table: "reports");

            migrationBuilder.DropColumn(
                name: "cancelled_at",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "folder",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "parent_message_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "related_case_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "related_report_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "thread_id",
                table: "messages");

            migrationBuilder.AddColumn<string>(
                name: "reporting_period_type",
                table: "file_libraries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_file_libraries_reporting_period_type",
                table: "file_libraries",
                column: "reporting_period_type");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_role",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_user",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_role",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_user",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "i_x_file_libraries_reporting_period_type",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "reporting_period_type",
                table: "file_libraries");

            migrationBuilder.AddColumn<long>(
                name: "supervised_entity_id",
                table: "reports",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "cancelled_at",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "folder",
                table: "messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "parent_message_id",
                table: "messages",
                type: "bigint",
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

            migrationBuilder.AddColumn<long>(
                name: "thread_id",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_reports_supervised_entity_id",
                table: "reports",
                column: "supervised_entity_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_related_case_id",
                table: "messages",
                column: "related_case_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_related_report_id",
                table: "messages",
                column: "related_report_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_thread_id",
                table: "messages",
                column: "thread_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_cases_related_case_id",
                table: "messages",
                column: "related_case_id",
                principalTable: "cases",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_reports_related_report_id",
                table: "messages",
                column: "related_report_id",
                principalTable: "reports",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_reports_entities_supervised_entity_id",
                table: "reports",
                column: "supervised_entity_id",
                principalTable: "entities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
