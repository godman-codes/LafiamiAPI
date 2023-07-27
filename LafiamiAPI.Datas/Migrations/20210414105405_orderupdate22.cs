using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class orderupdate22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BackgrundServiceErrorMessage",
                table: "Payments",
                newName: "IntegrationErrorMessage");

            migrationBuilder.RenameColumn(
                name: "BackgroundServiceStatus",
                table: "Payments",
                newName: "IntegrationStatus");

            migrationBuilder.RenameColumn(
                name: "BackgrundServiceErrorMessage",
                table: "Orders",
                newName: "IntegrationErrorMessage");

            migrationBuilder.RenameColumn(
                name: "BackgroundServiceStatus",
                table: "Orders",
                newName: "IntegrationStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IntegrationStatus",
                table: "Payments",
                newName: "BackgroundServiceStatus");

            migrationBuilder.RenameColumn(
                name: "IntegrationErrorMessage",
                table: "Payments",
                newName: "BackgrundServiceErrorMessage");

            migrationBuilder.RenameColumn(
                name: "IntegrationStatus",
                table: "Orders",
                newName: "BackgroundServiceStatus");

            migrationBuilder.RenameColumn(
                name: "IntegrationErrorMessage",
                table: "Orders",
                newName: "BackgrundServiceErrorMessage");
        }
    }
}
