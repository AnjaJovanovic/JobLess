using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Advertisement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UklonjenoPoljeStatusOglasa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobAdvertisements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JobAdvertisements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
