using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitCoachPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DbSeedWithIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "ExerciseName", "GifUrl" },
                values: new object[,]
                {
                    { new Guid("21000000-0000-0000-0000-000000000001"), "Push-up", "https://example.com/gifs/pushup.gif" },
                    { new Guid("21000000-0000-0000-0000-000000000002"), "Squat", "https://example.com/gifs/squat.gif" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "FirstName", "LastName", "Role" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Super", "Admin", 0 });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CoachId", "CreatedAt", "FirstName", "LastName", "Role", "SubscriptionExpiresAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333334"), null, new DateTime(2025, 9, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Alice", "NoTrainer", 2, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "FirstName", "LastName", "Role" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Jane", "Smith", 1 },
                    { new Guid("55555555-5555-5555-5555-555555555556"), new DateTime(2025, 9, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Mike", "SoloCoach", 1 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DomainUserId", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 0, "00000000-0000-0000-0000-000000000002", new Guid("55555555-5555-5555-5555-555555555555"), "jane@example.com", true, false, null, "JANE@EXAMPLE.COM", "JANE.SMITH", "AQAAAAEAACcQAAAAEDummyCoachHash123==", null, false, "00000000-0000-0000-0000-000000000012", false, "jane.smith" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 0, "00000000-0000-0000-0000-000000000003", new Guid("11111111-1111-1111-1111-111111111111"), "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "SUPER.ADMIN", "AQAAAAEAACcQAAAAEDummyAdminHash123==", null, false, "00000000-0000-0000-0000-000000000013", false, "super.admin" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), 0, "00000000-0000-0000-0000-000000000004", new Guid("33333333-3333-3333-3333-333333333334"), "alice@example.com", true, false, null, "ALICE@EXAMPLE.COM", "ALICE.NOTRAINER", "AQAAAAEAACcQAAAAEDummyClientNoTrainerHash==", null, false, "00000000-0000-0000-0000-000000000014", false, "alice.notrainer" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 0, "00000000-0000-0000-0000-000000000005", new Guid("55555555-5555-5555-5555-555555555556"), "mike@example.com", true, false, null, "MIKE@EXAMPLE.COM", "MIKE.SOLO", "AQAAAAEAACcQAAAAEDummyCoachSoloHash==", null, false, "00000000-0000-0000-0000-000000000015", false, "mike.solo" }
                });

            migrationBuilder.InsertData(
                table: "TemplateWorkoutPlans",
                columns: new[] { "Id", "CoachId", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("31000000-0000-0000-0000-000000000001"), new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 9, 5, 0, 0, 0, 0, DateTimeKind.Utc), null },
                    { new Guid("31000000-0000-0000-0000-000000000002"), new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 9, 6, 0, 0, 0, 0, DateTimeKind.Utc), null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CoachId", "CreatedAt", "FirstName", "LastName", "Role", "SubscriptionExpiresAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "John", "Doe", 2, null });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DomainUserId", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 0, "00000000-0000-0000-0000-000000000001", new Guid("33333333-3333-3333-3333-333333333333"), "john@example.com", true, false, null, "JOHN@EXAMPLE.COM", "JOHN.DOE", "AQAAAAEAACcQAAAAEDummyClientHash123==", null, false, "00000000-0000-0000-0000-000000000011", false, "john.doe" });

            migrationBuilder.InsertData(
                table: "TemplateWorkoutItems",
                columns: new[] { "Id", "Description", "ExerciseId", "TemplateWorkoutPlanId" },
                values: new object[,]
                {
                    { new Guid("32000000-0000-0000-0000-000000000001"), "3 sets of 12 push-ups", new Guid("21000000-0000-0000-0000-000000000001"), new Guid("31000000-0000-0000-0000-000000000001") },
                    { new Guid("32000000-0000-0000-0000-000000000002"), "4 sets of 10 squats", new Guid("21000000-0000-0000-0000-000000000002"), new Guid("31000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "WorkoutPlans",
                columns: new[] { "Id", "ClientId", "DateOfDoing" },
                values: new object[] { new Guid("41000000-0000-0000-0000-000000000001"), new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 9, 10, 8, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "WorkoutItems",
                columns: new[] { "Id", "Description", "ExerciseId", "WorkoutPlanId" },
                values: new object[] { new Guid("51000000-0000-0000-0000-000000000001"), "Morning push-up routine - 3x12", new Guid("21000000-0000-0000-0000-000000000001"), new Guid("41000000-0000-0000-0000-000000000001") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("32000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("32000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("51000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("21000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("21000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("31000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333334"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555556"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("41000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));
        }
    }
}
