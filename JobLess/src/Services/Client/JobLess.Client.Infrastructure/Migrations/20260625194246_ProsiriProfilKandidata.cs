using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Client.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProsiriProfilKandidata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Clients",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationEndYear",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationLevel",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationStartYear",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalSummary",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "Clients",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "EducationEndYear",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "EducationStartYear",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "InstitutionName",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ProfessionalSummary",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "Clients");
        }
    }
}
