using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Client.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DodatoPoljeGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Clients");
        }
    }
}
