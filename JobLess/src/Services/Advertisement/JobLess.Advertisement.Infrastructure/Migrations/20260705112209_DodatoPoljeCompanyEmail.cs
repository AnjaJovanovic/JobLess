using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Advertisement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DodatoPoljeCompanyEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyEmail",
                table: "JobAdvertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyEmail",
                table: "JobAdvertisements");
        }
    }
}
