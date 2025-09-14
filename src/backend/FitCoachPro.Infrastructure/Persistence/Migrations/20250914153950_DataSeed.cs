using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitCoachPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "ExerciseName", "GifUrl" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "Push-ups", "https://example.com/gifs/pushups.gif" },
                    { new Guid("bbbbbbbb-2222-2222-2222-222222222222"), "Squats", "https://example.com/gifs/squats.gif" },
                    { new Guid("cccccccc-3333-3333-3333-333333333333"), "Plank", "https://example.com/gifs/plank.gif" },
                    { new Guid("dddddddd-4444-4444-4444-444444444444"), "Lunges", "https://example.com/gifs/lunges.gif" },
                    { new Guid("eeeeeeee-5555-5555-5555-555555555555"), "Pull-ups", "https://example.com/gifs/pullups.gif" },
                    { new Guid("ffffffff-6666-6666-6666-666666666666"), "Deadlifts", "https://example.com/gifs/deadlifts.gif" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfRegistration", "FirstName", "LastName", "PasswordHash", "Role", "TelephoneNumber" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Super", "Admin", "adminhash1", 0, "+000000000" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Coach", "One", "coachhash1", 1, "+111111111" },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Coach", "Two", "coachhash2", 1, "+222222222" }
                });

            migrationBuilder.InsertData(
                table: "TemplateWorkoutItems",
                columns: new[] { "Id", "CoachId", "CreatedAt", "Description", "ExerciseId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "3 sets of 15 reps", new Guid("aaaaaaaa-1111-1111-1111-111111111111"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("11111111-0000-0000-0000-000000000002"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "3 sets of 20 reps", new Guid("bbbbbbbb-2222-2222-2222-222222222222"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000003"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hold for 60 seconds", new Guid("cccccccc-3333-3333-3333-333333333333"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { new Guid("22222222-0000-0000-0000-000000000004"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "3 sets of 10 reps per leg", new Guid("dddddddd-4444-4444-4444-444444444444"), new DateTime(2025, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CoachId", "DateOfExpiredSubscription", "DateOfRegistration", "FirstName", "LastName", "PasswordHash", "Role", "TelephoneNumber" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, new DateTime(2025, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), "John", "Doe", "hash1", 2, "+123456789" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, new DateTime(2025, 7, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Anna", "Smith", "hash2", 2, "+987654321" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), null, new DateTime(2025, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "Mike", "Johnson", "hash3", 2, "+192837465" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutPlans",
                columns: new[] { "Id", "ClientId", "DateOfDoing" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 7, 16, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 7, 17, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 7, 18, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "WorkoutItems",
                columns: new[] { "Id", "Description", "ExerciseId", "WorkoutPlanId" },
                values: new object[,]
                {
                    { new Guid("aaaa1111-0000-0000-0000-000000000001"), "3 sets of 15 reps", new Guid("aaaaaaaa-1111-1111-1111-111111111111"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("aaaa1111-0000-0000-0000-000000000002"), "3 sets of 20 reps", new Guid("bbbbbbbb-2222-2222-2222-222222222222"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("bbbb2222-0000-0000-0000-000000000001"), "Hold for 60 seconds", new Guid("cccccccc-3333-3333-3333-333333333333"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("bbbb2222-0000-0000-0000-000000000002"), "3 sets of 10 reps per leg", new Guid("dddddddd-4444-4444-4444-444444444444"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("cccc3333-0000-0000-0000-000000000001"), "3 sets to failure", new Guid("eeeeeeee-5555-5555-5555-555555555555"), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("cccc3333-0000-0000-0000-000000000002"), "3 sets of 8 reps", new Guid("ffffffff-6666-6666-6666-666666666666"), new Guid("33333333-3333-3333-3333-333333333333") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "TemplateWorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("bbbb2222-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("bbbb2222-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("cccc3333-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorkoutItems",
                keyColumn: "Id",
                keyValue: new Guid("cccc3333-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("ffffffff-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "WorkoutPlans",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
        }
    }
}
