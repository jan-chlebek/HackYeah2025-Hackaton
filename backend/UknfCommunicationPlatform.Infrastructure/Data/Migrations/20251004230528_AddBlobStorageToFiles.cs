using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBlobStorageToFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content_hash",
                table: "message_attachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "file_content",
                table: "message_attachments",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "file_content",
                table: "file_libraries",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "i_x_message_attachments_content_hash",
                table: "message_attachments",
                column: "content_hash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_message_attachments_content_hash",
                table: "message_attachments");

            migrationBuilder.DropColumn(
                name: "content_hash",
                table: "message_attachments");

            migrationBuilder.DropColumn(
                name: "file_content",
                table: "message_attachments");

            migrationBuilder.DropColumn(
                name: "file_content",
                table: "file_libraries");
        }
    }
}
