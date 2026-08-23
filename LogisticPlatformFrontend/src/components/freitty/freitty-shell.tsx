"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { logoutAction } from "@/actions/auth";

type Props = {
  children: React.ReactNode;
  ordersBadge?: number;
  showNewOrder?: boolean;
  searchPlaceholder?: string;
};

export function FreittyShell({
  children,
  ordersBadge = 3,
  showNewOrder = false,
  searchPlaceholder = "🔍 Search orders, invoices, documents…",
}: Props) {
  const pathname = usePathname();
  const ordersActive =
    pathname === "/dashboard" || pathname.startsWith("/orders") || pathname === "/";
  const settingsActive = pathname.startsWith("/settings");

  return (
    <div className="fc-shell">
      <aside className="fc-sidebar" style={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}>
        <div className="fc-logo">
          FREITT<span>Y</span>
        </div>
        <ul className="fc-nav">
          <li className={ordersActive ? "active" : undefined}>
            <Link href="/dashboard" style={{ display: "contents" }}>
              📦 Orders
              {ordersBadge > 0 ? <span className="badge">{ordersBadge}</span> : null}
            </Link>
          </li>
          <li className={settingsActive ? "active" : undefined}>
            <Link href="/settings" style={{ display: "contents" }}>
              ⚙️ Settings
            </Link>
          </li>
        </ul>
        {showNewOrder ? (
          <div style={{ padding: 20, marginTop: 20, borderTop: "1px solid rgba(255,255,255,.1)" }}>
            <Link
              href="/orders"
              className="btn btn-primary"
              style={{ width: "100%", justifyContent: "center" }}
            >
              + New Order
            </Link>
          </div>
        ) : null}
        <form action={logoutAction} style={{ padding: 20, marginTop: "auto" }}>
          <button type="submit" className="btn btn-ghost" style={{ color: "#B0B8C4", width: "100%" }}>
            Log out
          </button>
        </form>
      </aside>

      <main className="fc-main">
        <div className="fc-topbar">
          <div className="fc-search">{searchPlaceholder}</div>
          <div className="spacer" />
          <div className="fc-balance" title="Поточний баланс · клік → Billing">
            💳 $1 <span className="topup">Top up →</span>
          </div>
          <button type="button" className="fc-topbar-btn" aria-label="Notifications">
            🔔 <span className="dot" />
          </button>
          <button type="button" className="fc-topbar-btn" aria-label="Help">
            ❓
          </button>
          <div className="fc-user">
            <div className="avatar">U1</div>
            <div className="name">User 1</div>
          </div>
        </div>
        <div className="fc-content">{children}</div>
      </main>
    </div>
  );
}
