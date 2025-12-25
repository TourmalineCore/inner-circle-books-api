using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddKnowledgeAreasToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnowledgeAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BookKnowledgeArea",
                columns: table => new
                {
                    BooksId = table.Column<long>(type: "bigint", nullable: false),
                    KnowledgeAreasId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookKnowledgeArea", x => new { x.BooksId, x.KnowledgeAreasId });
                    table.ForeignKey(
                        name: "FK_BookKnowledgeArea_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookKnowledgeArea_KnowledgeAreas_KnowledgeAreasId",
                        column: x => x.KnowledgeAreasId,
                        principalTable: "KnowledgeAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "KnowledgeAreas",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Frontend" },
                    { 2, "Backend" },
                    { 3, "ML" },
                    { 4, "DevOps" },
                    { 5, "QA" },
                    { 6, "Design" },
                    { 7, "Business and Management" },
                    { 8, "Embedded" },
                    { 9, "GameDev" },
                    { 10, "Marketing" },
                    { 11, "Information Security" },
                    { 12, "Psychology" },
                    { 13, "Copywriting and Editing" },
                    { 14, "Languages" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookKnowledgeArea_KnowledgeAreasId",
                table: "BookKnowledgeArea",
                column: "KnowledgeAreasId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookKnowledgeArea");

            migrationBuilder.DropTable(
                name: "KnowledgeAreas");
        }
    }
}
