using Microsoft.EntityFrameworkCore.Migrations;

namespace YouTrack.Management.RelationalDal.Migrations
{
    public partial class ProjectName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Assignees",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Assignees");
        }
    }
}
