"use client";

import Link from "next/link";
import { useState } from "react";

const ICONS = (
  <div className="oc-icons">
    <span>💬</span>
    <span>📷</span>
    <span>📄</span>
    <span>🖨</span>
    <span>⏱</span>
  </div>
);

type Tab = "all" | "cross" | "consol" | "alerts" | "drafts";

export default function OrdersPage() {
  const [tab, setTab] = useState<Tab>("all");
  const [view, setView] = useState<"cards" | "table" | "pipeline">("cards");

  return (
    <>
      <div className="fc-crumbs">
        Home <span>›</span> Orders
      </div>
      <div className="fc-page-title">
        <h1>All Orders</h1>
        <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
          <button type="button" className="btn btn-secondary">
            📥 Export CSV
          </button>
          <button type="button" className="btn btn-primary">
            + New Order
          </button>
        </div>
      </div>

      <div className="tabs">
        {(
          [
            { id: "all", label: "All", count: 27 },
            { id: "cross", label: "Cross-Dock", count: 18 },
            { id: "consol", label: "Consolidation", count: 6 },
            { id: "alerts", label: "Alerts", count: 2, alert: true },
            { id: "drafts", label: "Drafts", count: 1 },
          ] as const
        ).map((t) => (
          <button
            key={t.id}
            type="button"
            className={`tab${tab === t.id ? " active" : ""}`}
            onClick={() => setTab(t.id)}
          >
            {t.label}{" "}
            <span className={`count${"alert" in t && t.alert ? " alert" : ""}`}>{t.count}</span>
          </button>
        ))}
      </div>

      <div className="filters-row">
        <select defaultValue="all">
          <option value="all">Hub: All</option>
          <option>Markham</option>
          <option>Toronto</option>
        </select>
        <select defaultValue="30">
          <option value="30">Date: Last 30 days</option>
          <option>Today</option>
          <option>This week</option>
        </select>
        <select defaultValue="any">
          <option value="any">Status: Any</option>
          <option>New</option>
          <option>In progress</option>
        </select>
        <div className="view-toggle">
          <button type="button" className={view === "cards" ? "active" : undefined} onClick={() => setView("cards")}>
            ⊞ Cards
          </button>
          <button type="button" className={view === "table" ? "active" : undefined} onClick={() => setView("table")}>
            ☰ Table
          </button>
          <button
            type="button"
            className={view === "pipeline" ? "active" : undefined}
            onClick={() => setView("pipeline")}
          >
            ⎔ Pipeline
          </button>
        </div>
      </div>

      <div className="pipeline-hint">
        <strong>⎔ Pipeline view:</strong> замість лінійного списку — 7 колонок kanban (Draft · Ready ·
        In Progress · Consolidated · In Transit · Deconsolidated · Closed). Drag-drop між колонками
        обмежений роллю (клієнт тільки з Draft → Ready). Фільтр chip-ами над колонками.
      </div>

      <div className="cards-grid cols-3">
        <Link href="/orders/FR001676" className="order-card">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001676</div>
                <details className="ref-n-multi" onClick={(e) => e.preventDefault()}>
                  <summary>
                    <span className="lbl">Ref N</span>
                    <span className="count">3</span>
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
              <div className="oc-type">Consolidation · 3 sub</div>
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
              <div className="v">12 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">15 Std + 3 XL</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">User 5</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>Next: Loading</div>
            {ICONS}
          </div>
        </Link>

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
              <div className="v">15 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">23 × Std</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">Schneider</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>Next: Await truck</div>
            {ICONS}
          </div>
        </Link>

        <Link href="/orders/FR001674" className="order-card alert-border">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001674</div>
                <details className="ref-n-multi" onClick={(e) => e.preventDefault()}>
                  <summary>
                    <span className="lbl">Ref N</span>
                    <span className="count">2</span>
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
                        7 pallets · <em style={{ color: "#DC2626" }}>⚠ alert</em>
                      </span>
                    </div>
                  </div>
                </details>
              </div>
              <div className="oc-type">Consolidation · 2 sub</div>
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
              <div className="v">13 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">
                20 decl · <span style={{ color: "#EA580C" }}>18 act</span>
              </div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">TForce</div>
            </div>
          </div>
          <div className="oc-footer">
            <div style={{ color: "#DC2626", fontWeight: 700 }}>⚠ Upload photo</div>
            {ICONS}
          </div>
        </Link>

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
              <span className="badge badge-done">Done</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Markham</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">14 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">10 × XL</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">Self</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>Next: Paid · $1</div>
            {ICONS}
          </div>
        </Link>

        <Link href="/orders/FR001668" className="order-card">
          <div className="oc-head">
            <div>
              <div className="oc-id-row">
                <div className="oc-num">FR001668</div>
                <details className="ref-n-multi" onClick={(e) => e.preventDefault()}>
                  <summary>
                    <span className="lbl">Ref N</span>
                    <span className="count">4</span>
                  </summary>
                  <div className="ref-list">
                    <div className="ref-item">
                      <span className="sub-id">FR001668-1</span>
                      <span className="sub-ref">REF-1008</span>
                      <span className="sub-desc">15 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001668-2</span>
                      <span className="sub-ref">REF-1009</span>
                      <span className="sub-desc">8 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001668-3</span>
                      <span className="sub-ref">REF-1010</span>
                      <span className="sub-desc">20 pallets</span>
                    </div>
                    <div className="ref-item">
                      <span className="sub-id">FR001668-4</span>
                      <span className="sub-ref">REF-1011</span>
                      <span className="sub-desc">12 pallets</span>
                    </div>
                  </div>
                </details>
              </div>
              <div className="oc-type">Consolidation · 4 sub</div>
              <div className="oc-by">
                <span className="lbl">by</span>
                <span className="chip">
                  <span className="ava u4">U4</span>User 4
                </span>
                <span className="role disp">Dispatcher</span>
              </div>
            </div>
            <div>
              <span className="badge badge-consol">Consolidation</span>{" "}
              <span className="badge badge-done">Done</span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Mark→Tor</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">11 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">28 × Std</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">TForce</div>
            </div>
          </div>
          <div className="oc-footer">
            <div>Next: Closed</div>
            {ICONS}
          </div>
        </Link>

        <div className="order-card draft">
          <div className="oc-head">
            <div>
              <div className="oc-num">DRAFT-003</div>
              <div className="oc-type">Consolidation · incomplete</div>
            </div>
            <div>
              <span className="badge" style={{ background: "#E5E7EB", color: "#374151" }}>
                DRAFT
              </span>
            </div>
          </div>
          <div className="oc-body">
            <div>
              <div className="k">Hub</div>
              <div className="v">Markham</div>
            </div>
            <div>
              <div className="k">Date</div>
              <div className="v">16 Apr</div>
            </div>
            <div>
              <div className="k">Q-ty</div>
              <div className="v">—</div>
            </div>
            <div>
              <div className="k">Carrier</div>
              <div className="v">—</div>
            </div>
          </div>
          <div className="oc-footer">
            <div style={{ color: "#2E75B6", fontWeight: 600 }}>Continue editing →</div>
            <div className="oc-icons">
              <span>🗑</span>
            </div>
          </div>
        </div>
      </div>

      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginTop: 20,
          fontSize: 12,
          color: "#6B7280",
        }}
      >
        <div>Showing 6 of 27</div>
        <div style={{ display: "flex", gap: 4 }}>
          <button type="button" className="btn btn-secondary" style={{ padding: "4px 10px" }}>
            ← Prev
          </button>
          <button type="button" className="btn btn-secondary" style={{ padding: "4px 10px" }}>
            Next →
          </button>
        </div>
      </div>
    </>
  );
}
