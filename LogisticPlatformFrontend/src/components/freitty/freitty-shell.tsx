"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useState, Suspense } from "react";
import { getSessionAction } from "@/actions/auth/get-session.action";
import { GlobalOrderSearch } from "@/components/freitty/global-order-search";
import { HelpModal } from "@/components/freitty/help-modal";
import { NotificationsPanel } from "@/components/freitty/notifications-panel";
import { useLogout } from "@/hooks/use-logout";
import { useMediaQuery } from "@/hooks/use-media-query";

type Props = {
  children: React.ReactNode;
  showNewOrder?: boolean;
  searchPlaceholder?: string;
};

type NavProps = {
  ordersActive: boolean;
  settingsActive: boolean;
  showNewOrder: boolean;
  pending: boolean;
  logout: () => void;
  onNavigate?: () => void;
};

function initials(name: string) {
  const parts = name.trim().split(/\s+/);
  if (parts.length === 0) return "U";
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return `${parts[0][0] ?? ""}${parts[1][0] ?? ""}`.toUpperCase();
}

function SidebarNav({
  ordersActive,
  settingsActive,
  showNewOrder,
  pending,
  logout,
  onNavigate,
}: NavProps) {
  return (
    <>
      <div className="fc-logo">
        FREITT<span>Y</span>
      </div>
      <ul className="fc-nav">
        <li className={ordersActive ? "active" : undefined}>
          <Link href="/dashboard" style={{ display: "contents" }} onClick={onNavigate}>
            📦 Orders
          </Link>
        </li>
        <li className={settingsActive ? "active" : undefined}>
          <Link href="/settings" style={{ display: "contents" }} onClick={onNavigate}>
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
            onClick={onNavigate}
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
          onClick={() => {
            onNavigate?.();
            logout();
          }}
        >
          {pending ? "Logging out…" : "Log out"}
        </button>
      </div>
    </>
  );
}

export function FreittyShell({
  children,
  showNewOrder = false,
  searchPlaceholder = "Search orders by number, ref, hub, carrier…",
}: Props) {
  const pathname = usePathname();
  const isMobile = useMediaQuery("(max-width: 1100px)");
  const { logout, pending } = useLogout();
  const [name, setName] = useState("User");
  const [helpOpen, setHelpOpen] = useState(false);
  const [notificationsOpen, setNotificationsOpen] = useState(false);
  const [navOpen, setNavOpen] = useState(false);
  const ordersActive =
    pathname === "/dashboard" || pathname.startsWith("/orders") || pathname === "/";
  const settingsActive = pathname.startsWith("/settings");

  const navProps: NavProps = {
    ordersActive,
    settingsActive,
    showNewOrder,
    pending,
    logout,
    onNavigate: () => setNavOpen(false),
  };

  useEffect(() => {
    void getSessionAction().then((user) => {
      if (user?.name) setName(user.name);
    });
  }, []);

  useEffect(() => {
    setNavOpen(false);
  }, [pathname]);

  useEffect(() => {
    if (!isMobile || !navOpen) return;
    const prev = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      document.body.style.overflow = prev;
    };
  }, [isMobile, navOpen]);

  return (
    <div className="fc-shell">
      <aside
        className="fc-sidebar fc-sidebar-desktop"
        style={{ display: "flex", flexDirection: "column", minHeight: "100vh" }}
      >
        <SidebarNav {...navProps} />
      </aside>

      {isMobile && navOpen ? (
        <>
          <button
            type="button"
            className="fc-mobile-nav-backdrop"
            aria-label="Close menu"
            onClick={() => setNavOpen(false)}
          />
          <aside className="fc-mobile-drawer" aria-label="Navigation">
            <SidebarNav {...navProps} onNavigate={() => setNavOpen(false)} />
          </aside>
        </>
      ) : null}

      <main className="fc-main">
        <div className="fc-topbar">
          {isMobile ? (
            <button
              type="button"
              className="fc-topbar-btn fc-mobile-menu-btn"
              aria-label="Open menu"
              aria-expanded={navOpen}
              onClick={() => setNavOpen(true)}
            >
              ☰
            </button>
          ) : null}
          <Suspense fallback={<div className="fc-search" aria-hidden />}>
            <GlobalOrderSearch placeholder={searchPlaceholder} />
          </Suspense>
          <div className="spacer" />
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
