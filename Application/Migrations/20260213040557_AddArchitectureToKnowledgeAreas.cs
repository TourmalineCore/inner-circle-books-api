using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddArchitectureToKnowledgeAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "KnowledgeAreas",
                columns: new[] { "Id", "Name" },
                values: new object[] { 16L, "Architecture" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 16L);
        }
    }
}
