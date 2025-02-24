using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructionProjectManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructionProjects",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProjectLocation = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ProjectStage = table.Column<int>(type: "integer", nullable: false),
                    ProjectCategory = table.Column<int>(type: "integer", nullable: false),
                    OtherCategory = table.Column<string>(type: "text", nullable: false),
                    ProjectConstructionStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectDetails = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ProjectCreatorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructionProjects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Password" },
                values: new object[] { new Guid("12345678-1234-1234-1234-1234567890ab"), "test@admin.com", "$2a$11$fjG27TwzU.jmCj0hf2hf3OxvfY5wYgX3Y10AbZTXcnttBJBmE8Z6K" });

            migrationBuilder.CreateIndex(
                name: "IX_ConstructionProjects_ProjectName",
                table: "ConstructionProjects",
                column: "ProjectName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructionProjects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
