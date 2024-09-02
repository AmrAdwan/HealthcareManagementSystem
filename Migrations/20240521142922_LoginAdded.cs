using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthcareManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class LoginAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Patients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Doctors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Billings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Doctors");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Billings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}
