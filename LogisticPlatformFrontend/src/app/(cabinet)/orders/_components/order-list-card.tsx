"use client";

import Link from "next/link";
import type { OrderListItem } from "@/types/orders";
import {
  avatarClass,
  formatScheduledShort,
  nextActionText,
  roleLabel,
  statusBadgeClass,
  typeBadgeClass,
  typeBadgeText,
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

function RefBlock({ order }: { order: OrderListItem }) {
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
        <span className="count">{refs.length}</span>
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

export function OrderListCard({ order }: { order: OrderListItem }) {
  const role = roleLabel(order.createdBy.role);
  const next = nextActionText(order);
  const isDraft = order.isDraftIncomplete || order.status === 1;

  return (
    <Link
      href={`/orders/${order.id}`}
      className={`order-card${order.hasAlert ? " alert-border" : ""}${isDraft ? " draft" : ""}`}
    >
      <div className="oc-head">
        <div>
          <div className="oc-id-row">
            <div className="oc-num">{order.number}</div>
            {!isDraft ? <RefBlock order={order} /> : null}
          </div>
          <div className="oc-type">{order.subtitle}</div>
          {!isDraft ? (
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
          ) : null}
        </div>
        <div>
          {!isDraft ? (
            <>
              <span className={`badge ${typeBadgeClass(order.type)}`}>
                {typeBadgeText(order.type)}
              </span>{" "}
            </>
          ) : null}
          <span
            className={`badge ${isDraft ? "" : statusBadgeClass(order.status, order.hasAlert)}`}
            style={isDraft ? { background: "#E5E7EB", color: "#374151" } : undefined}
          >
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
          <div className="v">{formatScheduledShort(order.scheduledAt)}</div>
        </div>
        <div>
          <div className="k">Q-ty</div>
          <div className="v">
            {order.declaredQty != null &&
            order.actualQty != null &&
            order.declaredQty !== order.actualQty ? (
              <>
                {order.declaredQty} decl ·{" "}
                <span style={{ color: "#EA580C" }}>{order.actualQty} act</span>
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
      </div>

      <div className="oc-footer">
        <div
          style={
            order.nextAction.isAlert || next.startsWith("⚠")
              ? { color: "#DC2626", fontWeight: 700 }
              : isDraft
                ? { color: "#2E75B6", fontWeight: 600 }
                : undefined
          }
        >
          {next}
        </div>
        {isDraft ? (
          <div className="oc-icons">
            <span>🗑</span>
          </div>
        ) : (
          ICON_BAR
        )}
      </div>
    </Link>
  );
}
