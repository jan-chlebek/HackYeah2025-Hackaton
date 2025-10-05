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
            // Only add the priority column - parent_message_id already exists from initial schema
            migrationBuilder.AddColumn<string>(
                name: "priority",
                table: "messages",
                type: "text",
                nullable: false,
                defaultValue: "Normal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "priority",
                table: "messages");
        }
    }
}
