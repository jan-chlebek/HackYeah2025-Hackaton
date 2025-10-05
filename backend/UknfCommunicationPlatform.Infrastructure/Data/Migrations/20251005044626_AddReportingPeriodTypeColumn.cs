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
            // Simply add the reporting_period_type column to file_libraries
            migrationBuilder.AddColumn<string>(
                name: "reporting_period_type",
                table: "file_libraries",
                type: "text",
                nullable: false,
                defaultValue: "Monthly");

            migrationBuilder.CreateIndex(
                name: "i_x_file_libraries_reporting_period_type",
                table: "file_libraries",
                column: "reporting_period_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_file_libraries_reporting_period_type",
                table: "file_libraries");

            migrationBuilder.DropColumn(
                name: "reporting_period_type",
                table: "file_libraries");
        }
    }
}
