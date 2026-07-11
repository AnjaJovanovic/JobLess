using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Notification.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationMetadataFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AdvertisementId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CandidateId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Notifications",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvertisementId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Notifications");
        }
    }
}
