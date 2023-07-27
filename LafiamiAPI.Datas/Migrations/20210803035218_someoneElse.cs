using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class someoneElse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "NextOfKins",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Identifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Banks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NextOfKins_OrderId",
                table: "NextOfKins",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_OrderId",
                table: "Jobs",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Identifications_OrderId",
                table: "Identifications",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_OrderId",
                table: "Banks",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Orders_OrderId",
                table: "Banks",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Identifications_Orders_OrderId",
                table: "Identifications",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Orders_OrderId",
                table: "Jobs",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NextOfKins_Orders_OrderId",
                table: "NextOfKins",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Orders_OrderId",
                table: "Banks");

            migrationBuilder.DropForeignKey(
                name: "FK_Identifications_Orders_OrderId",
                table: "Identifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Orders_OrderId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_NextOfKins_Orders_OrderId",
                table: "NextOfKins");

            migrationBuilder.DropIndex(
                name: "IX_NextOfKins_OrderId",
                table: "NextOfKins");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_OrderId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Identifications_OrderId",
                table: "Identifications");

            migrationBuilder.DropIndex(
                name: "IX_Banks_OrderId",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "NextOfKins");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Identifications");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Banks");
        }
    }
}
