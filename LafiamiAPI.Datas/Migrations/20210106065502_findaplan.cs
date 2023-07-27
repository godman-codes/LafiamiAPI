using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class findaplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FIndAPlanQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    OrderBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FIndAPlanQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FindAPlanQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderBy = table.Column<int>(type: "int", nullable: false),
                    FIndAPlanQuestionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FindAPlanQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FindAPlanQuestionAnswers_FIndAPlanQuestions_FIndAPlanQuestionId",
                        column: x => x.FIndAPlanQuestionId,
                        principalTable: "FIndAPlanQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePlanAnswerAsTags",
                columns: table => new
                {
                    InsurancePlanId = table.Column<long>(type: "bigint", nullable: false),
                    FindAPlanQuestionAnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePlanAnswerAsTags", x => new { x.FindAPlanQuestionAnswerId, x.InsurancePlanId });
                    table.ForeignKey(
                        name: "FK_InsurancePlanAnswerAsTags_FindAPlanQuestionAnswers_FindAPlanQuestionAnswerId",
                        column: x => x.FindAPlanQuestionAnswerId,
                        principalTable: "FindAPlanQuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsurancePlanAnswerAsTags_InsurancePlans_InsurancePlanId",
                        column: x => x.InsurancePlanId,
                        principalTable: "InsurancePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FindAPlanQuestionAnswers_FIndAPlanQuestionId",
                table: "FindAPlanQuestionAnswers",
                column: "FIndAPlanQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePlanAnswerAsTags_InsurancePlanId",
                table: "InsurancePlanAnswerAsTags",
                column: "InsurancePlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsurancePlanAnswerAsTags");

            migrationBuilder.DropTable(
                name: "FindAPlanQuestionAnswers");

            migrationBuilder.DropTable(
                name: "FIndAPlanQuestions");
        }
    }
}
