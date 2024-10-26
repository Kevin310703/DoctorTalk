using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorTalkWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddParentReplyIdToPostReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentReplyId",
                table: "PostReplies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReplies_ParentReplyId",
                table: "PostReplies",
                column: "ParentReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReplies_PostReplies_ParentReplyId",
                table: "PostReplies",
                column: "ParentReplyId",
                principalTable: "PostReplies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostReplies_PostReplies_ParentReplyId",
                table: "PostReplies");

            migrationBuilder.DropIndex(
                name: "IX_PostReplies_ParentReplyId",
                table: "PostReplies");

            migrationBuilder.DropColumn(
                name: "ParentReplyId",
                table: "PostReplies");
        }
    }
}
