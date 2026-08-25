using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital.Admissions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialAdmissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    admission_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    discharge_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    unit = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bed = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admissions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admissions_patient_id",
                table: "admissions",
                column: "patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admissions");
        }
    }
}
