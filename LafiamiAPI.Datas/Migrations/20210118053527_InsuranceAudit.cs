using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class InsuranceAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InsuranceAudits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HasResult = table.Column<bool>(type: "bit", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceAudits_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAuditCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    InsuranceAuditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAuditCategories", x => new { x.CategoryId, x.InsuranceAuditId });
                    table.ForeignKey(
                        name: "FK_InsuranceAuditCategories_Categorys_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceAuditCategories_InsuranceAudits_InsuranceAuditId",
                        column: x => x.InsuranceAuditId,
                        principalTable: "InsuranceAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceAuditQuestionAnswers",
                columns: table => new
                {
                    FindAPlanQuestionAnswerId = table.Column<int>(type: "int", nullable: false),
                    InsuranceAuditId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceAuditQuestionAnswers", x => new { x.FindAPlanQuestionAnswerId, x.InsuranceAuditId });
                    table.ForeignKey(
                        name: "FK_InsuranceAuditQuestionAnswers_FindAPlanQuestionAnswers_FindAPlanQuestionAnswerId",
                        column: x => x.FindAPlanQuestionAnswerId,
                        principalTable: "FindAPlanQuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceAuditQuestionAnswers_InsuranceAudits_InsuranceAuditId",
                        column: x => x.InsuranceAuditId,
                        principalTable: "InsuranceAudits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAuditCategories_InsuranceAuditId",
                table: "InsuranceAuditCategories",
                column: "InsuranceAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAuditQuestionAnswers_InsuranceAuditId",
                table: "InsuranceAuditQuestionAnswers",
                column: "InsuranceAuditId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceAudits_UserId",
                table: "InsuranceAudits",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuranceAuditCategories");

            migrationBuilder.DropTable(
                name: "InsuranceAuditQuestionAnswers");

            migrationBuilder.DropTable(
                name: "InsuranceAudits");
        }
    }
}
