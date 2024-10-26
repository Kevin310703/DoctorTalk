using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorTalkWebApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedColumnToPostReply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "PostReplies",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "PostReplies");
        }
    }
}
