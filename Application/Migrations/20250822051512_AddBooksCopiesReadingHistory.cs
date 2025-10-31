using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class AddBooksCopiesReadingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BooksCopiesReadingHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookCopyId = table.Column<long>(type: "bigint", nullable: false),
                    ReaderEmployeeId = table.Column<long>(type: "bigint", nullable: false),
                    TakenAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SсheduledReturnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActualReturnedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BooksCopiesReadingHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BooksCopiesReadingHistory_BooksCopies_BookCopyId",
                        column: x => x.BookCopyId,
                        principalTable: "BooksCopies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BooksCopiesReadingHistory_BookCopyId",
                table: "BooksCopiesReadingHistory",
                column: "BookCopyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BooksCopiesReadingHistory");
        }
    }
}
