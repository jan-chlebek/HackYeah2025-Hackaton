using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UknfCommunicationPlatform.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_file_libraries_file_libraries_parent_file_id",
                table: "file_libraries");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_roles_users_user_id1",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "i_x_user_roles_user_id1",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "i_x_file_libraries_parent_file_id",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "parent_file_id",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "version",
                table: "file_libraries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id1",
                table: "user_roles",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "parent_file_id",
                table: "file_libraries",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "version",
                table: "file_libraries",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_user_roles_user_id1",
                table: "user_roles",
                column: "user_id1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_file_libraries_parent_file_id",
                table: "file_libraries",
                column: "parent_file_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_file_libraries_file_libraries_parent_file_id",
                table: "file_libraries",
                column: "parent_file_id",
                principalTable: "file_libraries",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_roles_users_user_id1",
                table: "user_roles",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
