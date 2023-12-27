using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Migrations
{
    /// <inheritdoc />
    public partial class I : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluation_Books_BookId",
                table: "Evaluation");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Evaluation",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluation_Books_BookId",
                table: "Evaluation",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluation_Books_BookId",
                table: "Evaluation");

            migrationBuilder.AlterColumn<int>(
                name: "BookId",
                table: "Evaluation",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluation_Books_BookId",
                table: "Evaluation",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
