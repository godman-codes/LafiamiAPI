using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class planupdate23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_OrderId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "PartnerPaymentJsonResponse",
                table: "Orders",
                newName: "BackgrundServiceErrorMessage");

            migrationBuilder.AddColumn<int>(
                name: "BackgroundServiceStatus",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BackgrundServiceErrorMessage",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerPaymentJsonResponse",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RunBackgroundService",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BackgroundServiceStatus",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Company",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RunBackgroundService",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "NextOfKins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrequencyPerMonthsAllowed",
                table: "InsurancePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DependentName",
                table: "CompanyExtras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropdownListGetUrl",
                table: "CompanyExtras",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ForProductSetup",
                table: "CompanyExtras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasDependent",
                table: "CompanyExtras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LoadingToBeTrigger",
                table: "CompanyExtras",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlanCompanyExtraResults",
                columns: table => new
                {
                    CompanyExtraId = table.Column<int>(type: "int", nullable: false),
                    InsurancePlanId = table.Column<long>(type: "bigint", nullable: false),
                    ResultInString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultInHTML = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultInNumber = table.Column<int>(type: "int", nullable: true),
                    ResultInDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultInDecimal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanCompanyExtraResults", x => new { x.CompanyExtraId, x.InsurancePlanId });
                    table.ForeignKey(
                        name: "FK_PlanCompanyExtraResults_CompanyExtras_CompanyExtraId",
                        column: x => x.CompanyExtraId,
                        principalTable: "CompanyExtras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanCompanyExtraResults_InsurancePlans_InsurancePlanId",
                        column: x => x.InsurancePlanId,
                        principalTable: "InsurancePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_OrderId",
                table: "Carts",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlanCompanyExtraResults_InsurancePlanId",
                table: "PlanCompanyExtraResults",
                column: "InsurancePlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanCompanyExtraResults");

            migrationBuilder.DropIndex(
                name: "IX_Carts_OrderId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "BackgroundServiceStatus",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BackgrundServiceErrorMessage",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PartnerPaymentJsonResponse",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "RunBackgroundService",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BackgroundServiceStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RunBackgroundService",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "NextOfKins");

            migrationBuilder.DropColumn(
                name: "FrequencyPerMonthsAllowed",
                table: "InsurancePlans");

            migrationBuilder.DropColumn(
                name: "DependentName",
                table: "CompanyExtras");

            migrationBuilder.DropColumn(
                name: "DropdownListGetUrl",
                table: "CompanyExtras");

            migrationBuilder.DropColumn(
                name: "ForProductSetup",
                table: "CompanyExtras");

            migrationBuilder.DropColumn(
                name: "HasDependent",
                table: "CompanyExtras");

            migrationBuilder.DropColumn(
                name: "LoadingToBeTrigger",
                table: "CompanyExtras");

            migrationBuilder.RenameColumn(
                name: "BackgrundServiceErrorMessage",
                table: "Orders",
                newName: "PartnerPaymentJsonResponse");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_OrderId",
                table: "Carts",
                column: "OrderId");
        }
    }
}
