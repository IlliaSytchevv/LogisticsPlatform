import type { DashboardOrderCard } from "@/types/dashboard";

export function formatCents(cents: number): string {
  return `$${(cents / 100).toFixed(cents % 100 === 0 ? 0 : 2)}`;
}

export function formatDue(seconds: number | null): string | null {
  if (seconds == null || seconds <= 0) return null;
  const h = Math.floor(seconds / 3600);
  const m = Math.floor((seconds % 3600) / 60);
  if (h > 0) return `${h}h ${m}m`;
  return `${m}m`;
}

export function formatScheduled(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleString("en-GB", {
    day: "numeric",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
    hour12: false,
  });
}

export function roleLabel(role: number): string {
  if (role === 1) return "Admin";
  if (role === 2) return "Dispatcher";
  if (role === 3) return "Driver";
  return "User";
}

export function avatarClass(initials: string): string {
  const key = initials.toUpperCase();
  if (key.includes("1") || key === "U1") return "u1";
  if (key.includes("2") || key === "U2") return "u2";
  if (key.includes("3") || key === "U3") return "u3";
  if (key.includes("4") || key === "U4") return "u4";
  const n = key.charCodeAt(0) % 4;
  return `u${n + 1}`;
}

export function typeSubtitle(order: DashboardOrderCard): string {
  if (order.type === 2) {
    const n = order.references.length;
    return `Consolidation · ${n} sub-order${n === 1 ? "" : "s"}`;
  }
  return order.typeLabel
    .replace(/CROSS-DOCK/i, "Cross-Dock")
    .replace(/CONSOLIDATION/i, "Consolidation");
}

export function statusBadgeClass(status: number, hasAlert: boolean): string {
  if (hasAlert || status === 4) return "badge-alert";
  if (status === 2) return "badge-new";
  if (status === 3) return "badge-prog";
  if (status === 5 || status === 6) return "badge-done";
  return "badge-done";
}

export function typeBadgeClass(type: number): string {
  return type === 2 ? "badge-consol" : "badge-simple";
}

export function nextActionText(order: DashboardOrderCard): string {
  const { nextAction } = order;
  const due = formatDue(nextAction.dueInSeconds);
  let text = nextAction.label || "—";
  if (due) text = `${text} · ${due}`;
  if (nextAction.amountCents != null) {
    text = `${text} · ${formatCents(nextAction.amountCents)}`;
  }
  if (nextAction.documentNumber) {
    text = `${text} · #${nextAction.documentNumber}`;
  }
  return nextAction.isAlert ? `⚠ ${text}` : text.startsWith("Next:") ? text : `Next: ${text}`;
}

export function completedTrend(delta: number): { text: string; color: string } {
  if (delta > 0) return { text: `▲ ${delta} this week`, color: "#16A34A" };
  if (delta < 0) return { text: `▼ ${Math.abs(delta)} this week`, color: "#DC2626" };
  return { text: "⟶ same as last week", color: "#6B7280" };
}

export function vsPrevMonthTrend(vs: number): { text: string; color: string } {
  if (vs > 0) return { text: `▲ ${vs} vs previous month`, color: "#16A34A" };
  if (vs < 0) return { text: `▼ ${Math.abs(vs)} vs previous month`, color: "#DC2626" };
  return { text: "⟶ same as last month", color: "#6B7280" };
}
