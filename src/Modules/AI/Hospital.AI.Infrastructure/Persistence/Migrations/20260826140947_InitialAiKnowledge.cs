using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Hospital.AI.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAiKnowledge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "ai_knowledge_documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceId = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PatientId = table.Column<Guid>(type: "uuid", nullable: true),
                    Embedding = table.Column<Vector>(type: "vector(32)", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_knowledge_documents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_knowledge_documents_PatientId",
                table: "ai_knowledge_documents",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_knowledge_documents_SourceId",
                table: "ai_knowledge_documents",
                column: "SourceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_knowledge_documents");
        }
    }
}
