using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Prescriptions.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialPrescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "prescriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    prescribed_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescriptions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_prescriptions_patient_id",
                table: "prescriptions",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_prescriptions_prescribed_at_utc",
                table: "prescriptions",
                column: "prescribed_at_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "prescriptions");
        }
    }
}
