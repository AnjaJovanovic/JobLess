using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Oglasi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IzbrisanoNepotrebnoPolje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KorisnikId",
                table: "Oglasi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KorisnikId",
                table: "Oglasi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
