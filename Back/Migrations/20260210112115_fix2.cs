using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back.Migrations
{
    /// <inheritdoc />
    public partial class fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FamilyRelationTypes_ReverseRelationTypeId",
                table: "FamilyRelationTypes",
                column: "ReverseRelationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyRelationTypes_FamilyRelationTypes_ReverseRelationTypeId",
                table: "FamilyRelationTypes",
                column: "ReverseRelationTypeId",
                principalTable: "FamilyRelationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyRelationTypes_FamilyRelationTypes_ReverseRelationTypeId",
                table: "FamilyRelationTypes");

            migrationBuilder.DropIndex(
                name: "IX_FamilyRelationTypes_ReverseRelationTypeId",
                table: "FamilyRelationTypes");
        }
    }
}
