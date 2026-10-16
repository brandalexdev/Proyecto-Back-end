using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_app_001.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNuevosModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstadoEventoId",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EstadoEvento",
                columns: table => new
                {
                    EstadoEventoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoEvento", x => x.EstadoEventoId);
                });

            migrationBuilder.CreateTable(
                name: "Recordatorio",
                columns: table => new
                {
                    recordatorioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaRecordatorio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Enviado = table.Column<bool>(type: "bit", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recordatorio", x => x.recordatorioId);
                    table.ForeignKey(
                        name: "FK_Recordatorio_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_EstadoEventoId",
                table: "Eventos",
                column: "EstadoEventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recordatorio_EventoId",
                table: "Recordatorio",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_EstadoEvento_EstadoEventoId",
                table: "Eventos",
                column: "EstadoEventoId",
                principalTable: "EstadoEvento",
                principalColumn: "EstadoEventoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_EstadoEvento_EstadoEventoId",
                table: "Eventos");

            migrationBuilder.DropTable(
                name: "EstadoEvento");

            migrationBuilder.DropTable(
                name: "Recordatorio");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_EstadoEventoId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EstadoEventoId",
                table: "Eventos");
        }
    }
}
