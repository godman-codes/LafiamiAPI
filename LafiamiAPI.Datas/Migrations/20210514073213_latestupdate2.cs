using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class latestupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HospitalCashScheduleJsonResponse",
                table: "Orders",
                newName: "IntegrationBackgroundJsonResponse");

            migrationBuilder.AddColumn<bool>(
                name: "ResultInBool",
                table: "UserCompanyExtraResults",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ResultInBool",
                table: "PlanCompanyExtraResults",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HygeiaLegacyCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HygeiaMemberId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultInBool",
                table: "UserCompanyExtraResults");

            migrationBuilder.DropColumn(
                name: "ResultInBool",
                table: "PlanCompanyExtraResults");

            migrationBuilder.DropColumn(
                name: "HygeiaLegacyCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HygeiaMemberId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "IntegrationBackgroundJsonResponse",
                table: "Orders",
                newName: "HospitalCashScheduleJsonResponse");
        }
    }
}
