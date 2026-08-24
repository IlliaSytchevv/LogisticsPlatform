"use client";

import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import type { ActivityPeriod, ActivitySeriesPoint } from "@/types/dashboard";
import { dashboardActivityOptions } from "../_hooks/dashboard-queries";
import { formatCents } from "../_lib/format";

const PERIODS: { id: ActivityPeriod; label: string }[] = [
  { id: 1, label: "Day" },
  { id: 2, label: "CW" },
  { id: 3, label: "Month" },
  { id: 4, label: "Quarter" },
];

function barChart(series: ActivitySeriesPoint[], color: string) {
  const max = Math.max(1, ...series.map((p) => p.value));
  const n = Math.max(series.length, 1);
  const gap = 10;
  const barW = Math.min(28, (380 - gap * (n - 1)) / n);
  const chartH = 105;

  return (
    <svg viewBox="0 0 400 120" style={{ width: "100%", height: 120 }}>
      <line x1="0" y1="30" x2="400" y2="30" stroke="#F1F5F9" strokeWidth="1" />
      <line x1="0" y1="60" x2="400" y2="60" stroke="#F1F5F9" strokeWidth="1" />
      <line x1="0" y1="90" x2="400" y2="90" stroke="#F1F5F9" strokeWidth="1" />
      {series.map((p, i) => {
        const h = Math.max(4, (p.value / max) * chartH);
        const x = 10 + i * (barW + gap);
        const y = 115 - h;
        const opacity = 0.35 + (0.65 * (i + 1)) / n;
        return (
          <rect
            key={`${p.label}-${i}`}
            x={x}
            y={y}
            width={barW}
            height={h}
            rx="3"
            fill={color}
            opacity={opacity}
          />
        );
      })}
      {series
        .filter((_, i) => i === 0 || i === Math.floor(n / 3) || i === Math.floor((2 * n) / 3) || i === n - 1)
        .map((p, idx, arr) => {
          const i = series.indexOf(p);
          const x = 10 + i * (barW + gap) + barW / 2;
          return (
            <text key={`lbl-${idx}`} x={x} y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
              {p.label || arr[idx]?.label}
            </text>
          );
        })}
    </svg>
  );
}

function lineChart(series: ActivitySeriesPoint[], color: string) {
  const max = Math.max(1, ...series.map((p) => p.valueCents || p.value));
  const n = series.length;
  if (n === 0) {
    return <p style={{ fontSize: 12, color: "#94A3B8" }}>No data</p>;
  }

  const points = series.map((p, i) => {
    const x = n === 1 ? 200 : 10 + (i * 360) / (n - 1);
    const raw = p.valueCents || p.value;
    const y = 115 - Math.max(4, (raw / max) * 100);
    return { x, y, label: p.label };
  });

  const lineD = points.map((p, i) => `${i === 0 ? "M" : "L"} ${p.x},${p.y}`).join(" ");
  const area = `${lineD} L ${points[n - 1].x},115 L ${points[0].x},115 Z`;

  return (
    <svg viewBox="0 0 400 120" style={{ width: "100%", height: 120 }}>
      <line x1="0" y1="30" x2="400" y2="30" stroke="#F1F5F9" strokeWidth="1" />
      <line x1="0" y1="60" x2="400" y2="60" stroke="#F1F5F9" strokeWidth="1" />
      <line x1="0" y1="90" x2="400" y2="90" stroke="#F1F5F9" strokeWidth="1" />
      <path d={area} fill={color} opacity="0.12" />
      <path
        d={lineD}
        stroke={color}
        strokeWidth="2"
        fill="none"
        strokeLinecap="round"
        strokeLinejoin="round"
      />
      {points.map((p, i) => (
        <circle
          key={i}
          cx={p.x}
          cy={p.y}
          r={i === Math.floor(n / 2) ? 4 : 3}
          fill={color}
          stroke={i === Math.floor(n / 2) ? "#fff" : undefined}
          strokeWidth={i === Math.floor(n / 2) ? 2 : undefined}
        />
      ))}
      {points
        .filter((_, i) => i === 0 || i === Math.floor(n / 3) || i === Math.floor((2 * n) / 3) || i === n - 1)
        .map((p, i) => (
          <text key={i} x={p.x} y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
            {p.label}
          </text>
        ))}
    </svg>
  );
}

