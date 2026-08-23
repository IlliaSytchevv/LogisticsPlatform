"use client";

import Link from "next/link";
import { use } from "react";

export default function OrderDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);
  const orderNo = id?.startsWith("FR") ? id : "FR001383";

  return (
    <>
      <div className="fc-crumbs">
        <Link href="/orders">Orders</Link> <span>›</span> {orderNo}
      </div>

      <div className="xd-hub-header">
        <div className="xd-hub-logo">F</div>
        <div style={{ fontSize: 18, fontWeight: 700, color: "#1F2A3A" }}>Cross-Dock management</div>
        <div
          style={{
            marginLeft: "auto",
            width: 36,
            height: 36,
            background: "#F3F4F6",
            borderRadius: 8,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            color: "#6B7280",
          }}
        >
          ☰
        </div>
      </div>

      <div className="xd-order-row">
        <Link href="/orders" className="btn btn-secondary" style={{ padding: "6px 10px", fontSize: 14 }}>
          ←
        </Link>
        <span style={{ color: "#2563EB", fontSize: 20 }}>📦</span>
        <span style={{ fontSize: 13, color: "#6B7280" }}>Order#:</span>
        <strong style={{ fontSize: 15, color: "#1F2A3A" }}>{orderNo}</strong>
        <span style={{ color: "#16A34A", fontSize: 16, fontWeight: 700 }}>$</span>
        <span style={{ color: "#6B7280", fontSize: 16 }}>🚚</span>
        <span className="badge" style={{ background: "#DBEAFE", color: "#1E40AF" }}>
          Cross-Dock
        </span>
        <span className="badge" style={{ background: "#D1FAE5", color: "#065F46" }}>
          ● On Stock
        </span>
        <span className="badge badge-prog">Loading in progress</span>
        <span className="ref-n-inline">
          <span className="lbl">Ref N:</span>
          <span className="val">REF-1012</span>
        </span>
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
            MK
          </span>
          User 2
          <span
            style={{
              fontSize: 9,
              padding: "1px 5px",
              borderRadius: 3,
              background: "#DBEAFE",
              color: "#1E40AF",
              fontWeight: 800,
              letterSpacing: ".3px",
            }}
          >
            DISP
          </span>
        </span>
        <span className="badge" style={{ background: "#FEF3C7", color: "#92400E" }}>
          ⚠ Actual ≠ Expected
        </span>
      </div>

      <div className="xd-actions">
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          💬 <span style={{ color: "#9CA3AF" }}>0/0</span>
        </button>
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          📷 <span style={{ color: "#9CA3AF" }}>0/5</span>
        </button>
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          $
        </button>
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          🖨
        </button>
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          📦
        </button>
        <button type="button" className="btn btn-secondary" style={{ padding: "7px 12px", fontSize: 12 }}>
          🕘
        </button>
        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          <button type="button" className="btn btn-secondary" style={{ fontSize: 12, padding: "7px 12px" }}>
            📷 Share QR
          </button>
          <button type="button" className="btn btn-primary" style={{ fontSize: 12, padding: "7px 12px" }}>
            📥 BOL PDF
          </button>
          <button type="button" className="btn btn-primary" style={{ padding: "7px 14px", fontSize: 12 }}>
            ✏️ Edit
          </button>
        </div>
      </div>

      <div className="xd-readonly">
        <span style={{ fontSize: 16 }}>ℹ️</span>
        <span>
          <strong>Read-only:</strong> Transfer операції між hub-ами виконує тільки call center. Якщо
          треба перенести — натисни Chat.
        </span>
      </div>

      <div className="xd-meta">
        <div className="xd-meta-col">
          <div className="k">Customer</div>
          <div style={{ fontWeight: 600 }}>R-way Transport</div>
          <div className="k">Hub</div>
          <div>Markham (ON)</div>
          <div className="k">Services</div>
          <div>
            <span className="badge" style={{ background: "#DBEAFE", color: "#1E40AF" }}>
              Transload
            </span>{" "}
            <span className="badge" style={{ background: "#EDE9FE", color: "#5B21B6" }}>
              Restock &amp; Rework
            </span>
          </div>
          <div className="k">Date</div>
          <div style={{ color: "#DC2626", fontWeight: 600 }}>17 Apr 2026 · today</div>
          <div className="k">Declared q-ty</div>
          <div style={{ fontWeight: 600 }}>
            10 <span style={{ fontSize: 11, color: "#6B7280", fontWeight: 500 }}>Std · 48×40</span>
          </div>
          <div className="k">Actual q-ty</div>
          <div style={{ fontWeight: 600, color: "#78350F" }}>
            12{" "}
            <span className="badge" style={{ background: "#FB923C", color: "#fff", fontSize: 10 }}>
              Δ +2
            </span>
          </div>
          <div className="k">Trailer type</div>
          <div>
            <span className="badge" style={{ background: "#F3F4F6", color: "#374151" }}>
              Van · 53ft
            </span>
          </div>
        </div>
        <div className="xd-meta-col">
          <div className="k">Carrier</div>
          <div style={{ fontWeight: 600 }}>R-way Transport Inc.</div>
          <div className="k">Phone</div>
          <div>+1 647 555 0199</div>
          <div className="k">Truck / trailer</div>
          <div>TRK-4521 / TRL-8830</div>
          <div className="k">Dock</div>
          <div>
            <strong style={{ color: "#ED1C2E" }}>Dock 12 · Bay B</strong>
          </div>
          <div className="k">Assigned to</div>
          <div style={{ color: "#ED1C2E", fontWeight: 600 }}>User 6 (floor lead)</div>
          <div className="k">Status flow</div>
          <div>
            <span style={{ fontSize: 11, color: "#374151" }}>
              Draft → Ready → <strong style={{ color: "#2563EB" }}>In Progress</strong> → Closed
            </span>
          </div>
          <div className="k">Deltas detected</div>
          <div>
            <span className="badge" style={{ background: "#FEF3C7", color: "#92400E", fontSize: 10 }}>
              1 (q-ty)
            </span>{" "}
            <a href="#" style={{ color: "#2563EB", fontSize: 11, fontWeight: 700, marginLeft: 6 }}>
              View all →
            </a>
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
              MARKHAM HUB
            </div>
            <div
              style={{
                position: "absolute",
                top: 44,
                left: "50%",
                transform: "translateX(-50%)",
                width: 70,
                height: 50,
                background: "#ED1C2E",
                border: "2px solid #991B1B",
                borderRadius: 4,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                color: "#fff",
                fontSize: 11,
                fontWeight: 700,
              }}
            >
              DOCK 12
            </div>
            <div
              style={{
                position: "absolute",
                top: 44,
                left: 30,
                width: 38,
                height: 50,
                background: "#9CA3AF",
                borderRadius: 4,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                color: "#fff",
                fontSize: 9,
              }}
            >
              11
            </div>
            <div
              style={{
                position: "absolute",
                top: 44,
                right: 30,
                width: 38,
                height: 50,
                background: "#9CA3AF",
                borderRadius: 4,
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                color: "#fff",
                fontSize: 9,
              }}
            >
              13
            </div>
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
              🚚 TRL-8830
            </div>
          </div>
          <div style={{ fontSize: 12, marginTop: 10, lineHeight: 1.55 }}>
            <div>
              <strong>Dock 12 · Bay B</strong>
            </div>
            <div style={{ color: "#6B7280" }}>Assigned 17 Apr, 08:42</div>
            <div style={{ color: "#16A34A", fontWeight: 600, marginTop: 4 }}>● Trailer docked · loading</div>
          </div>
        </div>

        <div className="xd-qty-grid">
          <div className="xd-panel" style={{ padding: 12 }}>
            <div className="xd-panel-title" style={{ marginBottom: 6 }}>
              Expected (BOL)
            </div>
            <div style={{ fontSize: 28, fontWeight: 700, color: "#1F2A3A" }}>10</div>
            <div style={{ fontSize: 11, color: "#6B7280" }}>Standard · 48×40</div>
          </div>
          <div
            className="xd-panel"
            style={{ padding: 12, background: "#FEF3C7", border: "1px solid #FDE68A" }}
          >
            <div className="xd-panel-title" style={{ marginBottom: 6, color: "#78350F" }}>
              Actual (warehouse)
            </div>
            <div
              style={{
                fontSize: 28,
                fontWeight: 700,
                color: "#78350F",
                display: "flex",
                alignItems: "baseline",
                gap: 6,
              }}
            >
              12{" "}
              <span
                style={{
                  fontSize: 12,
                  background: "#FB923C",
                  color: "#fff",
                  padding: "2px 8px",
                  borderRadius: 10,
                }}
              >
                +2
              </span>
            </div>
            <div style={{ fontSize: 11, color: "#78350F" }}>Delta from BOL · read-only</div>
          </div>
          <div className="xd-panel" style={{ padding: 12 }}>
            <div className="xd-panel-title" style={{ marginBottom: 6 }}>
              Warehouse note
            </div>
            <div style={{ fontSize: 12, color: "#1F2A3A", lineHeight: 1.5 }}>
              &quot;Counted 12 pallets on arrival, BOL says 10. <strong>1 pallet damaged</strong> →
              routed to Disposal.&quot;
            </div>
            <div style={{ marginTop: 6, display: "flex", gap: 4 }}>
              <div
                style={{
                  width: 34,
                  height: 34,
                  background: "#E5E7EB",
                  borderRadius: 4,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  fontSize: 14,
                }}
              >
                📷
              </div>
              <div
                style={{
                  width: 34,
                  height: 34,
                  background: "#E5E7EB",
                  borderRadius: 4,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  fontSize: 14,
                }}
              >
                📷
              </div>
              <div
                style={{
                  width: 34,
                  height: 34,
                  background: "#E5E7EB",
                  borderRadius: 4,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  fontSize: 11,
                  color: "#6B7280",
                }}
              >
                +3
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Operations */}
      <div className="xd-table-wrap">
        <div className="xd-table-head">
          <div style={{ fontWeight: 700, color: "#1F2A3A" }}>Operations</div>
          <button type="button" className="btn btn-secondary" style={{ fontSize: 12, padding: "4px 10px" }}>
            + Operation
          </button>
        </div>
        <table className="xd-table">
          <thead>
            <tr>
              <th>Operation</th>
              <th style={{ padding: "8px 6px" }}>Trailer</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Q-ty</th>
              <th style={{ padding: "8px 6px" }}>Unit</th>
              <th style={{ padding: "8px 6px" }}>Applied at</th>
              <th style={{ padding: "8px 6px" }}>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>
                <span style={{ color: "#059669", fontWeight: 700 }}>$</span> Unloading
              </td>
              <td style={{ padding: "10px 6px" }}>TRL-8830</td>
              <td style={{ padding: "10px 6px", textAlign: "right", color: "#2563EB", fontWeight: 600 }}>
                12
              </td>
              <td style={{ padding: "10px 6px" }}>Standard (48×40)</td>
              <td style={{ padding: "10px 6px", color: "#6B7280" }}>17 Apr · 08:55</td>
              <td style={{ padding: "10px 6px", color: "#9CA3AF" }}>💬 0 · ⧉ 4 · ⧉ · 🗑</td>
            </tr>
            <tr style={{ background: "#FEF2F2" }}>
              <td>
                <span className="badge" style={{ background: "#FEE2E2", color: "#991B1B" }}>
                  Disposal
                </span>
              </td>
              <td style={{ padding: "10px 6px" }}>—</td>
              <td style={{ padding: "10px 6px", textAlign: "right", color: "#DC2626", fontWeight: 600 }}>
                1
              </td>
              <td style={{ padding: "10px 6px" }}>Standard (48×40)</td>
              <td style={{ padding: "10px 6px", color: "#6B7280" }}>17 Apr · 09:10</td>
              <td style={{ padding: "10px 6px", color: "#9CA3AF" }}>💬 1 · ⧉ 2 · ⧉ · 🗑</td>
            </tr>
            <tr>
              <td>Restack</td>
              <td style={{ padding: "10px 6px" }}>—</td>
              <td style={{ padding: "10px 6px", textAlign: "right", color: "#2563EB", fontWeight: 600 }}>
                11
              </td>
              <td style={{ padding: "10px 6px" }}>Standard (48×40)</td>
              <td style={{ padding: "10px 6px", color: "#6B7280" }}>17 Apr · 09:25</td>
              <td style={{ padding: "10px 6px", color: "#9CA3AF" }}>💬 0 · ⧉ 1 · ⧉ · 🗑</td>
            </tr>
            <tr>
              <td>
                <span style={{ color: "#059669", fontWeight: 700 }}>$</span> Loading
              </td>
              <td style={{ padding: "10px 6px" }}>TRL-8830</td>
              <td style={{ padding: "10px 6px", textAlign: "right", color: "#2563EB", fontWeight: 600 }}>
                11
              </td>
              <td style={{ padding: "10px 6px" }}>Standard (48×40)</td>
              <td style={{ padding: "10px 6px", color: "#6B7280" }}>17 Apr · 10:40</td>
              <td style={{ padding: "10px 6px", color: "#9CA3AF" }}>💬 0 · ⧉ 0 · ⧉ · 🗑</td>
            </tr>
          </tbody>
        </table>
      </div>

      {/* Supplies */}
      <div className="xd-table-wrap" style={{ marginBottom: 0 }}>
        <div className="xd-table-head" style={{ padding: "10px 14px" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <strong style={{ color: "#1F2A3A" }}>Supplies</strong>
            <span style={{ fontSize: 11, color: "#6B7280" }}>
              mandatory stock items purchased as Platform Sale
            </span>
          </div>
          <button type="button" className="btn btn-secondary" style={{ fontSize: 12, padding: "4px 10px" }}>
            + Supply
          </button>
        </div>
        <table className="xd-table">
          <thead>
            <tr>
              <th>SKU</th>
              <th style={{ padding: "8px 6px" }}>Category</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Q-ty</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Unit $</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Line total</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Straps 12</td>
              <td style={{ padding: "10px 6px" }}>
                <span className="badge" style={{ background: "#DBEAFE", color: "#1E40AF", fontSize: 10 }}>
                  Securement
                </span>
              </td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>4</td>
              <td style={{ padding: "10px 6px", textAlign: "right" }}>$1</td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>$1</td>
              <td style={{ color: "#9CA3AF" }}>✏️ · 🗑</td>
            </tr>
            <tr>
              <td>Corners 50</td>
              <td style={{ padding: "10px 6px" }}>
                <span className="badge" style={{ background: "#FEF3C7", color: "#92400E", fontSize: 10 }}>
                  Edge protect
                </span>
              </td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>16</td>
              <td style={{ padding: "10px 6px", textAlign: "right" }}>$1</td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>$1</td>
              <td style={{ color: "#9CA3AF" }}>✏️ · 🗑</td>
            </tr>
            <tr>
              <td>Shrink wrap 120g</td>
              <td style={{ padding: "10px 6px" }}>
                <span className="badge" style={{ background: "#DCFCE7", color: "#166534", fontSize: 10 }}>
                  Wrap
                </span>
              </td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>2</td>
              <td style={{ padding: "10px 6px", textAlign: "right" }}>$1</td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>$1</td>
              <td style={{ color: "#9CA3AF" }}>✏️ · 🗑</td>
            </tr>
          </tbody>
          <tfoot style={{ background: "#F9FAFB", fontSize: 12 }}>
            <tr style={{ borderTop: "1px solid #E5E7EB" }}>
              <td colSpan={4} style={{ padding: "10px 14px", textAlign: "right", color: "#6B7280", fontWeight: 600 }}>
                Supply subtotal
              </td>
              <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 700, color: "#1F2A3A" }}>$1</td>
              <td style={{ padding: "10px 14px", color: "#6B7280", fontSize: 11 }}>→ Invoice line items</td>
            </tr>
          </tfoot>
        </table>
        <div className="xd-supply-hint">
          💡 Supply picker (FOE catalog — 16 SKUs) буде доступний під час створення Cross-Dock /
          Sub-order (Cargo step) і в Builder delegation mode. Client бачить тільки Platform price; WP
          та margin split — приховані.
        </div>
      </div>
    </>
  );
}
