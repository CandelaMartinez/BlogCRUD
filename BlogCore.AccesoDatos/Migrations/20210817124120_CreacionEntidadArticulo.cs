using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogCore.AccesoDatos.Migrations
{
    public partial class CreacionEntidadArticulo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articulo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(nullable: false),
                    Descripcion = table.Column<string>(nullable: false),
                    FechaCreacion = table.Column<string>(nullable: true),
                    UrlImagen = table.Column<string>(nullable: true),
                    CategoriaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Articulo_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articulo_CategoriaId",
                table: "Articulo",
                column: "CategoriaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articulo");
        }
    }
}
