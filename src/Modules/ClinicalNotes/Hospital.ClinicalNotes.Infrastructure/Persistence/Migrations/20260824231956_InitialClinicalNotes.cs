using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.ClinicalNotes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialClinicalNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clinical_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    note_type = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinical_notes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_clinical_notes_created_at_utc",
                table: "clinical_notes",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_clinical_notes_patient_id",
                table: "clinical_notes",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clinical_notes");
        }
    }
}
