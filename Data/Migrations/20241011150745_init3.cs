using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeaWork.Migrations
{
    /// <inheritdoc />
    public partial class init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "OwnDesignConcepts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OwnDesignConcepts_ProjectId",
                table: "OwnDesignConcepts",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnDesignConcepts_Projects_ProjectId",
                table: "OwnDesignConcepts",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnDesignConcepts_Projects_ProjectId",
                table: "OwnDesignConcepts");

            migrationBuilder.DropIndex(
                name: "IX_OwnDesignConcepts_ProjectId",
                table: "OwnDesignConcepts");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "OwnDesignConcepts");
        }
    }
}
