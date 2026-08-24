import type { OrderListItem, OrderListTab } from "@/types/orders";

export type DatePreset = "all" | "today" | "week" | "30";

export const ORDER_TABS: {
  id: OrderListTab;
  key: string;
  label: string;
  countKey: "all" | "crossDock" | "consolidation" | "alerts" | "drafts";
  alert?: boolean;
}[] = [
  { id: 1, key: "all", label: "All", countKey: "all" },
  { id: 2, key: "cross", label: "Cross-Dock", countKey: "crossDock" },
  { id: 3, key: "consol", label: "Consolidation", countKey: "consolidation" },
  { id: 4, key: "alerts", label: "Alerts", countKey: "alerts", alert: true },
  { id: 5, key: "drafts", label: "Drafts", countKey: "drafts" },
];

export function dateRangeFromPreset(preset: DatePreset): {
  dateFrom?: string;
  dateTo?: string;
} {
  if (preset === "all") return {};

  const now = new Date();
  const end = new Date(now);
  end.setHours(23, 59, 59, 999);

  const start = new Date(now);
  start.setHours(0, 0, 0, 0);

  if (preset === "week") {
    const day = start.getDay();
    const diff = (day + 6) % 7;
    start.setDate(start.getDate() - diff);
  } else if (preset === "30") {
    start.setDate(start.getDate() - 30);
  }

  return {
    dateFrom: start.toISOString(),
    dateTo: end.toISOString(),
  };
}

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

export function formatScheduledShort(iso: string): string {
  const d = new Date(iso);
  return d.toLocaleString("en-GB", {
    day: "numeric",
    month: "short",
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

export function statusBadgeClass(status: number, hasAlert: boolean): string {
  if (hasAlert || status === 4) return "badge-alert";
  if (status === 1) return "badge-done";
  if (status === 2) return "badge-new";
  if (status === 3) return "badge-prog";
  if (status === 5 || status === 6) return "badge-done";
  return "badge-done";
}

export function typeBadgeClass(type: number): string {
  return type === 2 ? "badge-consol" : "badge-simple";
}

export function typeBadgeText(type: number): string {
  return type === 2 ? "Consolidation" : "Cross-Dock";
}

export function nextActionText(order: OrderListItem): string {
  const { nextAction, isDraftIncomplete } = order;
  if (isDraftIncomplete) return "Continue editing →";

  const due = formatDue(nextAction.dueInSeconds);
  let text = nextAction.label || "—";
  if (due) text = `${text} · ${due}`;
  if (nextAction.amountCents != null) {
    text = `${text} · ${formatCents(nextAction.amountCents)}`;
  }
  if (nextAction.documentNumber) {
    text = `${text} · #${nextAction.documentNumber}`;
  }
  if (nextAction.isAlert) return `⚠ ${text}`;
  if (text.startsWith("Next:") || text.startsWith("Continue")) return text;
  return `Next: ${text}`;
}
