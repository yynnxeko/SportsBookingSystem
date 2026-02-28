using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportsBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TimeSlots",
                columns: new[] { "Id", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, new TimeOnly(7, 0, 0), new TimeOnly(6, 0, 0) },
                    { 2, new TimeOnly(8, 0, 0), new TimeOnly(7, 0, 0) },
                    { 3, new TimeOnly(9, 0, 0), new TimeOnly(8, 0, 0) },
                    { 4, new TimeOnly(10, 0, 0), new TimeOnly(9, 0, 0) },
                    { 5, new TimeOnly(11, 0, 0), new TimeOnly(10, 0, 0) },
                    { 6, new TimeOnly(12, 0, 0), new TimeOnly(11, 0, 0) },
                    { 7, new TimeOnly(13, 0, 0), new TimeOnly(12, 0, 0) },
                    { 8, new TimeOnly(14, 0, 0), new TimeOnly(13, 0, 0) },
                    { 9, new TimeOnly(15, 0, 0), new TimeOnly(14, 0, 0) },
                    { 10, new TimeOnly(16, 0, 0), new TimeOnly(15, 0, 0) },
                    { 11, new TimeOnly(17, 0, 0), new TimeOnly(16, 0, 0) },
                    { 12, new TimeOnly(18, 0, 0), new TimeOnly(17, 0, 0) },
                    { 13, new TimeOnly(19, 0, 0), new TimeOnly(18, 0, 0) },
                    { 14, new TimeOnly(20, 0, 0), new TimeOnly(19, 0, 0) },
                    { 15, new TimeOnly(21, 0, 0), new TimeOnly(20, 0, 0) },
                    { 16, new TimeOnly(22, 0, 0), new TimeOnly(21, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TimeSlots",
                keyColumn: "Id",
                keyValue: 16);
        }
    }
}
