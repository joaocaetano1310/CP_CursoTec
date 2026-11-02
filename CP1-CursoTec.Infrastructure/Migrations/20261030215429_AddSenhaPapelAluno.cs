using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CP1_CursoTec.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSenhaPapelAluno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Papel",
                table: "Alunos",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "Aluno");

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Alunos",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Papel",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Alunos");
        }
    }
}