export function ActivityBlock() {
  const [period, setPeriod] = useState<ActivityPeriod>(3);
  const { data, isLoading, isError, error } = useQuery(dashboardActivityOptions(period));

  return (
    <div className="activity-block">
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 16,
        }}
      >
        <h2
          style={{
            fontSize: 16,
            color: "#1F2A3A",
            fontWeight: 700,
            margin: 0,
            display: "flex",
            alignItems: "center",
            gap: 8,
          }}
        >
          📊 Your activity
        </h2>
        <div className="period-pills">
          {PERIODS.map((p) => (
            <button
              key={p.id}
              type="button"
              className={period === p.id ? "active" : undefined}
              onClick={() => setPeriod(p.id)}
            >
              {p.label}
            </button>
          ))}
        </div>
      </div>

      {isLoading ? (
        <p style={{ fontSize: 13, color: "#6B7280" }}>Loading activity…</p>
      ) : isError || !data ? (
        <p style={{ color: "#DC2626", fontSize: 13 }}>
          Failed to load activity{error instanceof Error ? `: ${error.message}` : ""}
        </p>
      ) : (
        <>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
            <div>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "baseline",
                  marginBottom: 8,
                }}
              >
                <span
                  style={{
                    fontSize: 12,
                    color: "#64748B",
                    fontWeight: 600,
                    textTransform: "uppercase",
                    letterSpacing: 0.5,
                  }}
                >
                  Completed orders
                </span>
                <span style={{ fontSize: 20, fontWeight: 800, color: "#16A34A" }}>
                  {data.completedTotal}{" "}
                  <span style={{ fontSize: 11, color: "#64748B", fontWeight: 600 }}>
                    selected period
                  </span>
                </span>
              </div>
              {barChart(data.completedSeries, "#16A34A")}
            </div>

            <div>
              <div
                style={{
                  display: "flex",
                  justifyContent: "space-between",
                  alignItems: "baseline",
                  marginBottom: 8,
                }}
              >
                <span
                  style={{
                    fontSize: 12,
                    color: "#64748B",
                    fontWeight: 600,
                    textTransform: "uppercase",
                    letterSpacing: 0.5,
                  }}
                >
                  Spend
                </span>
                <span style={{ fontSize: 20, fontWeight: 800, color: "#ED1C2E" }}>
                  {formatCents(data.spendCentsTotal)}{" "}
                  <span style={{ fontSize: 11, color: "#64748B", fontWeight: 600 }}>
                    selected period
                  </span>
                </span>
              </div>
              {lineChart(data.spendSeries, "#ED1C2E")}
            </div>
          </div>

          <div
            style={{
              marginTop: 14,
              padding: "10px 14px",
              background: "#F1F5F9",
              borderRadius: 6,
              fontSize: 12,
              color: "#475569",
              display: "flex",
              gap: 16,
              flexWrap: "wrap",
            }}
          >
            <span>
              📈{" "}
              <strong
                style={{
                  color: data.insights.completedGrowthPercent >= 0 ? "#16A34A" : "#DC2626",
                }}
              >
                {data.insights.completedGrowthPercent >= 0 ? "+" : ""}
                {data.insights.completedGrowthPercent}%
              </strong>{" "}
              completed orders vs previous period
            </span>
            <span>
              💰 <strong style={{ color: "#ED1C2E" }}>{formatCents(data.insights.spendCentsTotal)}</strong>{" "}
              spent · avg {formatCents(data.insights.avgSpendCentsPerOrder)}/order
            </span>
            {data.insights.bestWeekLabel ? (
              <span>
                ⭐ Best week: {data.insights.bestWeekLabel} (peak spend{" "}
                {formatCents(data.insights.bestWeekPeakSpendCents)})
              </span>
            ) : null}
          </div>
        </>
      )}
    </div>
  );
}
