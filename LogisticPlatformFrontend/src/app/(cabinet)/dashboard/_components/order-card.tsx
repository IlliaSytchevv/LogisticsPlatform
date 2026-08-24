"use client";

import Link from "next/link";
import type { DashboardOrderCard } from "@/types/dashboard";
import {
  avatarClass,
  formatScheduled,
  nextActionText,
  roleLabel,
  statusBadgeClass,
  typeBadgeClass,
  typeSubtitle,
} from "../_lib/format";

const ICON_BAR = (
  <div className="oc-icons">
    <span>💬</span>
    <span>📷</span>
    <span>📄</span>
    <span>🖨</span>
    <span>⏱</span>
  </div>
);

function RefBlock({ order }: { order: DashboardOrderCard }) {
  const refs = order.references;
  if (refs.length === 0) return null;

  if (refs.length === 1) {
    return (
      <div className="ref-n-inline">
        <span className="lbl">Ref N:</span>
        <span className="val">{refs[0].reference}</span>
      </div>
    );
  }

  return (
    <details className="ref-n-multi" onClick={(e) => e.preventDefault()}>
      <summary>
        <span className="lbl">Ref N</span>
        <span className="count">
          {refs.length} ref{refs.length === 1 ? "" : "s"}
        </span>
      </summary>
      <div className="ref-list">
        {refs.map((r) => (
          <div key={r.subOrderNumber} className="ref-item">
            <span className="sub-id">{r.subOrderNumber}</span>
            <span className="sub-ref">{r.reference}</span>
            <span className="sub-desc">
              {r.description}
              {r.alert ? (
                <>
                  {" "}
                  · <em style={{ color: "#DC2626" }}>⚠ {r.alert}</em>
                </>
              ) : null}
            </span>
          </div>
        ))}
      </div>
    </details>
  );
}

export function OrderCard({ order }: { order: DashboardOrderCard }) {
  const isConsol = order.type === 2;
  const role = roleLabel(order.createdBy.role);
  const next = nextActionText(order);

  return (
    <Link
      href={`/orders/${order.id}`}
      className={`order-card${order.hasAlert ? " alert-border" : ""}`}
    >
      <div className="oc-head">
        <div>
          <div className="oc-id-row">
            <div className="oc-num">{order.number}</div>
            <RefBlock order={order} />
          </div>
          <div className="oc-type">{typeSubtitle(order)}</div>
          <div className="oc-by">
            <span className="lbl">by</span>
            <span className="chip">
              <span className={`ava ${avatarClass(order.createdBy.initials)}`}>
                {order.createdBy.initials}
              </span>
              {order.createdBy.name}
            </span>
            <span className={`role${role === "Dispatcher" ? " disp" : ""}`}>{role}</span>
          </div>
        </div>
        <div>
          <span className={`badge ${typeBadgeClass(order.type)}`}>
            {isConsol ? "Consolidation" : "Cross-Dock"}
          </span>{" "}
          <span className={`badge ${statusBadgeClass(order.status, order.hasAlert)}`}>
            {order.statusLabel}
          </span>
        </div>
      </div>

      <div className="oc-body">
        <div>
          <div className="k">Hub</div>
          <div className="v">{order.hub}</div>
        </div>
        <div>
          <div className="k">Date</div>
          <div className="v">{formatScheduled(order.scheduledAt)}</div>
        </div>
        <div>
          <div className="k">Q-ty</div>
          <div className="v">
            {order.declaredQty != null &&
            order.actualQty != null &&
            order.declaredQty !== order.actualQty ? (
              <>
                {order.declaredQty} decl ·{" "}
                <span style={{ color: "#EA580C" }}>{order.actualQty} actual</span>
              </>
            ) : (
              order.quantityDisplay
            )}
          </div>
        </div>
        <div>
          <div className="k">Carrier</div>
          <div className="v">{order.carrierDisplay}</div>
        </div>
        <div style={isConsol ? undefined : { gridColumn: "span 2" }}>
          <div className="k">Destination </div>
          <div className="v">{order.destinationDisplay}</div>
        </div>
        {isConsol && order.trailersConsolidated != null ? (
          <div>
            <div className="k">Trailers </div>
            <div className="v">
              <span className="trailer-pill">
                {order.trailersConsolidated} consolidated
              </span>
            </div>
          </div>
        ) : null}
      </div>

      <div className="oc-footer">
        <div style={order.nextAction.isAlert ? { color: "#DC2626", fontWeight: 700 } : undefined}>
          {next}
        </div>
        {ICON_BAR}
      </div>
    </Link>
  );
}
