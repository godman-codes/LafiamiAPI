using Microsoft.EntityFrameworkCore.Migrations;

namespace LafiamiAPI.Datas.Migrations
{
    public partial class dependentQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasDependency",
                table: "FIndAPlanQuestions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DependentQuestionId",
                table: "FindAPlanQuestionAnswers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasDependency",
                table: "FIndAPlanQuestions");

            migrationBuilder.DropColumn(
                name: "DependentQuestionId",
                table: "FindAPlanQuestionAnswers");
        }
    }
}
