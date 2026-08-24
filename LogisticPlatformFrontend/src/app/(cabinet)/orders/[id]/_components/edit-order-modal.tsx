"use client";

import { useState, type ReactNode } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { OrderDetails, OrderStatus } from "@/types/orders";
import { ordersService } from "@/api/services/orders.service";
import { ordersKeys } from "../../_hooks/orders-queries";
import { ORDER_STATUS_OPTIONS } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  order: OrderDetails;
  open: boolean;
  onClose: () => void;
};

export function EditOrderModal({ order, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const [customerName, setCustomerName] = useState(order.customerName ?? "");
  const [primaryReference, setPrimaryReference] = useState(order.primaryReference ?? "");
  const [phone, setPhone] = useState(order.phone ?? "");
  const [truckNumber, setTruckNumber] = useState(order.truckNumber ?? "");
  const [trailerNumber, setTrailerNumber] = useState(order.trailerNumber ?? "");
  const [trailerType, setTrailerType] = useState(order.trailerType ?? "");
  const [dockCode, setDockCode] = useState(order.assignedDock.dockCode ?? "");
  const [dockBay, setDockBay] = useState(order.assignedDock.dockBay ?? "");
  const [declaredQty, setDeclaredQty] = useState(String(order.expected.quantity ?? ""));
  const [actualQty, setActualQty] = useState(String(order.actual.quantity ?? ""));
  const [warehouseNote, setWarehouseNote] = useState(order.warehouseNote.text ?? "");
  const [stockStatusLabel, setStockStatusLabel] = useState(order.stockStatusLabel ?? "");
  const [loadingStatusLabel, setLoadingStatusLabel] = useState(order.loadingStatusLabel ?? "");
  const [status, setStatus] = useState<OrderStatus>(order.status);
  const [error, setError] = useState<string | null>(null);

  const saveMutation = useMutation({
    mutationFn: () =>
      ordersService.update(order.id, {
        customerName: customerName || null,
        primaryReference: primaryReference || null,
        phone: phone || null,
        truckNumber: truckNumber || null,
        trailerNumber: trailerNumber || null,
        trailerType: trailerType || null,
        dockCode: dockCode || null,
        dockBay: dockBay || null,
        declaredQty: declaredQty === "" ? null : Number(declaredQty),
        actualQty: actualQty === "" ? null : Number(actualQty),
        warehouseNote: warehouseNote || null,
        stockStatusLabel: stockStatusLabel || null,
        loadingStatusLabel: loadingStatusLabel || null,
        status,
        quantityUnitLabel: order.expected.unitLabel,
        dockStatusLabel: order.assignedDock.statusLabel,
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(order.id) });
      await queryClient.invalidateQueries({ queryKey: ordersKeys.all });
      onClose();
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Save failed"),
  });

  const field = (label: string, node: ReactNode) => (
    <label style={{ display: "block", marginBottom: 10 }}>
      <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>{label}</div>
      {node}
    </label>
  );

  return (
    <DetailModal open={open} title={`Edit ${order.number}`} onClose={onClose} width={520}>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 8 }}>
        {field(
          "Customer",
          <input value={customerName} onChange={(e) => setCustomerName(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Ref N",
          <input
            value={primaryReference}
            onChange={(e) => setPrimaryReference(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
        {field(
          "Phone",
          <input value={phone} onChange={(e) => setPhone(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Trailer type",
          <input value={trailerType} onChange={(e) => setTrailerType(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Truck",
          <input value={truckNumber} onChange={(e) => setTruckNumber(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Trailer",
          <input
            value={trailerNumber}
            onChange={(e) => setTrailerNumber(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
        {field(
          "Dock code",
          <input value={dockCode} onChange={(e) => setDockCode(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Dock bay",
          <input value={dockBay} onChange={(e) => setDockBay(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Declared qty",
          <input
            type="number"
            value={declaredQty}
            onChange={(e) => setDeclaredQty(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
        {field(
          "Actual qty",
          <input
            type="number"
            value={actualQty}
            onChange={(e) => setActualQty(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
        {field(
          "Stock status",
          <input
            value={stockStatusLabel}
            onChange={(e) => setStockStatusLabel(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
        {field(
          "Loading status",
          <input
            value={loadingStatusLabel}
            onChange={(e) => setLoadingStatusLabel(e.target.value)}
            style={{ width: "100%" }}
          />,
        )}
      </div>
      {field(
        "Status",
        <select
          value={status}
          onChange={(e) => setStatus(Number(e.target.value) as OrderStatus)}
          style={{ width: "100%" }}
        >
          {ORDER_STATUS_OPTIONS.map((o) => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>,
      )}
      {field(
        "Warehouse note",
        <textarea
          value={warehouseNote}
          onChange={(e) => setWarehouseNote(e.target.value)}
          rows={3}
          style={{ width: "100%" }}
        />,
      )}
      {error && <div style={{ color: "#DC2626", fontSize: 12, marginBottom: 8 }}>{error}</div>}
      <button
        type="button"
        className="btn btn-primary"
        disabled={saveMutation.isPending}
        onClick={() => saveMutation.mutate()}
      >
        {saveMutation.isPending ? "Saving…" : "Save changes"}
      </button>
    </DetailModal>
  );
}
