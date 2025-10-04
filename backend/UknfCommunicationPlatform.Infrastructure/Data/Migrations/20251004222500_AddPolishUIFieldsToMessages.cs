using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPolishUIFieldsToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "identyfikator",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sygnatura_sprawy",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "podmiot",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_wiadomosci",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "priorytet",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_przeslania_podmiotu",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "uzytkownik",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wiadomosc_uzytkownika",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_przeslania_uknf",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pracownik_uknf",
                table: "messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wiadomosc_pracownika_uknf",
                table: "messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "identyfikator",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "sygnatura_sprawy",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "podmiot",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "status_wiadomosci",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "priorytet",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "data_przeslania_podmiotu",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "uzytkownik",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "wiadomosc_uzytkownika",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "data_przeslania_uknf",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "pracownik_uknf",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "wiadomosc_pracownika_uknf",
                table: "messages");
        }
    }
}
