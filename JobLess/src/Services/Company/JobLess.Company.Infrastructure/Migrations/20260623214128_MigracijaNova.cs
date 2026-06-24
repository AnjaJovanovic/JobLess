using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobLess.Company.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigracijaNova : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IndustryTemp",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 8);

            migrationBuilder.Sql("""
                UPDATE Companies SET IndustryTemp = CASE Industry
                    WHEN N'Informacione tehnologije' THEN 1
                    WHEN N'Finansije i bankarstvo' THEN 2
                    WHEN N'Maloprodaja i usluge' THEN 3
                    WHEN N'Industrija i proizvodnja' THEN 4
                    WHEN N'Zdravstvo' THEN 5
                    WHEN N'Građevinarstvo' THEN 6
                    WHEN N'Mediji i marketing' THEN 7
                    ELSE 8
                END
                """);

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "IndustryTemp",
                table: "Companies",
                newName: "Industry");

            migrationBuilder.AddColumn<int>(
                name: "CompanySizeTemp",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE Companies SET CompanySizeTemp = CASE CompanySize
                    WHEN N'1-10' THEN 1
                    WHEN N'11-50' THEN 2
                    WHEN N'51-200' THEN 3
                    WHEN N'201-500' THEN 4
                    WHEN N'500+' THEN 5
                    ELSE 1
                END
                """);

            migrationBuilder.DropColumn(
                name: "CompanySize",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "CompanySizeTemp",
                table: "Companies",
                newName: "CompanySize");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OwnerName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CompanySize",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
