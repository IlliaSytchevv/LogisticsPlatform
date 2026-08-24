"use client";

import Link from "next/link";
import { useEffect, useRef } from "react";
import { useQuery } from "@tanstack/react-query";
import { notificationsService } from "@/api/services/notifications.service";
import type { NotificationFeedItem } from "@/types/notifications";

type Props = {
  open: boolean;
  onClose: () => void;
};

function formatWhen(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleString("en-GB", {
    day: "numeric",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

function ItemRow({ item, onClose }: { item: NotificationFeedItem; onClose: () => void }) {
  const isAlert = item.kind === "alert";
  return (
    <Link
      href={`/orders/${item.orderId}`}
      onClick={onClose}
      style={{
        display: "block",
        padding: "10px 12px",
        borderBottom: "1px solid #F3F4F6",
        textDecoration: "none",
        color: "inherit",
      }}
    >
      <div style={{ display: "flex", alignItems: "center", gap: 8, marginBottom: 4 }}>
        <span
          style={{
            fontSize: 10,
            fontWeight: 800,
            textTransform: "uppercase",
            padding: "2px 6px",
            borderRadius: 4,
            background: isAlert ? "#FEE2E2" : "#DBEAFE",
            color: isAlert ? "#991B1B" : "#1E40AF",
          }}
        >
          {isAlert ? "Alert" : "Awaiting"}
        </span>
        <strong style={{ fontSize: 13 }}>{item.orderNumber}</strong>
      </div>
      <div style={{ fontSize: 12, color: "#374151", lineHeight: 1.4 }}>{item.title}</div>
      <div style={{ fontSize: 11, color: "#9CA3AF", marginTop: 4 }}>{formatWhen(item.createdAt)}</div>
    </Link>
  );
}

export function NotificationsPanel({ open, onClose }: Props) {
  const ref = useRef<HTMLDivElement>(null);
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["notifications", "feed"],
    queryFn: () => notificationsService.feed(7, 20),
    enabled: open,
    staleTime: 30_000,
  });

  useEffect(() => {
    if (!open) return;
    const onDoc = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) onClose();
    };
    document.addEventListener("mousedown", onDoc);
    return () => document.removeEventListener("mousedown", onDoc);
  }, [open, onClose]);

  if (!open) return null;

  const items = data?.items ?? [];
  const unread = items.length;

  return (
    <div
      ref={ref}
      className="no-print"
      style={{
        position: "absolute",
        right: 0,
        top: "calc(100% + 8px)",
        width: 340,
        background: "#fff",
        border: "1px solid #E5E7EB",
        borderRadius: 10,
        boxShadow: "0 12px 32px rgba(15, 23, 42, 0.14)",
        zIndex: 40,
        overflow: "hidden",
      }}
    >
      <div
        style={{
          padding: "10px 12px",
          borderBottom: "1px solid #E5E7EB",
          fontWeight: 700,
          fontSize: 13,
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
        }}
      >
        <span>Notifications</span>
        <span style={{ fontSize: 11, color: "#6B7280", fontWeight: 600 }}>
          {unread} item{unread === 1 ? "" : "s"}
        </span>
      </div>

      <div style={{ maxHeight: 360, overflowY: "auto" }}>
        {isLoading ? (
          <p style={{ padding: 14, fontSize: 12, color: "#6B7280", margin: 0 }}>Loading…</p>
        ) : isError ? (
          <p style={{ padding: 14, fontSize: 12, color: "#DC2626", margin: 0 }}>
            {error instanceof Error ? error.message : "Failed to load"}
          </p>
        ) : items.length === 0 ? (
          <p style={{ padding: 14, fontSize: 12, color: "#6B7280", margin: 0 }}>
            No alerts or awaiting actions in the last 7 days.
          </p>
        ) : (
          items.map((item) => <ItemRow key={item.orderId} item={item} onClose={onClose} />)
        )}
      </div>
    </div>
  );
}
