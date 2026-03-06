using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHrModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrApplicants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrApplicants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HrTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SkillSet = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrTests_HrApplicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "HrApplicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HrTestTechStacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tech = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTestTechStacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HrTestTechStacks_HrTests_TestId",
                        column: x => x.TestId,
                        principalTable: "HrTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrApplicants_Email",
                table: "HrApplicants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HrTests_ApplicantId",
                table: "HrTests",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTests_CreatedAtUtc",
                table: "HrTests",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_HrTestTechStacks_TestId_Tech",
                table: "HrTestTechStacks",
                columns: new[] { "TestId", "Tech" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrTestTechStacks");

            migrationBuilder.DropTable(
                name: "HrTests");

            migrationBuilder.DropTable(
                name: "HrApplicants");
        }
    }
}
