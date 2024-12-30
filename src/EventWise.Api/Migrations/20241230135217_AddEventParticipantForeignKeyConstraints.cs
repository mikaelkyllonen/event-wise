using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventWise.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEventParticipantForeignKeyConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipants_Users_ParticipantId",
                table: "EventParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipants_Users_ParticipantId",
                table: "EventParticipants",
                column: "ParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipants_Users_ParticipantId",
                table: "EventParticipants");

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipants_Users_ParticipantId",
                table: "EventParticipants",
                column: "ParticipantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}