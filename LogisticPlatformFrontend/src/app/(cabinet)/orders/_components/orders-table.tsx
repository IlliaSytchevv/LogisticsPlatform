"use client";

import Link from "next/link";
import type { OrderListItem } from "@/types/orders";
import {
  formatScheduled,
  nextActionText,
  statusBadgeClass,
  typeBadgeClass,
  typeBadgeText,
} from "../_lib/format";

export function OrdersTable({ items }: { items: OrderListItem[] }) {
  if (items.length === 0) {
    return <p style={{ color: "#6B7280", fontSize: 13 }}>No orders found.</p>;
  }

  return (
    <div style={{ overflowX: "auto", border: "1px solid #E5E7EB", borderRadius: 10 }}>
      <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 13 }}>
        <thead style={{ background: "#F9FAFB", textAlign: "left" }}>
          <tr>
            {["Order", "Type", "Status", "Hub", "Date", "Q-ty", "Carrier", "Next"].map((h) => (
              <th
                key={h}
                style={{
                  padding: "10px 12px",
                  fontSize: 11,
                  color: "#6B7280",
                  fontWeight: 600,
                  borderBottom: "1px solid #E5E7EB",
                }}
              >
                {h}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {items.map((order) => (
            <tr key={order.id} style={{ borderBottom: "1px solid #F3F4F6" }}>
              <td style={{ padding: "10px 12px" }}>
                <Link href={`/orders/${order.id}`} style={{ fontWeight: 700, color: "#1F2A3A" }}>
                  {order.number}
                </Link>
                <div style={{ fontSize: 11, color: "#6B7280" }}>{order.referenceSummary}</div>
              </td>
              <td style={{ padding: "10px 12px" }}>
                <span className={`badge ${typeBadgeClass(order.type)}`}>
                  {typeBadgeText(order.type)}
                </span>
              </td>
              <td style={{ padding: "10px 12px" }}>
                <span className={`badge ${statusBadgeClass(order.status, order.hasAlert)}`}>
                  {order.statusLabel}
                </span>
              </td>
              <td style={{ padding: "10px 12px" }}>{order.hub}</td>
              <td style={{ padding: "10px 12px" }}>{formatScheduled(order.scheduledAt)}</td>
              <td style={{ padding: "10px 12px" }}>{order.quantityDisplay}</td>
              <td style={{ padding: "10px 12px" }}>{order.carrierDisplay}</td>
              <td
                style={{
                  padding: "10px 12px",
                  color: order.nextAction.isAlert ? "#DC2626" : undefined,
                  fontWeight: order.nextAction.isAlert ? 700 : 500,
                }}
              >
                {nextActionText(order)}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
