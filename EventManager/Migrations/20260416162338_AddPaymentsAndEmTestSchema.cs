using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EventManager.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsAndEmTestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "em_test");

            migrationBuilder.RenameTable(
                name: "Tickets",
                newName: "Tickets",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "RegistrationStatuses",
                newName: "RegistrationStatuses",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "Registrations",
                newName: "Registrations",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "EventStatuses",
                newName: "EventStatuses",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "Events",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "EventCategories",
                newName: "EventCategories",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "EmailLogs",
                newName: "EmailLogs",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "em_test");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "em_test");

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "em_test",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationId = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CardholderName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CardLast4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    CardBrand = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Currency = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalSchema: "em_test",
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_RegistrationId",
                schema: "em_test",
                table: "Payments",
                column: "RegistrationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionReference",
                schema: "em_test",
                table: "Payments",
                column: "TransactionReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments",
                schema: "em_test");

            migrationBuilder.RenameTable(
                name: "Tickets",
                schema: "em_test",
                newName: "Tickets");

            migrationBuilder.RenameTable(
                name: "RegistrationStatuses",
                schema: "em_test",
                newName: "RegistrationStatuses");

            migrationBuilder.RenameTable(
                name: "Registrations",
                schema: "em_test",
                newName: "Registrations");

            migrationBuilder.RenameTable(
                name: "EventStatuses",
                schema: "em_test",
                newName: "EventStatuses");

            migrationBuilder.RenameTable(
                name: "Events",
                schema: "em_test",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "EventCategories",
                schema: "em_test",
                newName: "EventCategories");

            migrationBuilder.RenameTable(
                name: "EmailLogs",
                schema: "em_test",
                newName: "EmailLogs");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "em_test",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "em_test",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "em_test",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "em_test",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "em_test",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "em_test",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "em_test",
                newName: "AspNetRoleClaims");
        }
    }
}
