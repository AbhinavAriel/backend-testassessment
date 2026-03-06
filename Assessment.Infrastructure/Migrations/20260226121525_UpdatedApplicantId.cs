using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApplicantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AspNetUsers_UserId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_UserId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserAnswers",
                newName: "ApplicantId");

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
                name: "FK_UserAnswers_HrApplicants_ApplicantId",
                table: "UserAnswers",
                column: "ApplicantId",
                principalTable: "HrApplicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_HrApplicants_ApplicantId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.RenameColumn(
                name: "ApplicantId",
                table: "UserAnswers",
                newName: "UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AnsweredAt",
                table: "UserAnswers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_UserId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "UserId", "QuestionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AspNetUsers_UserId",
                table: "UserAnswers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
