export function formatMoneyCents(cents: number) {
  return `$${(cents / 100).toFixed(cents % 100 === 0 ? 0 : 2)}`;
}

export function formatDetailDate(iso: string | null | undefined) {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "—";
  const date = d.toLocaleDateString("en-GB", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
  const today = new Date();
  if (d.toDateString() === today.toDateString()) return `${date} · today`;
  return date;
}

export function formatDetailDateTime(iso: string | null | undefined) {
  if (!iso) return "—";
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return "—";
  const date = d.toLocaleDateString("en-GB", { day: "numeric", month: "short" });
  const time = d.toLocaleTimeString("en-GB", { hour: "2-digit", minute: "2-digit" });
  return `${date} · ${time}`;
}

export function initials(name: string | null | undefined) {
  if (!name?.trim()) return "?";
  const parts = name.trim().split(/\s+/);
  if (parts.length === 1) return parts[0].slice(0, 2).toUpperCase();
  return `${parts[0][0] ?? ""}${parts[1][0] ?? ""}`.toUpperCase();
}

export function deltaLabel(delta: number) {
  if (delta === 0) return "0";
  return delta > 0 ? `+${delta}` : String(delta);
}

export const OPERATION_TYPE_OPTIONS = [
  { value: 1 as const, label: "Unloading" },
  { value: 2 as const, label: "Disposal" },
  { value: 3 as const, label: "Restack" },
  { value: 4 as const, label: "Loading" },
];

export const PALLET_UNIT_OPTIONS = [
  { value: 1 as const, label: "Standard (48×40)" },
  { value: 2 as const, label: "XL" },
];

export const ORDER_STATUS_OPTIONS = [
  { value: 1 as const, label: "Draft" },
  { value: 2 as const, label: "New" },
  { value: 3 as const, label: "In Progress" },
  { value: 4 as const, label: "Alert" },
  { value: 5 as const, label: "Completed" },
  { value: 6 as const, label: "Closed" },
];
