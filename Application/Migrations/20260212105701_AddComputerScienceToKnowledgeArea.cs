using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddComputerScienceToKnowledgeArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "KnowledgeAreas",
                columns: new[] { "Id", "Name" },
                values: new object[] { 15L, "Computer Science" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 15L);
        }
    }
}
