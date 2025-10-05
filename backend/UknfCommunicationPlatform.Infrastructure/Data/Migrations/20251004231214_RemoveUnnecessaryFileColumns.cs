using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryFileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_message_attachments_content_hash",
                table: "message_attachments");

            migrationBuilder.DropIndex(
                name: "i_x_file_libraries_category_is_current_version",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "content_hash",
                table: "message_attachments");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "message_attachments");

            migrationBuilder.DropColumn(
                name: "download_count",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "file_path",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "is_current_version",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "is_public",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "tags",
                table: "file_libraries");

            migrationBuilder.CreateIndex(
                name: "i_x_file_libraries_category",
                table: "file_libraries",
                column: "category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_file_libraries_category",
                table: "file_libraries");

            migrationBuilder.AddColumn<string>(
                name: "content_hash",
                table: "message_attachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "message_attachments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "download_count",
                table: "file_libraries",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "file_libraries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "is_current_version",
                table: "file_libraries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                table: "file_libraries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "tags",
                table: "file_libraries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_message_attachments_content_hash",
                table: "message_attachments",
                column: "content_hash");

            migrationBuilder.CreateIndex(
                name: "i_x_file_libraries_category_is_current_version",
                table: "file_libraries",
                columns: new[] { "category", "is_current_version" });
        }
    }
}
