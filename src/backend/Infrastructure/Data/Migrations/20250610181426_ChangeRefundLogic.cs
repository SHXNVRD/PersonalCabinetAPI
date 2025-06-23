using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRefundLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refund_purchase_id",
                table: "refunds");

            migrationBuilder.DropIndex(
                name: "IX_refunds_purchase_id",
                table: "refunds");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("4af78223-f66b-457c-b099-7d7326f64544"));

            migrationBuilder.RenameColumn(
                name: "purchase_id",
                table: "refunds",
                newName: "purchase_item_id");

            migrationBuilder.AddColumn<double>(
                name: "quantity",
                table: "refunds",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[] { new Guid("9d8c2588-9d95-4a42-8649-c8a7b43c3930"), null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_refunds_purchase_item_id",
                table: "refunds",
                column: "purchase_item_id");

            migrationBuilder.AddForeignKey(
                name: "FK_refund_purchase_item_id",
                table: "refunds",
                column: "purchase_item_id",
                principalTable: "purchase_items",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refund_purchase_item_id",
                table: "refunds");

            migrationBuilder.DropIndex(
                name: "IX_refunds_purchase_item_id",
                table: "refunds");

            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: new Guid("9d8c2588-9d95-4a42-8649-c8a7b43c3930"));

            migrationBuilder.DropColumn(
                name: "quantity",
                table: "refunds");

            migrationBuilder.RenameColumn(
                name: "purchase_item_id",
                table: "refunds",
                newName: "purchase_id");

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "concurrency_stamp", "name", "normalized_name" },
                values: new object[] { new Guid("4af78223-f66b-457c-b099-7d7326f64544"), null, "User", "USER" });

            migrationBuilder.CreateIndex(
                name: "IX_refunds_purchase_id",
                table: "refunds",
                column: "purchase_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_refund_purchase_id",
                table: "refunds",
                column: "purchase_id",
                principalTable: "purchases",
                principalColumn: "id");
        }
    }
}
