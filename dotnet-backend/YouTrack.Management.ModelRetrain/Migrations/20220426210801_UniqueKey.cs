using Microsoft.EntityFrameworkCore.Migrations;

namespace YouTrack.Management.ReTrain.Migrations
{
    public partial class UniqueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectKey",
                table: "Projects",
                column: "ProjectKey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectKey",
                table: "Projects");
        }
    }
}
