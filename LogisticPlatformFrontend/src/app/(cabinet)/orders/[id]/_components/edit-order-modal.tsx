"use client";

import { useEffect, useState, type ReactNode } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { OrderDetails, OrderStatus } from "@/types/orders";
import { ordersService } from "@/api/services/orders.service";
import { ValidationErrorBanner } from "@/components/freitty/validation-error-banner";
import { getErrorIssues, type ApiValidationIssue } from "@/lib/api/errors";
import { ordersKeys } from "../../_hooks/orders-queries";
import { allowedOrderStatusOptions } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  order: OrderDetails;
  open: boolean;
  onClose: () => void;
};

const HEARTBEAT_MS = 15_000;

export function EditOrderModal({ order, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const [number, setNumber] = useState(order.number);
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
  const [awaitingClientAction, setAwaitingClientAction] = useState(order.awaitingClientAction);
  const [errorIssues, setErrorIssues] = useState<ApiValidationIssue[] | null>(null);

  const isDraftNumber = (value: string) =>
    value.trim().toUpperCase().startsWith("DRAFT-");
  const mustRenameDraftNumber =
    status !== 1 && isDraftNumber(number);

  useEffect(() => {
    if (!open) return;

    const tick = () => {
      void ordersService.heartbeatEditLock(order.id).catch(() => {
        setErrorIssues([
          { message: "Edit lock was lost. Close and try again." },
        ]);
      });
    };

    const id = window.setInterval(tick, HEARTBEAT_MS);

    // Do NOT release in this cleanup — React Strict Mode remounts and would drop the lock.
    return () => {
      window.clearInterval(id);
    };
  }, [open, order.id]);

  useEffect(() => {
    const onPageHide = () => {
      void ordersService.releaseEditLock(order.id).catch(() => undefined);
    };
    window.addEventListener("pagehide", onPageHide);
    return () => window.removeEventListener("pagehide", onPageHide);
  }, [order.id]);

  const closeAndRelease = () => {
    void ordersService.releaseEditLock(order.id).finally(() => onClose());
  };

  const saveMutation = useMutation({
    mutationFn: () =>
      ordersService.update(order.id, {
        number: number.trim() || null,
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
        awaitingClientAction,
      }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(order.id) });
      await queryClient.invalidateQueries({ queryKey: ordersKeys.all });
      await queryClient.invalidateQueries({ queryKey: ["notifications"] });
      closeAndRelease();
    },
    onError: (err) => setErrorIssues(getErrorIssues(err)),
  });

  const field = (label: string, node: ReactNode) => (
    <label style={{ display: "block", marginBottom: 10 }}>
      <div style={{ fontSize: 12, color: "#6B7280", marginBottom: 4 }}>{label}</div>
      {node}
    </label>
  );

  return (
    <DetailModal open={open} title={`Edit ${order.number}`} onClose={closeAndRelease} width={520}>
      <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 8 }}>
        {field(
          "Order #",
          <input
            value={number}
            onChange={(e) => setNumber(e.target.value)}
            style={{
              width: "100%",
              ...(mustRenameDraftNumber
                ? { borderColor: "#DC2626", outlineColor: "#DC2626" }
                : {}),
            }}
          />,
        )}
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
          "Truck #",
          <input value={truckNumber} onChange={(e) => setTruckNumber(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Trailer #",
          <input value={trailerNumber} onChange={(e) => setTrailerNumber(e.target.value)} style={{ width: "100%" }} />,
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
          <input value={declaredQty} onChange={(e) => setDeclaredQty(e.target.value)} style={{ width: "100%" }} />,
        )}
        {field(
          "Actual qty",
          <input value={actualQty} onChange={(e) => setActualQty(e.target.value)} style={{ width: "100%" }} />,
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
          {allowedOrderStatusOptions(order.status).map((o) => (
            <option key={o.value} value={o.value}>
              {o.label}
            </option>
          ))}
        </select>,
      )}
      <label
        style={{
          display: "flex",
          alignItems: "center",
          gap: 8,
          fontSize: 13,
          marginBottom: 12,
          cursor: "pointer",
        }}
      >
        <input
          type="checkbox"
          checked={awaitingClientAction}
          onChange={(e) => setAwaitingClientAction(e.target.checked)}
        />
        Awaiting client action (shows in Notifications)
      </label>
      {field(
        "Warehouse note",
        <textarea
          value={warehouseNote}
          onChange={(e) => setWarehouseNote(e.target.value)}
          rows={3}
          style={{ width: "100%" }}
        />,
      )}
      {mustRenameDraftNumber && (
        <div style={{ color: "#B45309", fontSize: 12, marginBottom: 8 }}>
          Change Order # before leaving Draft — <code>DRAFT-…</code> numbers are not allowed for New /
          In Progress / etc.
        </div>
      )}
      <ValidationErrorBanner issues={errorIssues} title="Could not save order:" />
      <button
        type="button"
        className="btn btn-primary"
        disabled={saveMutation.isPending || mustRenameDraftNumber}
        onClick={() => {
          setErrorIssues(null);
          saveMutation.mutate();
        }}
      >
        {saveMutation.isPending ? "Saving…" : "Save changes"}
      </button>
    </DetailModal>
  );
}
