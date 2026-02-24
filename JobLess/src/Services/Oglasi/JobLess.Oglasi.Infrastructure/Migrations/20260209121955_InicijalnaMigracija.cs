using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MATFInfostud.Oglasi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicijalnaMigracija : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Oglasi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KorisnikId = table.Column<int>(type: "int", nullable: false),
                    KompanijaId = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pozicija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DatumPostavljanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumIsteka = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktivan = table.Column<bool>(type: "bit", nullable: false),
                    TipZaposlenja = table.Column<int>(type: "int", nullable: false),
                    RadnoVreme = table.Column<int>(type: "int", nullable: false),
                    Senioritet = table.Column<int>(type: "int", nullable: false),
                    IskustvoMin = table.Column<int>(type: "int", nullable: true),
                    IskustvoMax = table.Column<int>(type: "int", nullable: true),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Drzava = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipRada = table.Column<int>(type: "int", nullable: false),
                    PlataOd = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlataDo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Valuta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlataVidljiva = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oglasi", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Oglasi");
        }
    }
}
