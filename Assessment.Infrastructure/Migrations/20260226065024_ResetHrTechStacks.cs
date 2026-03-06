using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    public partial class ResetHrTechStacks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrTestTechStacks");

            migrationBuilder.CreateTable(
                name: "TechStacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechStacks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechStacks_Name",
                table: "TechStacks",
                column: "Name",
                unique: true);

            var techStacks = new (Guid Id, string Name)[]
            {
                (Guid.NewGuid(), "Angular"),
                (Guid.NewGuid(), "React"),
                (Guid.NewGuid(), "JavaScript"),
                (Guid.NewGuid(), "TypeScript"),
                (Guid.NewGuid(), "C#"),
                (Guid.NewGuid(), "HTML"),
                (Guid.NewGuid(), "CSS"),
                (Guid.NewGuid(), "Python")
            };

            foreach (var t in techStacks)
            {
                migrationBuilder.InsertData(
                    table: "TechStacks",
                    columns: new[] { "Id", "Name" },
                    values: new object[] { t.Id, t.Name }
                );
            }

            migrationBuilder.CreateTable(
                name: "HrTestTechStacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechStackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

                    table.ForeignKey(
                        name: "FK_HrTestTechStacks_TechStacks_TechStackId",
                        column: x => x.TechStackId,
                        principalTable: "TechStacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HrTestTechStacks_TestId",
                table: "HrTestTechStacks",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTestTechStacks_TechStackId",
                table: "HrTestTechStacks",
                column: "TechStackId");

            migrationBuilder.CreateIndex(
                name: "IX_HrTestTechStacks_TestId_TechStackId",
                table: "HrTestTechStacks",
                columns: new[] { "TestId", "TechStackId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HrTestTechStacks");

            migrationBuilder.DropTable(
                name: "TechStacks");

        }
    }
}