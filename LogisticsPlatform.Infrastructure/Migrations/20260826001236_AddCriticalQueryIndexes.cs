using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCriticalQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplyCatalogItems_SortOrder",
                table: "SupplyCatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_Orders_HubId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "OrderWarehousePhotos");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "OrderWarehousePhotos");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "OrderOperationPhotos");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "OrderOperationPhotos");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "SubOrders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "SubOrders",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "OrderWarehousePhotos",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NewStatus",
                table: "OrderTimelineEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousStatus",
                table: "OrderTimelineEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Orders",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "OrderOperationPhotos",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "RegionCode",
                table: "Hubs",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hubs",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Carriers",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCatalogItems_IsActive_SortOrder",
                table: "SupplyCatalogItems",
                columns: new[] { "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderWarehousePhotos_OrderId_IsDeleted",
                table: "OrderWarehousePhotos",
                columns: new[] { "OrderId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderWarehousePhotos_StorageKey",
                table: "OrderWarehousePhotos",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTimelineEntries_OrderId_CreatedAt",
                table: "OrderTimelineEntries",
                columns: new[] { "OrderId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupplies_OrderId_IsDeleted",
                table: "OrderSupplies",
                columns: new[] { "OrderId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AwaitingClientAction",
                table: "Orders",
                column: "AwaitingClientAction");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_HasAlert_CreatedAt",
                table: "Orders",
                columns: new[] { "HasAlert", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_HubId_ScheduledAt",
                table: "Orders",
                columns: new[] { "HubId", "ScheduledAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Number",
                table: "Orders",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_CompletedAt",
                table: "Orders",
                columns: new[] { "Status", "CompletedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_ScheduledAt",
                table: "Orders",
                columns: new[] { "Status", "ScheduledAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Type_Status_HasAlert_ScheduledAt",
                table: "Orders",
                columns: new[] { "Type", "Status", "HasAlert", "ScheduledAt" },
                descending: new[] { false, false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperations_OrderId_IsDeleted",
                table: "OrderOperations",
                columns: new[] { "OrderId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperationPhotos_OperationId_IsDeleted",
                table: "OrderOperationPhotos",
                columns: new[] { "OperationId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperationPhotos_StorageKey",
                table: "OrderOperationPhotos",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderOperationComments_OperationId_CreatedAt",
                table: "OrderOperationComments",
                columns: new[] { "OperationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderComments_OrderId_CreatedAt",
                table: "OrderComments",
                columns: new[] { "OrderId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplyCatalogItems_IsActive_SortOrder",
                table: "SupplyCatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderWarehousePhotos_OrderId_IsDeleted",
                table: "OrderWarehousePhotos");

            migrationBuilder.DropIndex(
                name: "IX_OrderWarehousePhotos_StorageKey",
                table: "OrderWarehousePhotos");

            migrationBuilder.DropIndex(
                name: "IX_OrderTimelineEntries_OrderId_CreatedAt",
                table: "OrderTimelineEntries");

            migrationBuilder.DropIndex(
                name: "IX_OrderSupplies_OrderId_IsDeleted",
                table: "OrderSupplies");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AwaitingClientAction",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_HasAlert_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_HubId_ScheduledAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Number",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_CompletedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_ScheduledAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Type_Status_HasAlert_ScheduledAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderOperations_OrderId_IsDeleted",
                table: "OrderOperations");

            migrationBuilder.DropIndex(
                name: "IX_OrderOperationPhotos_OperationId_IsDeleted",
                table: "OrderOperationPhotos");

            migrationBuilder.DropIndex(
                name: "IX_OrderOperationPhotos_StorageKey",
                table: "OrderOperationPhotos");

            migrationBuilder.DropIndex(
                name: "IX_OrderOperationComments_OperationId_CreatedAt",
                table: "OrderOperationComments");

            migrationBuilder.DropIndex(
                name: "IX_OrderComments_OrderId_CreatedAt",
                table: "OrderComments");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "OrderWarehousePhotos");

            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "OrderTimelineEntries");

            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "OrderTimelineEntries");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "OrderOperationPhotos");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                table: "SubOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "SubOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "OrderWarehousePhotos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "OrderWarehousePhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<byte[]>(
                name: "Content",
                table: "OrderOperationPhotos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "OrderOperationPhotos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RegionCode",
                table: "Hubs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Hubs",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Carriers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_SupplyCatalogItems_SortOrder",
                table: "SupplyCatalogItems",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_HubId",
                table: "Orders",
                column: "HubId");
        }
    }
}
