using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Timeline.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "timeline_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeline_items", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_timeline_items_occurred_at_utc",
                table: "timeline_items",
                column: "occurred_at_utc");

            migrationBuilder.CreateIndex(
                name: "ix_timeline_items_patient_id",
                table: "timeline_items",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_timeline_items_patient_id_occurred_at_utc",
                table: "timeline_items",
                columns: new[] { "patient_id", "occurred_at_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "timeline_items");
        }
    }
}
