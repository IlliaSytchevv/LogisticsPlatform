"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { dashboardMetricsOptions } from "../_hooks/dashboard-queries";
import { completedTrend, vsPrevMonthTrend } from "../_lib/format";

export function MetricsKpis() {
  const { data, isLoading, isError, error } = useQuery(dashboardMetricsOptions());

  if (isLoading) {
    return (
      <div className="kpi-grid">
        {[1, 2, 3].map((i) => (
          <div key={i} className="kpi" style={{ minHeight: 96, opacity: 0.5 }}>
            <div className="label">Loading…</div>
            <div className="value">—</div>
          </div>
        ))}
      </div>
    );
  }

  if (isError || !data) {
    return (
      <p style={{ color: "#DC2626", fontSize: 13, marginBottom: 16 }}>
        Failed to load metrics{error instanceof Error ? `: ${error.message}` : ""}. Is API running on{" "}
        {process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5124"}?
      </p>
    );
  }

  const activeTrend = completedTrend(data.activeOrders.deltaThisWeek);
  const completedTrendInfo = vsPrevMonthTrend(data.completedLast30Days.vsPreviousMonth);
  const alertSample = data.needAttention.alertSamples[0];

  return (
    <div className="kpi-grid">
      <div className="kpi kpi-accent-blue">
        <div className="label">Active Orders</div>
        <div className="value">{data.activeOrders.count}</div>
        <div className="trend" style={{ color: activeTrend.color }}>
          {activeTrend.text}
        </div>
      </div>

      <div className="kpi kpi-accent-green">
        <div className="label">Completed (30 d)</div>
        <div className="value">{data.completedLast30Days.count}</div>
        <div className="trend" style={{ color: completedTrendInfo.color }}>
          {completedTrendInfo.text}
        </div>
      </div>

      <div
        className="kpi kpi-accent-red kpi-need"
        title="Ордери, що потребують вашої дії або мають алерти"
      >
        <div
          className="label"
          style={{ color: "#B8142A", display: "flex", alignItems: "center", gap: 8 }}
        >
          <span>⚠ Need Attention</span>
        </div>
        <div style={{ display: "flex", alignItems: "baseline", gap: 18, marginTop: 2 }}>
          <div>
            <div className="value" style={{ color: "#B8142A" }}>
              {data.needAttention.total}
            </div>
            <div
              style={{
                fontSize: 10,
                color: "#991B1B",
                fontWeight: 700,
                textTransform: "uppercase",
                letterSpacing: "0.06em",
              }}
            >
              Total
            </div>
          </div>
          <div style={{ display: "flex", gap: 8, flexWrap: "wrap" }}>
            <span
              style={{
                fontSize: 11,
                background: "#FED7AA",
                color: "#9A3412",
                padding: "3px 8px",
                borderRadius: 8,
                fontWeight: 700,
              }}
            >
              {data.needAttention.awaitingAction} · awaiting your action
            </span>
            {data.needAttention.alerts > 0 ? (
              <span
                style={{
                  fontSize: 11,
                  background: "#FECACA",
                  color: "#7F1D1D",
                  padding: "3px 8px",
                  borderRadius: 8,
                  fontWeight: 700,
                }}
              >
                {data.needAttention.alerts} · alert
                {alertSample
                  ? ` (${alertSample.reason} · ${alertSample.orderNumber})`
                  : ""}
              </span>
            ) : null}
          </div>
        </div>
        <Link href="/orders" className="open-list">
          Open list →
        </Link>
      </div>
    </div>
  );
}
