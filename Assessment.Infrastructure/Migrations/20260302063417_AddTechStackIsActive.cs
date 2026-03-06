using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTechStackIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HrTestTechStacks_TechStacks_TechStackId",
                table: "HrTestTechStacks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_HrApplicants_ApplicantId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId_TestId_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_TechStacks_Name",
                table: "TechStacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HrTestTechStacks",
                table: "HrTestTechStacks");

            migrationBuilder.DropIndex(
                name: "IX_HrTestTechStacks_TestId_TechStackId",
                table: "HrTestTechStacks");

            migrationBuilder.DropIndex(
                name: "IX_HrTests_CreatedAtUtc",
                table: "HrTests");

            migrationBuilder.DropIndex(
                name: "IX_HrApplicants_Email",
                table: "HrApplicants");

            migrationBuilder.AlterColumn<int>(
                name: "ElapsedSeconds",
                table: "UserAnswers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TechStacks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TechStacks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "TechStackId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "HrTests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "HrTests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "HrApplicants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "HrApplicants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "HrApplicants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "HrApplicants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HrTestTechStacks",
                table: "HrTestTechStacks",
                columns: new[] { "TestId", "TechStackId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId",
                table: "UserAnswers",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_TestId",
                table: "UserAnswers",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_TechStackId",
                table: "Questions",
                column: "TechStackId");

            migrationBuilder.AddForeignKey(
                name: "FK_HrTestTechStacks_TechStacks_TechStackId",
                table: "HrTestTechStacks",
                column: "TechStackId",
                principalTable: "TechStacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_TechStacks_TechStackId",
                table: "Questions",
                column: "TechStackId",
                principalTable: "TechStacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers",
                column: "SelectedOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_HrApplicants_ApplicantId",
                table: "UserAnswers",
                column: "ApplicantId",
                principalTable: "HrApplicants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_HrTests_TestId",
                table: "UserAnswers",
                column: "TestId",
                principalTable: "HrTests",
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
                name: "FK_HrTestTechStacks_TechStacks_TechStackId",
                table: "HrTestTechStacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_TechStacks_TechStackId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_HrApplicants_ApplicantId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_HrTests_TestId",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_QuestionId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ApplicantId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_TestId",
                table: "UserAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Questions_TechStackId",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HrTestTechStacks",
                table: "HrTestTechStacks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TechStacks");

            migrationBuilder.DropColumn(
                name: "TechStackId",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "ElapsedSeconds",
                table: "UserAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "TechStacks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Questions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "HrTests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "HrTests",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "HrApplicants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "HrApplicants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "HrApplicants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "HrApplicants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HrTestTechStacks",
                table: "HrTestTechStacks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ApplicantId_TestId_QuestionId",
                table: "UserAnswers",
                columns: new[] { "ApplicantId", "TestId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechStacks_Name",
                table: "TechStacks",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrTestTechStacks_TestId_TechStackId",
                table: "HrTestTechStacks",
                columns: new[] { "TestId", "TechStackId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrTests_CreatedAtUtc",
                table: "HrTests",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HrApplicants_Email",
                table: "HrApplicants",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HrTestTechStacks_TechStacks_TechStackId",
                table: "HrTestTechStacks",
                column: "TechStackId",
                principalTable: "TechStacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_AnswerOptions_SelectedOptionId",
                table: "UserAnswers",
                column: "SelectedOptionId",
                principalTable: "AnswerOptions",
                principalColumn: "Id");

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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
