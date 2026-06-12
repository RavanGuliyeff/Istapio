using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Istapio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateRelationBetweenVacationTypeAndJobPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VacationTypeId",
                table: "JobPosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_JobPosts_VacationTypeId",
                table: "JobPosts",
                column: "VacationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobPosts_VacationTypes_VacationTypeId",
                table: "JobPosts",
                column: "VacationTypeId",
                principalTable: "VacationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobPosts_VacationTypes_VacationTypeId",
                table: "JobPosts");

            migrationBuilder.DropIndex(
                name: "IX_JobPosts_VacationTypeId",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "VacationTypeId",
                table: "JobPosts");
        }
    }
}
