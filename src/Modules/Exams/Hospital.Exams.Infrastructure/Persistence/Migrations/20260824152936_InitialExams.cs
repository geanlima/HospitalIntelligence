using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Exams.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    requested_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    resulted_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    result = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exams", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exams_patient_id",
                table: "exams",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_exams_requested_at_utc",
                table: "exams",
                column: "requested_at_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exams");
        }
    }
}
