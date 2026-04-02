using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSupabaseSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BlobKey",
                table: "TestSnapshots",
                newName: "ImagePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImagePath",
                table: "TestSnapshots",
                newName: "BlobKey");
        }
    }
}
