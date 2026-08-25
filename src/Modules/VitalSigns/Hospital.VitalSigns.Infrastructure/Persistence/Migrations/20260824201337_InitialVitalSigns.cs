using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.VitalSigns.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialVitalSigns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vital_signs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    measured_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    temperature = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    heart_rate = table.Column<int>(type: "integer", nullable: true),
                    respiratory_rate = table.Column<int>(type: "integer", nullable: true),
                    systolic_blood_pressure = table.Column<int>(type: "integer", nullable: true),
                    diastolic_blood_pressure = table.Column<int>(type: "integer", nullable: true),
                    oxygen_saturation = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vital_signs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vital_signs_measured_at_utc",
                table: "vital_signs",
                column: "measured_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_vital_signs_patient_id",
                table: "vital_signs",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vital_signs");
        }
    }
}
