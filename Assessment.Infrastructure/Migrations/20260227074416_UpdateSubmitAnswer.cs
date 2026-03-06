using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubmitAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "ElapsedSeconds",
                table: "UserAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "TestId",
                table: "UserAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "AnsweredCount",
                table: "HrTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CorrectCount",
                table: "HrTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAtUtc",
                table: "HrTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId_TestId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "ApplicantId", "TestId", "QuestionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_TestId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "AnsweredCount",
                table: "HrTests");

            migrationBuilder.DropColumn(
                name: "CorrectCount",
                table: "HrTests");

            migrationBuilder.DropColumn(
                name: "SubmittedAtUtc",
                table: "HrTests");

            migrationBuilder.AlterColumn<int>(
                name: "ElapsedSeconds",
                table: "UserAnswers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "ApplicantId", "QuestionId" },
                unique: true);
        }
    }
}
