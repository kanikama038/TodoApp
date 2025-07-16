using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoApp.Migrations
{
    /// <inheritdoc />
    public partial class MergedMasterController : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerReviewee_Users_RevieweeId",
                table: "ReviewerReviewee");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerReviewee_Users_ReviewerId",
                table: "ReviewerReviewee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewerReviewee",
                table: "ReviewerReviewee");

            migrationBuilder.RenameTable(
                name: "ReviewerReviewee",
                newName: "ReviewerReviewees");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewerReviewee_ReviewerId",
                table: "ReviewerReviewees",
                newName: "IX_ReviewerReviewees_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewerReviewee_RevieweeId",
                table: "ReviewerReviewees",
                newName: "IX_ReviewerReviewees_RevieweeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewerReviewees",
                table: "ReviewerReviewees",
                columns: new[] { "ReviewerId", "RevieweeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerReviewees_Users_RevieweeId",
                table: "ReviewerReviewees",
                column: "RevieweeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerReviewees_Users_ReviewerId",
                table: "ReviewerReviewees",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerReviewees_Users_RevieweeId",
                table: "ReviewerReviewees");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewerReviewees_Users_ReviewerId",
                table: "ReviewerReviewees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewerReviewees",
                table: "ReviewerReviewees");

            migrationBuilder.RenameTable(
                name: "ReviewerReviewees",
                newName: "ReviewerReviewee");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewerReviewees_ReviewerId",
                table: "ReviewerReviewee",
                newName: "IX_ReviewerReviewee_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewerReviewees_RevieweeId",
                table: "ReviewerReviewee",
                newName: "IX_ReviewerReviewee_RevieweeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewerReviewee",
                table: "ReviewerReviewee",
                columns: new[] { "ReviewerId", "RevieweeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerReviewee_Users_RevieweeId",
                table: "ReviewerReviewee",
                column: "RevieweeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewerReviewee_Users_ReviewerId",
                table: "ReviewerReviewee",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
