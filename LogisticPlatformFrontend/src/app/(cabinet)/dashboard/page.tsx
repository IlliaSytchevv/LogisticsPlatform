"use client";

import Link from "next/link";
import { useState } from "react";

const ICON_BAR = (
  <div className="oc-icons">
    <span>💬</span>
    <span>📷</span>
    <span>📄</span>
    <span>🖨</span>
    <span>⏱</span>
  </div>
);

export default function DashboardPage() {
  const [period, setPeriod] = useState<"Day" | "CW" | "Month" | "Quarter">("Month");

  return (
    <>
      <div className="fc-crumbs">
        Home <span>›</span> Dashboard
      </div>
      <div className="fc-page-title">
        <h1>Welcome, User 1 👋</h1>
      </div>

      <div className="kpi-grid">
        <div className="kpi kpi-accent-blue">
          <div className="label">Active Orders</div>
          <div className="value">7</div>
          <div className="trend" style={{ color: "#16A34A" }}>
            ▲ 2 this week
          </div>
        </div>
        <div className="kpi kpi-accent-green">
          <div className="label">Completed (30 d)</div>
          <div className="value">24</div>
          <div className="trend" style={{ color: "#6B7280" }}>
            ⟶ same as last month
          </div>
        </div>
        <div
          className="kpi kpi-accent-red kpi-need"
          title="Ордери, що потребують вашої дії або мають алерти"
        >
          <div className="label" style={{ color: "#B8142A", display: "flex", alignItems: "center", gap: 8 }}>
            <span>⚠ Need Attention</span>
          </div>
          <div style={{ display: "flex", alignItems: "baseline", gap: 18, marginTop: 2 }}>
            <div>
              <div className="value" style={{ color: "#B8142A" }}>
                3
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
                2 · awaiting your action
              </span>
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
                1 · alert (photo missing · FR001674)
              </span>
            </div>
          </div>
          <Link href="/orders" className="open-list">
            Open list →
          </Link>
        </div>
      </div>

      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", margin: "20px 0 12px" }}>
        <h2 style={{ fontSize: 16, color: "#1F2A3A", fontWeight: 700 }}>Active Orders</h2>
        <Link href="/orders" style={{ fontSize: 12, color: "#2E75B6", fontWeight: 600 }}>
          View all →
        </Link>
      </div>

      <div className="cards-grid">
        {/* FR001676 */}
        <Link href="/orders/FR001676" className="order-card">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001676</div>
                <details className="ref-n-multi" onClick={(e) => e.preventDefault()}>
                  <summary>
                    <span className="lbl">Ref N</span>
                    <span className="count">3 refs</span>
                  </summary>
                  <div className="ref-list">
                    <div className="ref-item">
                      <span className="sub-id">FR001676-1</span>
                      <span className="sub-ref">REF-1001</span>
                      <span className="sub-desc">9 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001676-2</span>
                      <span className="sub-ref">REF-1003</span>
                      <span className="sub-desc">6 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001676-3</span>
                      <span className="sub-ref">REF-1002</span>
                      <span className="sub-desc">12 pallets</span>
                    </div>
                  </div>
                </details>
              </div>
              <div className="oc-type">Consolidation · 3 sub-orders</div>
              <div className="oc-by">
                <span className="lbl">by</span>
                <span className="chip">
                  <span className="ava u1">U1</span>User 1
                </span>
                <span className="role">Admin</span>
              </div>
            </div>
            <div>
              <span className="badge badge-consol">Consolidation</span>{" "}
              <span className="badge badge-prog">In progress</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Markham</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">12 Apr, 09:00</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">15 × Std + 3 × XL</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">Driver: User 5</div>
            </div>
            <div>
              <div className="k">Destination </div>
              <div className="v">Toronto, ON</div>
            </div>
            <div>
              <div className="k">Trailers </div>
              <div className="v">
                <span className="trailer-pill">2 consolidated</span>
              </div>
            </div>
          </div>
          <div className="oc-footer">
            <div>
              Next: Loading · <strong style={{ color: "#1F2A3A" }}>2h 14m</strong>
            </div>
            {ICON_BAR}
          </div>
        </Link>

        {/* FR001681 */}
        <Link href="/orders/FR001681" className="order-card">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001681</div>
                <div className="ref-n-inline">
                  <span className="lbl">Ref N:</span>
                  <span className="val">REF-1004</span>
                </div>
              </div>
              <div className="oc-type">Cross-Dock · Storage</div>
              <div className="oc-by">
                <span className="lbl">by</span>
                <span className="chip">
                  <span className="ava u2">U2</span>User 2
                </span>
                <span className="role disp">Dispatcher</span>
              </div>
            </div>
            <div>
              <span className="badge badge-simple">Cross-Dock</span>{" "}
              <span className="badge badge-new">New</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Toronto</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">15 Apr, 14:00</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">23 × Standard</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">Schneider</div>
            </div>
            <div style={{ gridColumn: "span 2" }}>
              <div className="k">Destination </div>
              <div className="v">Detroit, MI · via External PDF</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>Next: Waiting for truck</div>
            {ICON_BAR}
          </div>
        </Link>

        {/* FR001674 alert */}
        <Link href="/orders/FR001674" className="order-card alert-border">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001674</div>
                <details className="ref-n-multi" open onClick={(e) => e.preventDefault()}>
                  <summary>
                    <span className="lbl">Ref N</span>
                    <span className="count">2 refs</span>
                  </summary>
                  <div className="ref-list">
                    <div className="ref-item">
                      <span className="sub-id">FR001674-1</span>
                      <span className="sub-ref">REF-1005</span>
                      <span className="sub-desc">11 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001674-2</span>
                      <span className="sub-ref">REF-1006</span>
                      <span className="sub-desc">
                        7 pallets · <em style={{ color: "#DC2626" }}>⚠ missing photo</em>
                      </span>
                    </div>
                  </div>
                </details>
              </div>
              <div className="oc-type">Consolidation · 2 sub-orders</div>
              <div className="oc-by">
                <span className="lbl">by</span>
                <span className="chip">
                  <span className="ava u3">U3</span>User 3
                </span>
                <span className="role disp">Dispatcher</span>
              </div>
            </div>
            <div>
              <span className="badge badge-consol">Consolidation</span>{" "}
              <span className="badge badge-alert">Alert</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Markham</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">13 Apr, 11:00</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">
                20 decl · <span style={{ color: "#EA580C" }}>18 actual</span>
              </div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">TForce</div>
            </div>
            <div>
              <div className="k">Destination </div>
              <div className="v">Calgary, AB</div>
            </div>
            <div>
              <div className="k">Trailers </div>
              <div className="v">
                <span className="trailer-pill">1 consolidated</span>
              </div>
            </div>
          </div>
          <div className="oc-footer">
            <div style={{ color: "#DC2626", fontWeight: 700 }}>⚠ Next: Upload photo</div>
            {ICON_BAR}
          </div>
        </Link>

        {/* FR001672 */}
        <Link href="/orders/FR001383" className="order-card">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001672</div>
                <div className="ref-n-inline">
                  <span className="lbl">Ref N:</span>
                  <span className="val">REF-1007</span>
                </div>
              </div>
              <div className="oc-type">Cross-Dock · Pickup</div>
              <div className="oc-by">
                <span className="lbl">by</span>
                <span className="chip">
                  <span className="ava u1">U1</span>User 1
                </span>
                <span className="role">Admin</span>
              </div>
            </div>
            <div>
              <span className="badge badge-simple">Cross-Dock</span>{" "}
              <span className="badge badge-done">Completed</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Markham</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">14 Apr, 17:30</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">10 × XL</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">Self pickup</div>
            </div>
            <div style={{ gridColumn: "span 2" }}>
              <div className="k">Destination </div>
              <div className="v">Brampton, ON · Order with photos</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>
              Next: Paid · $1 · <span style={{ color: "#2E75B6" }}>#001812</span>
            </div>
            {ICON_BAR}
          </div>
        </Link>
      </div>

      {/* Activity charts */}
      <div className="activity-block">
        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 16 }}>
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
            {(["Day", "CW", "Month", "Quarter"] as const).map((p) => (
              <button
                key={p}
                type="button"
                className={period === p ? "active" : undefined}
                onClick={() => setPeriod(p)}
              >
                {p}
              </button>
            ))}
          </div>
        </div>

        <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
          <div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", marginBottom: 8 }}>
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
                24 <span style={{ fontSize: 11, color: "#64748B", fontWeight: 600 }}>last 30d</span>
              </span>
            </div>
            <svg viewBox="0 0 400 120" style={{ width: "100%", height: 120 }}>
              <line x1="0" y1="30" x2="400" y2="30" stroke="#F1F5F9" strokeWidth="1" />
              <line x1="0" y1="60" x2="400" y2="60" stroke="#F1F5F9" strokeWidth="1" />
              <line x1="0" y1="90" x2="400" y2="90" stroke="#F1F5F9" strokeWidth="1" />
              <rect x="10" y="70" width="24" height="45" rx="3" fill="#16A34A" opacity="0.3" />
              <rect x="44" y="50" width="24" height="65" rx="3" fill="#16A34A" opacity="0.4" />
              <rect x="78" y="60" width="24" height="55" rx="3" fill="#16A34A" opacity="0.5" />
              <rect x="112" y="40" width="24" height="75" rx="3" fill="#16A34A" opacity="0.6" />
              <rect x="146" y="35" width="24" height="80" rx="3" fill="#16A34A" opacity="0.7" />
              <rect x="180" y="25" width="24" height="90" rx="3" fill="#16A34A" opacity="0.8" />
              <rect x="214" y="20" width="24" height="95" rx="3" fill="#16A34A" opacity="0.9" />
              <rect x="248" y="10" width="24" height="105" rx="3" fill="#16A34A" />
              <rect x="282" y="5" width="24" height="110" rx="3" fill="#16A34A" />
              <rect x="316" y="25" width="24" height="90" rx="3" fill="#16A34A" opacity="0.7" />
              <rect x="350" y="40" width="24" height="75" rx="3" fill="#16A34A" opacity="0.5" />
              <text x="22" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W1
              </text>
              <text x="124" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W4
              </text>
              <text x="226" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W7
              </text>
              <text x="328" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W10
              </text>
            </svg>
          </div>

          <div>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "baseline", marginBottom: 8 }}>
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
                $1 <span style={{ fontSize: 11, color: "#64748B", fontWeight: 600 }}>last 30d</span>
              </span>
            </div>
            <svg viewBox="0 0 400 120" style={{ width: "100%", height: 120 }}>
              <line x1="0" y1="30" x2="400" y2="30" stroke="#F1F5F9" strokeWidth="1" />
              <line x1="0" y1="60" x2="400" y2="60" stroke="#F1F5F9" strokeWidth="1" />
              <line x1="0" y1="90" x2="400" y2="90" stroke="#F1F5F9" strokeWidth="1" />
              <path
                d="M 10,80 L 50,70 L 90,75 L 130,55 L 170,50 L 210,35 L 250,30 L 290,15 L 330,25 L 370,45 L 370,115 L 10,115 Z"
                fill="#ED1C2E"
                opacity="0.12"
              />
              <path
                d="M 10,80 L 50,70 L 90,75 L 130,55 L 170,50 L 210,35 L 250,30 L 290,15 L 330,25 L 370,45"
                stroke="#ED1C2E"
                strokeWidth="2"
                fill="none"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              <circle cx="10" cy="80" r="3" fill="#ED1C2E" />
              <circle cx="50" cy="70" r="3" fill="#ED1C2E" />
              <circle cx="90" cy="75" r="3" fill="#ED1C2E" />
              <circle cx="130" cy="55" r="3" fill="#ED1C2E" />
              <circle cx="170" cy="50" r="3" fill="#ED1C2E" />
              <circle cx="210" cy="35" r="3" fill="#ED1C2E" />
              <circle cx="250" cy="30" r="3" fill="#ED1C2E" />
              <circle cx="290" cy="15" r="4" fill="#ED1C2E" stroke="#fff" strokeWidth="2" />
              <circle cx="330" cy="25" r="3" fill="#ED1C2E" />
              <circle cx="370" cy="45" r="3" fill="#ED1C2E" />
              <text x="10" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W1
              </text>
              <text x="130" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W4
              </text>
              <text x="250" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W7
              </text>
              <text x="370" y="118" fontSize="9" fill="#94A3B8" textAnchor="middle">
                W10
              </text>
            </svg>
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
            📈 <strong style={{ color: "#16A34A" }}>+18%</strong> completed orders vs previous month
          </span>
          <span>
            💰 <strong style={{ color: "#ED1C2E" }}>$1</strong> spent · avg $1/order
          </span>
          <span>⭐ Best week: W7 (peak spend $1)</span>
        </div>
      </div>
    </>
  );
}
