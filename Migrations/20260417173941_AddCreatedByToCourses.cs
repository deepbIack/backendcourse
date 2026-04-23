using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Courses",
                type: "integer",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedById",
                table: "Courses",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Users_CreatedById",
                table: "Courses",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Users_CreatedById",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CreatedById",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Courses");

            
        }
    }
}
