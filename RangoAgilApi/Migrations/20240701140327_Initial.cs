using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RangoAgilApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ingredientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rangos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rangos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredienteRango",
                columns: table => new
                {
                    IngredientesId = table.Column<int>(type: "INTEGER", nullable: false),
                    RangosId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredienteRango", x => new { x.IngredientesId, x.RangosId });
                    table.ForeignKey(
                        name: "FK_IngredienteRango_Ingredientes_IngredientesId",
                        column: x => x.IngredientesId,
                        principalTable: "Ingredientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredienteRango_Rangos_RangosId",
                        column: x => x.RangosId,
                        principalTable: "Rangos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Ingredientes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pão" },
                    { 2, "Carne de Vaca" },
                    { 3, "Cebola" },
                    { 4, "Maionese" },
                    { 5, "Alface" }
                });

            migrationBuilder.InsertData(
                table: "Rangos",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Subway" },
                    { 2, "Pizza" },
                    { 3, "Sopa" },
                    { 4, "Espaguete" },
                    { 5, "Lasanha" }
                });

            migrationBuilder.InsertData(
                table: "IngredienteRango",
                columns: new[] { "IngredientesId", "RangosId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 3 },
                    { 3, 2 },
                    { 3, 4 },
                    { 5, 3 },
                    { 5, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredienteRango_RangosId",
                table: "IngredienteRango",
                column: "RangosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredienteRango");

            migrationBuilder.DropTable(
                name: "Ingredientes");

            migrationBuilder.DropTable(
                name: "Rangos");
        }
    }
}
