using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppointmentBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDataMigrationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataMigrationHistory",
                columns: table => new
                {
                    ScriptName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataMigrationHistory", x => x.ScriptName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataMigrationHistory");
        }
    }
}
