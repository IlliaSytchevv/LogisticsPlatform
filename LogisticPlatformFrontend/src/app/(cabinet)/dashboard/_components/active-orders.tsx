"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { dashboardActiveOrdersOptions } from "../_hooks/dashboard-queries";
import { OrderCard } from "./order-card";

export function ActiveOrdersSection() {
  const { data, isLoading, isError, error } = useQuery(dashboardActiveOrdersOptions(4));

  return (
    <>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          margin: "20px 0 12px",
        }}
      >
        <h2 style={{ fontSize: 16, color: "#1F2A3A", fontWeight: 700 }}>Active Orders</h2>
        <Link href="/orders" style={{ fontSize: 12, color: "#2E75B6", fontWeight: 600 }}>
          View all →
        </Link>
      </div>

      {isLoading ? (
        <div className="cards-grid">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="order-card" style={{ minHeight: 160, opacity: 0.5 }}>
              Loading…
            </div>
          ))}
        </div>
      ) : isError ? (
        <p style={{ color: "#DC2626", fontSize: 13 }}>
          Failed to load orders{error instanceof Error ? `: ${error.message}` : ""}
        </p>
      ) : !data?.items.length ? (
        <p style={{ color: "#6B7280", fontSize: 13 }}>No active orders</p>
      ) : (
        <div className="cards-grid">
          {data.items.map((order) => (
            <OrderCard key={order.number} order={order} />
          ))}
        </div>
      )}
    </>
  );
}
