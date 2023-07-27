using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class companyupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePlans_InsuranceCompanies_InsuranceCompanyId",
                table: "InsurancePlans");

            migrationBuilder.DropIndex(
                name: "IX_InsurancePlans_InsuranceCompanyId",
                table: "InsurancePlans");

            migrationBuilder.DropColumn(
                name: "InsuranceCompanyId",
                table: "InsurancePlans");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "InsuranceCompanies",
                newName: "Logo");

            migrationBuilder.AddColumn<int>(
                name: "Company",
                table: "InsurancePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Company",
                table: "InsuranceCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "InsurancePlans");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "InsuranceCompanies");

            migrationBuilder.RenameColumn(
                name: "Logo",
                table: "InsuranceCompanies",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "InsuranceCompanyId",
                table: "InsurancePlans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePlans_InsuranceCompanyId",
                table: "InsurancePlans",
                column: "InsuranceCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePlans_InsuranceCompanies_InsuranceCompanyId",
                table: "InsurancePlans",
                column: "InsuranceCompanyId",
                principalTable: "InsuranceCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
