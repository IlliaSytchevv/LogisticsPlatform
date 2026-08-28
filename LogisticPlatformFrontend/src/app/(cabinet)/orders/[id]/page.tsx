"use client";

import Link from "next/link";
import { use, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { ApiError } from "@/types/auth";
import { mediaUrl, ordersService } from "@/api/services/orders.service";
import { paymentsService } from "@/api/services/payments.service";
import { useSession } from "@/hooks/use-session";
import {
  orderCommentsOptions,
  orderDetailOptions,
} from "../_hooks/orders-queries";
import { CommentsPanel } from "./_components/comments-panel";
import { EditOrderModal } from "./_components/edit-order-modal";
import { OperationsTable } from "./_components/operations-table";
import { SuppliesTable } from "./_components/supplies-table";
import { TimelinePanel } from "./_components/timeline-panel";
import { WarehousePhotosPanel } from "./_components/warehouse-photos-panel";
import {
  deltaLabel,
  formatDetailDate,
  formatDetailDateTime,
  initials,
} from "./_lib/format";

const STATUS_FLOW: { value: number; label: string }[] = [
  { value: 1, label: "Draft" },
  { value: 2, label: "New" },
  { value: 3, label: "In Progress" },
  { value: 5, label: "Completed" },
  { value: 6, label: "Closed" },
];

export default function OrderDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const { canWrite, loading: sessionLoading } = useSession();
  const [commentsOpen, setCommentsOpen] = useState(false);
  const [photosOpen, setPhotosOpen] = useState(false);
  const [timelineOpen, setTimelineOpen] = useState(false);
  const [editOpen, setEditOpen] = useState(false);
  const [docBusy, setDocBusy] = useState<"bol" | "qr" | null>(null);
  const [docError, setDocError] = useState<string | null>(null);
  const [payBusy, setPayBusy] = useState(false);
  const [editBusy, setEditBusy] = useState(false);

  const {
    data: order,
    isLoading,
    isError,
    error,
  } = useQuery(orderDetailOptions(id));

  const { data: comments = [] } = useQuery({
    ...orderCommentsOptions(id),
    enabled: Boolean(id),
  });

  if (isLoading) {
    return <div style={{ padding: 24, color: "#6B7280" }}>Loading order…</div>;
  }

  if (isError || !order) {
    const status = error instanceof ApiError ? error.status : undefined;
    return (
      <div style={{ padding: 24 }}>
        <div style={{ color: "#DC2626", fontWeight: 600, marginBottom: 8 }}>
          {status === 404 ? "Order not found" : "Failed to load order"}
        </div>
        <div style={{ color: "#6B7280", fontSize: 13, marginBottom: 12 }}>
          {error instanceof Error ? error.message : "Unknown error"}
        </div>
        <Link href="/orders" className="btn btn-secondary">
          ← Back to orders
        </Link>
      </div>
    );
  }

  const dock = order.assignedDock;
  const photos = order.warehouseNote.photos ?? [];
  const hubLabel = order.hubRegionCode
    ? `${order.hubName} (${order.hubRegionCode})`
    : order.hubName;
  const qtyDelta = order.qtyDelta;
  const hasQtyDelta = qtyDelta !== 0;

  const downloadBol = async () => {
    setDocError(null);
    setDocBusy("bol");
    try {
      await ordersService.downloadBolPdf(order.id);
    } catch (err) {
      setDocError(err instanceof Error ? err.message : "BOL download failed");
    } finally {
      setDocBusy(null);
    }
  };

  const downloadQr = async () => {
    setDocError(null);
    setDocBusy("qr");
    try {
      await ordersService.downloadQr(order.id);
    } catch (err) {
      setDocError(err instanceof Error ? err.message : "QR download failed");
    } finally {
      setDocBusy(null);
    }
  };

  const conflictMessage = (err: unknown, fallback: string) => {
    if (!(err instanceof ApiError) || err.status !== 409) return null;
    const body = err.body;
    if (Array.isArray(body) && typeof body[0] === "string" && body[0]) return body[0];
    if (typeof body === "string" && body) return body;
    return fallback;
  };

  const startCheckout = async () => {
    setDocError(null);
    setPayBusy(true);
    try {
      const { checkoutUrl } = await paymentsService.createCheckout(order.id);
      window.location.assign(checkoutUrl);
    } catch (err) {
      setDocError(
        conflictMessage(err, "Try again in a moment.") ??
          (err instanceof Error ? err.message : "Checkout failed"),
      );
      setPayBusy(false);
    }
  };

  const openEdit = async () => {
    setDocError(null);
    setEditBusy(true);
    try {
      await ordersService.acquireEditLock(order.id);
      setEditOpen(true);
    } catch (err) {
      setDocError(
        conflictMessage(err, "Order is being edited in another tab or device.") ??
          (err instanceof Error ? err.message : "Could not open editor"),
      );
    } finally {
      setEditBusy(false);
    }
  };

  const isDraftOrClosed = order.status === 1 || order.status === 6;
  const payDisabled =
    sessionLoading || !canWrite || payBusy || order.isPaid || isDraftOrClosed;
  const payTitle = !canWrite
    ? "Payment requires Admin or Dispatcher."
    : order.isPaid
      ? "Already paid"
      : isDraftOrClosed
        ? "You cannot pay for an order in Draft or Closed status."
        : "Pay supplies";
  const payGreyStyle = payDisabled && !payBusy
    ? {
        color: "#9CA3AF",
        borderColor: "#D1D5DB",
        background: "#F3F4F6",
        cursor: "not-allowed" as const,
      }
    : {};

  return (
    <>
      <div className="fc-crumbs">
        <Link href="/orders">Orders</Link> <span>›</span> {order.number}
      </div>

      <div className="xd-hub-header">
        <div className="xd-hub-logo">F</div>
        <div style={{ fontSize: 18, fontWeight: 700, color: "#1F2A3A" }}>
          {order.type === 1 ? "Cross-Dock management" : "Consolidation management"}
        </div>
      </div>

      <div className="xd-order-row">
        <Link href="/orders" className="btn btn-secondary" style={{ padding: "6px 10px", fontSize: 14 }}>
          ←
        </Link>
        <span style={{ color: "#2563EB", fontSize: 20 }}>📦</span>
        <span style={{ fontSize: 13, color: "#6B7280" }}>Order#:</span>
        <strong style={{ fontSize: 15, color: "#1F2A3A" }}>{order.number}</strong>
        <span style={{ color: "#16A34A", fontSize: 16, fontWeight: 700 }}>$</span>
        <span style={{ color: "#6B7280", fontSize: 16 }}>🚚</span>
        <span className="badge" style={{ background: "#DBEAFE", color: "#1E40AF" }}>
          {order.typeLabel}
        </span>
        {order.stockStatusLabel && (
          <span className="badge" style={{ background: "#D1FAE5", color: "#065F46" }}>
            ● {order.stockStatusLabel}
          </span>
        )}
        {order.loadingStatusLabel && (
          <span className="badge badge-prog">{order.loadingStatusLabel}</span>
        )}
        {order.primaryReference && (
          <span className="ref-n-inline">
            <span className="lbl">Ref N:</span>
            <span className="val">{order.primaryReference}</span>
          </span>
        )}
        {order.assignedToUserName && (
          <span className="xd-by-chip">
            <span style={{ color: "#9CA3AF", fontWeight: 500 }}>by</span>
            <span
              style={{
                width: 18,
                height: 18,
                borderRadius: "50%",
                background: "#0EA5E9",
                color: "#fff",
                display: "inline-flex",
                alignItems: "center",
                justifyContent: "center",
                fontSize: 9,
                fontWeight: 800,
              }}
            >
              {initials(order.assignedToUserName)}
            </span>
            {order.assignedToUserName}
          </span>
        )}
        {order.hasAlert && (
          <span className="badge" style={{ background: "#FEF3C7", color: "#92400E" }}>
            ⚠ {order.alertReason || "Alert"}
          </span>
        )}
      </div>

      {docError && (
        <div
          style={{
            margin: "0 0 10px",
            padding: "8px 12px",
            borderRadius: 6,
            background: "#FEF2F2",
            border: "1px solid #FECACA",
            color: "#B91C1C",
            fontSize: 13,
            fontWeight: 600,
          }}
        >
          {docError}
        </div>
      )}

      <div className="xd-actions no-print">
        <button
          type="button"
          className="btn btn-secondary"
          style={{ padding: "7px 12px", fontSize: 12 }}
          onClick={() => setCommentsOpen(true)}
        >
          💬 <span style={{ color: "#9CA3AF" }}>{comments.length}</span>
        </button>
        <button
          type="button"
          className="btn btn-secondary"
          style={{ padding: "7px 12px", fontSize: 12 }}
          onClick={() => setPhotosOpen(true)}
        >
          📷 <span style={{ color: "#9CA3AF" }}>{photos.length}</span>
        </button>
        {!sessionLoading && canWrite ? (
          <button
            type="button"
            className="btn btn-secondary"
            style={{
              padding: "7px 12px",
              fontSize: 12,
              ...payGreyStyle,
            }}
            title={payTitle}
            disabled={payDisabled}
            onClick={startCheckout}
          >
            {payBusy ? "…" : "$"}
          </button>
        ) : null}
        <button
          type="button"
          className="btn btn-secondary"
          style={{ padding: "7px 12px", fontSize: 12 }}
          title="Print"
          onClick={() => window.print()}
        >
          🖨
        </button>
        <button
          type="button"
          className="btn btn-secondary"
          style={{ padding: "7px 12px", fontSize: 12 }}
          title="Supplies"
          onClick={() => document.getElementById("order-supplies")?.scrollIntoView({ behavior: "smooth" })}
        >
          📦
        </button>
        <button
          type="button"
          className="btn btn-secondary"
          style={{ padding: "7px 12px", fontSize: 12 }}
          onClick={() => setTimelineOpen(true)}
        >
          🕘
        </button>
        {order.type === 1 && (
          <span
            title={order.isPaid ? "Supplies paid" : "Supplies not paid"}
            style={{
              display: "inline-flex",
              alignItems: "center",
              justifyContent: "center",
              minWidth: 56,
              height: 30,
              padding: "0 8px",
              borderRadius: 6,
              border: `1px solid ${order.isPaid ? "#86EFAC" : "#E5E7EB"}`,
              background: order.isPaid ? "#DCFCE7" : "#F3F4F6",
              color: order.isPaid ? "#166534" : "#6B7280",
              fontSize: 11,
              fontWeight: 700,
              lineHeight: 1,
            }}
          >
            {order.isPaid ? "Paid" : "Unpaid"}
          </span>
        )}
        <div style={{ marginLeft: "auto", display: "flex", gap: 8, alignItems: "center" }}>
          <button
            type="button"
            className="btn btn-secondary"
            style={{ fontSize: 12, padding: "7px 12px" }}
            disabled={docBusy !== null}
            onClick={downloadQr}
          >
            📷 {docBusy === "qr" ? "…" : "Share QR"}
          </button>
          <button
            type="button"
            className="btn btn-primary"
            style={{ fontSize: 12, padding: "7px 12px" }}
            disabled={docBusy !== null}
            onClick={downloadBol}
          >
            📥 {docBusy === "bol" ? "…" : "BOL PDF"}
          </button>
          {!sessionLoading && canWrite ? (
            <button
              type="button"
              className="btn btn-primary"
              style={{ padding: "7px 14px", fontSize: 12 }}
              disabled={editBusy}
              onClick={openEdit}
            >
              {editBusy ? "…" : "✏️ Edit"}
            </button>
          ) : null}
        </div>
      </div>

      <div className="xd-meta">
        <div className="xd-meta-col">
          <div className="k">Customer</div>
          <div style={{ fontWeight: 600 }}>{order.customerName || "—"}</div>
          <div className="k">Hub</div>
          <div>{hubLabel}</div>
          <div className="k">Services</div>
          <div>
            {(order.services?.length ?? 0) === 0 ? (
              "—"
            ) : (
              order.services.map((s) => (
                <span
                  key={s}
                  className="badge"
                  style={{ background: "#DBEAFE", color: "#1E40AF", marginRight: 4 }}
                >
                  {s}
                </span>
              ))
            )}
          </div>
          <div className="k">Date</div>
          <div style={{ color: "#DC2626", fontWeight: 600 }}>
            {formatDetailDate(order.scheduledAt)}
          </div>
          <div className="k">Declared q-ty</div>
          <div style={{ fontWeight: 600 }}>
            {order.expected.quantity ?? "—"}{" "}
            {order.expected.unitLabel && (
              <span style={{ fontSize: 11, color: "#6B7280", fontWeight: 500 }}>
                {order.expected.unitLabel}
              </span>
            )}
          </div>
          <div className="k">Actual q-ty</div>
          <div style={{ fontWeight: 600, color: hasQtyDelta ? "#78350F" : undefined }}>
            {order.actual.quantity ?? "—"}{" "}
            {hasQtyDelta && (
              <span className="badge" style={{ background: "#FB923C", color: "#fff", fontSize: 10 }}>
                Δ {deltaLabel(qtyDelta)}
              </span>
            )}
          </div>
          <div className="k">Trailer type</div>
          <div>
            {order.trailerType ? (
              <span className="badge" style={{ background: "#F3F4F6", color: "#374151" }}>
                {order.trailerType}
              </span>
            ) : (
              "—"
            )}
          </div>
        </div>
        <div className="xd-meta-col">
          <div className="k">Carrier</div>
          <div style={{ fontWeight: 600 }}>{order.carrierName || "—"}</div>
          <div className="k">Phone</div>
          <div>{order.phone || "—"}</div>
          <div className="k">Truck / trailer</div>
          <div>
            {order.truckNumber || "—"} / {order.trailerNumber || "—"}
          </div>
          <div className="k">Dock</div>
          <div>
            {dock.dockCode ? (
              <strong style={{ color: "#ED1C2E" }}>
                Dock {dock.dockCode}
                {dock.dockBay ? ` · Bay ${dock.dockBay}` : ""}
              </strong>
            ) : (
              "—"
            )}
          </div>
          <div className="k">Assigned to</div>
          <div style={{ color: "#ED1C2E", fontWeight: 600 }}>
            {order.assignedToUserName || "—"}
          </div>
          <div className="k">Status flow</div>
          <div>
            <span style={{ fontSize: 11, color: "#374151" }}>
              {STATUS_FLOW.map((step, i) => {
                const active =
                  order.status === step.value ||
                  (order.status === 4 && step.value === 3);
                return (
                  <span key={step.value}>
                    {i > 0 ? " → " : ""}
                    {active ? (
                      <strong style={{ color: "#2563EB" }}>{step.label}</strong>
                    ) : (
                      step.label
                    )}
                  </span>
                );
              })}
            </span>
          </div>
          <div className="k">Deltas detected</div>
          <div>
            {hasQtyDelta ? (
              <span className="badge" style={{ background: "#FEF3C7", color: "#92400E", fontSize: 10 }}>
                1 (q-ty)
              </span>
            ) : (
              <span style={{ color: "#6B7280", fontSize: 12 }}>None</span>
            )}
          </div>
        </div>
      </div>

      <div className="xd-dock-row">
        <div className="xd-panel">
          <div className="xd-panel-title">Your assigned dock</div>
          <div className="xd-dock-map">
            <div
              style={{
                position: "absolute",
                top: 12,
                left: 12,
                right: 12,
                height: 22,
                background: "#1F2A3A",
                borderRadius: 4,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                color: "#fff",
                fontSize: 10,
                fontWeight: 700,
                letterSpacing: 1,
              }}
            >
              {(dock.hubName || order.hubName).toUpperCase()} HUB
            </div>
            {(dock.hubDocks?.length ? dock.hubDocks : [{ code: dock.dockCode || "—", bayLabel: dock.dockBay, isAssigned: true }]).map(
              (d, index, arr) => {
                const assigned = d.isAssigned || d.code === dock.dockCode;
                const mid = Math.floor(arr.length / 2);
                const left = assigned
                  ? "50%"
                  : index < mid
                    ? 30 + index * 44
                    : undefined;
                const right = !assigned && index >= mid ? 30 + (arr.length - 1 - index) * 44 : undefined;
                return (
                  <div
                    key={`${d.code}-${index}`}
                    style={{
                      position: "absolute",
                      top: 44,
                      left: left !== undefined ? left : undefined,
                      right: right !== undefined ? right : undefined,
                      transform: assigned ? "translateX(-50%)" : undefined,
                      width: assigned ? 70 : 38,
                      height: 50,
                      background: assigned ? "#ED1C2E" : "#9CA3AF",
                      border: assigned ? "2px solid #991B1B" : undefined,
                      borderRadius: 4,
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "center",
                      color: "#fff",
                      fontSize: assigned ? 11 : 9,
                      fontWeight: 700,
                    }}
                  >
                    {assigned ? `DOCK ${d.code}` : d.code}
                  </div>
                );
              },
            )}
            {dock.trailerNumber && (
              <div
                style={{
                  position: "absolute",
                  bottom: 10,
                  left: "50%",
                  transform: "translateX(-50%)",
                  width: 56,
                  height: 28,
                  background: "#fff",
                  border: "2px solid #1F2A3A",
                  borderRadius: 3,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  fontSize: 9,
                  fontWeight: 700,
                }}
              >
                🚚 {dock.trailerNumber}
              </div>
            )}
          </div>
          <div style={{ fontSize: 12, marginTop: 10, lineHeight: 1.55 }}>
            <div>
              <strong>
                {dock.dockCode
                  ? `Dock ${dock.dockCode}${dock.dockBay ? ` · Bay ${dock.dockBay}` : ""}`
                  : "No dock assigned"}
              </strong>
            </div>
            <div style={{ color: "#6B7280" }}>
              {dock.assignedAt ? `Assigned ${formatDetailDateTime(dock.assignedAt)}` : "Not assigned yet"}
            </div>
            {dock.statusLabel && (
              <div style={{ color: "#16A34A", fontWeight: 600, marginTop: 4 }}>
                ● {dock.statusLabel}
              </div>
            )}
          </div>
        </div>

        <div className="xd-qty-grid">
          <div className="xd-panel" style={{ padding: 12 }}>
            <div className="xd-panel-title" style={{ marginBottom: 6 }}>
              Expected (BOL)
            </div>
            <div style={{ fontSize: 28, fontWeight: 700, color: "#1F2A3A" }}>
              {order.expected.quantity ?? "—"}
            </div>
            <div style={{ fontSize: 11, color: "#6B7280" }}>
              {order.expected.unitLabel || "—"}
            </div>
          </div>
          <div
            className="xd-panel"
            style={{
              padding: 12,
              background: hasQtyDelta ? "#FEF3C7" : undefined,
              border: hasQtyDelta ? "1px solid #FDE68A" : undefined,
            }}
          >
            <div
              className="xd-panel-title"
              style={{ marginBottom: 6, color: hasQtyDelta ? "#78350F" : undefined }}
            >
              Actual (warehouse)
            </div>
            <div
              style={{
                fontSize: 28,
                fontWeight: 700,
                color: hasQtyDelta ? "#78350F" : "#1F2A3A",
                display: "flex",
                alignItems: "baseline",
                gap: 6,
              }}
            >
              {order.actual.quantity ?? "—"}{" "}
              {hasQtyDelta && (
                <span
                  style={{
                    fontSize: 12,
                    background: "#FB923C",
                    color: "#fff",
                    padding: "2px 8px",
                    borderRadius: 10,
                  }}
                >
                  {deltaLabel(qtyDelta)}
                </span>
              )}
            </div>
            <div style={{ fontSize: 11, color: hasQtyDelta ? "#78350F" : "#6B7280" }}>
              {hasQtyDelta ? "Delta from BOL · read-only" : "Matches BOL"}
            </div>
          </div>
          <div className="xd-panel" style={{ padding: 12 }}>
            <div className="xd-panel-title" style={{ marginBottom: 6 }}>
              Warehouse note
            </div>
            <div style={{ fontSize: 12, color: "#1F2A3A", lineHeight: 1.5 }}>
              {order.warehouseNote.text ? `“${order.warehouseNote.text}”` : "—"}
            </div>
            <div style={{ marginTop: 6, display: "flex", gap: 4, flexWrap: "wrap" }}>
              {photos.slice(0, 3).map((p) => (
                <a key={p.id} href={mediaUrl(p.downloadUrl)} target="_blank" rel="noreferrer">
                  <img
                    src={mediaUrl(p.downloadUrl)}
                    alt={p.fileName}
                    style={{
                      width: 34,
                      height: 34,
                      objectFit: "cover",
                      borderRadius: 4,
                      background: "#E5E7EB",
                    }}
                  />
                </a>
              ))}
              {photos.length > 3 && (
                <button
                  type="button"
                  onClick={() => setPhotosOpen(true)}
                  style={{
                    width: 34,
                    height: 34,
                    background: "#E5E7EB",
                    borderRadius: 4,
                    border: "none",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    fontSize: 11,
                    color: "#6B7280",
                    cursor: "pointer",
                  }}
                >
                  +{photos.length - 3}
                </button>
              )}
              {photos.length === 0 && (
                <button
                  type="button"
                  onClick={() => setPhotosOpen(true)}
                  style={{
                    width: 34,
                    height: 34,
                    background: "#E5E7EB",
                    borderRadius: 4,
                    border: "none",
                    cursor: "pointer",
                    fontSize: 14,
                  }}
                >
                  📷
                </button>
              )}
            </div>
          </div>
        </div>
      </div>

      <OperationsTable
        orderId={order.id}
        operations={order.operations ?? []}
        defaultTrailer={order.trailerNumber}
      />

      <div id="order-supplies">
        <SuppliesTable
          orderId={order.id}
          orderNumber={order.number}
          supplies={order.supplies ?? []}
          subtotalCents={order.suppliesSubtotalCents ?? 0}
          isPaid={order.isPaid}
        />
      </div>

      <CommentsPanel
        orderId={order.id}
        open={commentsOpen}
        onClose={() => setCommentsOpen(false)}
      />
      <WarehousePhotosPanel
        orderId={order.id}
        photos={photos}
        open={photosOpen}
        onClose={() => setPhotosOpen(false)}
      />
      <TimelinePanel
        orderId={order.id}
        open={timelineOpen}
        onClose={() => setTimelineOpen(false)}
      />
      {editOpen && (
        <EditOrderModal order={order} open onClose={() => setEditOpen(false)} />
      )}
    </>
  );
}
