using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class hospitalupdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HospitalId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UseSystemHospitals",
                table: "InsuranceCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_HospitalId",
                table: "Orders",
                column: "HospitalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Hospitals_HospitalId",
                table: "Orders",
                column: "HospitalId",
                principalTable: "Hospitals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Hospitals_HospitalId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_HospitalId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HospitalId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UseSystemHospitals",
                table: "InsuranceCompanies");
        }
    }
}
