using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduledReturnDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SсheduledReturnDate",
                table: "BooksCopiesReadingHistory",
                newName: "ScheduledReturnDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledReturnDate",
                table: "BooksCopiesReadingHistory",
                newName: "SсheduledReturnDate");
        }
    }
}
