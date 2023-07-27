using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class planupdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoneyUnit",
                table: "InsurancePrices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlanType",
                table: "InsurancePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyUnit",
                table: "InsurancePrices");

            migrationBuilder.DropColumn(
                name: "PlanType",
                table: "InsurancePlans");
        }
    }
}
