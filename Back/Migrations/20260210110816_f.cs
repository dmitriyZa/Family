using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back.Migrations
{
    /// <inheritdoc />
    public partial class f : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelationTypeId",
                table: "Relationships",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "FamilyMembers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FamilyRelationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    ReverseRelationTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Emoji = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    BaseType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyRelationTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Relationships_RelationTypeId",
                table: "Relationships",
                column: "RelationTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relationships_FamilyRelationTypes_RelationTypeId",
                table: "Relationships",
                column: "RelationTypeId",
                principalTable: "FamilyRelationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relationships_FamilyRelationTypes_RelationTypeId",
                table: "Relationships");

            migrationBuilder.DropTable(
                name: "FamilyRelationTypes");

            migrationBuilder.DropIndex(
                name: "IX_Relationships_RelationTypeId",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "RelationTypeId",
                table: "Relationships");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "FamilyMembers");
        }
    }
}
