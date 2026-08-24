"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import { getSessionAction } from "@/actions/auth/get-session.action";
import { GlobalOrderSearch } from "@/components/freitty/global-order-search";
import { HelpModal } from "@/components/freitty/help-modal";
import { NotificationsPanel } from "@/components/freitty/notifications-panel";
import { useLogout } from "@/hooks/use-logout";

type Props = {
  children: React.ReactNode;
  ordersBadge?: number;
  showNewOrder?: boolean;
  searchPlaceholder?: string;
};

function initials(name: string) {
  const parts = name.trim().split(/\s+/);
  if (parts.length === 0) return "U";
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return `${parts[0][0] ?? ""}${parts[1][0] ?? ""}`.toUpperCase();
}

export function FreittyShell({
  children,
  ordersBadge = 3,
  showNewOrder = false,
  searchPlaceholder = "Search orders by number, ref, hub, carrier…",
}: Props) {
  const pathname = usePathname();
  const { logout, pending } = useLogout();
  const [name, setName] = useState("User");
  const [helpOpen, setHelpOpen] = useState(false);
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const ordersActive =
    pathname === "/dashboard" || pathname.startsWith("/orders") || pathname === "/";
  const settingsActive = pathname.startsWith("/settings");

  useEffect(() => {
    void getSessionAction().then((user) => {
      if (user?.name) setName(user.name);
    });
  }, []);

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
        <div style={{ padding: 20, marginTop: "auto" }}>
          <button
            type="button"
            className="btn btn-ghost"
            style={{ color: "#B0B8C4", width: "100%" }}
            disabled={pending}
            onClick={logout}
          >
            {pending ? "Logging out…" : "Log out"}
          </button>
        </div>
      </aside>

      <main className="fc-main">
        <div className="fc-topbar">
          <GlobalOrderSearch placeholder={searchPlaceholder} />
          <div className="spacer" />
          <div className="fc-balance" title="Поточний баланс · клік → Billing">
            💳 $1 <span className="topup">Top up →</span>
          </div>
          <div style={{ position: "relative" }}>
            <button
              type="button"
              className="fc-topbar-btn"
              aria-label="Notifications"
              aria-expanded={notificationsOpen}
              onClick={() => setNotificationsOpen((v) => !v)}
            >
              🔔 {notificationsOpen ? null : <span className="dot" />}
            </button>
            <NotificationsPanel
              open={notificationsOpen}
              onClose={() => setNotificationsOpen(false)}
            />
          </div>
          <button
            type="button"
            className="fc-topbar-btn"
            aria-label="Help"
            aria-expanded={helpOpen}
            onClick={() => setHelpOpen(true)}
          >
            ❓
          </button>
          <div className="fc-user">
            <div className="avatar">{initials(name)}</div>
            <div className="name">{name}</div>
          </div>
        </div>
        <div className="fc-content">{children}</div>
      </main>

      <HelpModal open={helpOpen} onClose={() => setHelpOpen(false)} />
    </div>
  );
}
