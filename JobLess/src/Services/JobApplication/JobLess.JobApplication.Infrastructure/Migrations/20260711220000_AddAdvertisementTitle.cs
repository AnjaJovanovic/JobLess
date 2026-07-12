using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.JobApplication.Infrastructure.Migrations
{
    public partial class AddAdvertisementTitle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdvertisementTitle",
                table: "JobApplications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdvertisementTitle",
                table: "JobApplications");
        }
    }
}
