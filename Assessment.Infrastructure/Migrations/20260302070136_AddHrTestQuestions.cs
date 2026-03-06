using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHrTestQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HrTestQuestions",
                columns: table => new
                {
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HrTestQuestions", x => new { x.TestId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_HrTestQuestions_HrTests_TestId",
                        column: x => x.TestId,
                        principalTable: "HrTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HrTestQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrTestQuestions_QuestionId",
                table: "HrTestQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTestQuestions_TestId_Order",
                table: "HrTestQuestions",
                columns: new[] { "TestId", "Order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrTestQuestions");
        }
    }
}
