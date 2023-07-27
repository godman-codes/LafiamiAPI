using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class hygeiaorderupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HygeiaDependantId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HygeiaCompletionOrderId",
                table: "EmailLogs",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HygeiaDependantId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HygeiaCompletionOrderId",
                table: "EmailLogs");
        }
    }
}
