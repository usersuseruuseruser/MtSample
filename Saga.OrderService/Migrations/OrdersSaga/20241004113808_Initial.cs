using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saga.OrderService.Migrations.OrdersSaga
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Sagas");

            migrationBuilder.CreateTable(
                name: "OrderStates",
                schema: "Sagas",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    WarehouseId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    BankPaymentCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    OrderCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OrderCreationCompensatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StockReservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StockReservationCompensatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentCompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentCompensatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationCompensatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStates", x => x.CorrelationId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderStates",
                schema: "Sagas");
        }
    }
}
