using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class DoctorRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rights",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rights",
                table: "Doctors");
        }
    }
}
