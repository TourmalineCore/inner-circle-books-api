using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class ChangeKnowledgeAreasIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "KnowledgeAreas",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<long>(
                name: "KnowledgeAreasId",
                table: "BookKnowledgeArea",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "KnowledgeAreas",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1L, "Frontend" },
                    { 2L, "Backend" },
                    { 3L, "ML" },
                    { 4L, "DevOps" },
                    { 5L, "QA" },
                    { 6L, "Design" },
                    { 7L, "Business and Management" },
                    { 8L, "Embedded" },
                    { 9L, "GameDev" },
                    { 10L, "Marketing" },
                    { 11L, "Information Security" },
                    { 12L, "Psychology" },
                    { 13L, "Copywriting and Editing" },
                    { 14L, "Languages" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "KnowledgeAreas",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "KnowledgeAreas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "KnowledgeAreasId",
                table: "BookKnowledgeArea",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

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
        }
    }
}
