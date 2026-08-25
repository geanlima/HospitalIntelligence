using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Alerts.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patient_alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    acknowledged_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    resolved_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_alerts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patient_alerts_created_at_utc",
                table: "patient_alerts",
                column: "created_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_patient_alerts_patient_id",
                table: "patient_alerts",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_alerts_patient_id_status",
                table: "patient_alerts",
                columns: new[] { "patient_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_patient_alerts_status",
                table: "patient_alerts",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_alerts");
        }
    }
}
