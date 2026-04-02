using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBlobSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "TestSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "BlobKey",
                table: "TestSnapshots",
                type: "nvarchar(500)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlobKey",
                table: "TestSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "ImageData",
                table: "TestSnapshots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
