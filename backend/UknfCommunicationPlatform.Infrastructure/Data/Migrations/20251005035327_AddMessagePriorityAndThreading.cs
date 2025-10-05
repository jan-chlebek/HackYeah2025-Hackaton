using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMessagePriorityAndThreading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_cancelled",
                table: "messages");

            migrationBuilder.AddColumn<long>(
                name: "parent_message_id",
                table: "messages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "messages",
                type: "text",
                nullable: false,
                defaultValue: "Normal");

            migrationBuilder.CreateIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id",
                principalTable: "messages",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "parent_message_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "priority",
                table: "messages");

            migrationBuilder.AddColumn<bool>(
                name: "is_cancelled",
                table: "messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
