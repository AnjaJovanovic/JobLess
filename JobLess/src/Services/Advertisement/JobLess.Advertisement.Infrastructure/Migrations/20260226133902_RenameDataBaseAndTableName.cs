using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Advertisement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDataBaseAndTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oglasi");

            migrationBuilder.CreateTable(
                name: "JobAdvertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    WorkSchedule = table.Column<int>(type: "int", nullable: false),
                    SeniorityLevel = table.Column<int>(type: "int", nullable: false),
                    MinExperience = table.Column<int>(type: "int", nullable: true),
                    MaxExperience = table.Column<int>(type: "int", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkType = table.Column<int>(type: "int", nullable: false),
                    SalaryFrom = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SalaryTo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSalaryVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAdvertisements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAdvertisements");

            migrationBuilder.CreateTable(
                name: "Oglasi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DatumPostavljanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IskustvoMax = table.Column<int>(type: "int", nullable: true),
                    IskustvoMin = table.Column<int>(type: "int", nullable: true),
                    KompanijaId = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlataDo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlataOd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlataVidljiva = table.Column<bool>(type: "bit", nullable: true),
                    Pozicija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RadnoVreme = table.Column<int>(type: "int", nullable: false),
                    Senioritet = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TipRada = table.Column<int>(type: "int", nullable: false),
                    TipZaposlenja = table.Column<int>(type: "int", nullable: false),
                    Valuta = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oglasi", x => x.Id);
                });
        }
    }
}
