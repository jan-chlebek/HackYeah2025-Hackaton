using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMessageThreadAndFolderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_thread_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "folder",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "parent_message_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "thread_id",
                table: "messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "thread_id",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_thread_id",
                table: "messages",
                column: "thread_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
