using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckConstraintForChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_checks_purchase_id",
                table: "checks");

            migrationBuilder.DropIndex(
                name: "IX_checks_refund_id",
                table: "checks");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("9d8c2588-9d95-4a42-8649-c8a7b43c3930"));

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[] { new Guid("0fceec43-f581-4783-875b-24f41f6e9efb"), null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_checks_purchase_id",
                table: "checks",
                column: "purchase_id",
                unique: true,
                filter: "purchase_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_checks_refund_id",
                table: "checks",
                column: "refund_id",
                unique: true,
                filter: "refund_id IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Check_Single_Owner",
                table: "checks",
                sql: "(purchase_id IS NOT NULL AND refund_id IS NULL) OR (purchase_id IS NULL AND refund_id IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_checks_purchase_id",
                table: "checks");

            migrationBuilder.DropIndex(
                name: "IX_checks_refund_id",
                table: "checks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Check_Single_Owner",
                table: "checks");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("0fceec43-f581-4783-875b-24f41f6e9efb"));

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[] { new Guid("9d8c2588-9d95-4a42-8649-c8a7b43c3930"), null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_checks_purchase_id",
                table: "checks",
                column: "purchase_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_checks_refund_id",
                table: "checks",
                column: "refund_id",
                unique: true);
        }
    }
}
