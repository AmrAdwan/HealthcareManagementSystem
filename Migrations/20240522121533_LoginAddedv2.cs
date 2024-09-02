using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class LoginAddedv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password1",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password2",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password1",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Password2",
                table: "Patients");
        }
    }
}
