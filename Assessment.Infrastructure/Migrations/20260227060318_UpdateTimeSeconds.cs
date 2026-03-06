using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeSeconds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.RenameColumn(
                name: "TimeTakenSeconds",
                table: "UserAnswers",
                newName: "ElapsedSeconds");

            migrationBuilder.AlterColumn<Guid>(
                name: "SelectedOptionId",
                table: "UserAnswers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "UserAnswers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AnsweredAt",
                table: "UserAnswers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "ApplicantId", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers",
                column: "SelectedOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.RenameColumn(
                name: "ElapsedSeconds",
                table: "UserAnswers",
                newName: "TimeTakenSeconds");

            migrationBuilder.AlterColumn<Guid>(
                name: "SelectedOptionId",
                table: "UserAnswers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCorrect",
                table: "UserAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AnsweredAt",
                table: "UserAnswers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "ApplicantId", "QuestionId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers",
                column: "SelectedOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
