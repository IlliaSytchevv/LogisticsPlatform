"use client";

import type { SupplyCatalogItem } from "@/types/supplies";

export type CatalogPick = {
  catalogItemId: string;
  quantity: number;
};

type Props = {
  items: SupplyCatalogItem[];
  picks: Record<string, number>;
  onChange: (catalogItemId: string, quantity: number) => void;
  loading?: boolean;
  /** Real API/load failure — do not confuse with an empty DB table. */
  error?: string | null;
};

function formatPlatformPrice(cents: number): string {
  return `$${(cents / 100).toFixed(cents % 100 === 0 ? 0 : 2)}`;
}

export function FoeSupplyPicker({ items, picks, onChange, loading, error }: Props) {
  if (loading) {
    return <p style={{ fontSize: 12, color: "#6B7280" }}>Loading FOE catalog…</p>;
  }

  if (error) {
    return (
      <p style={{ fontSize: 12, color: "#DC2626", lineHeight: 1.45 }}>
        Cannot load FOE catalog: {error}
        <br />
        Check <code>GET /api/v1/supplies/catalog</code> in Swagger and that table{" "}
        <code>SupplyCatalogItems</code> exists (migration applied).
      </p>
    );
  }

  if (items.length === 0) {
    return (
      <p style={{ fontSize: 12, color: "#DC2626", lineHeight: 1.45 }}>
        FOE catalog table is empty (0 SKUs). Orders seed ≠ supply catalog.
        <br />
        1) Apply migration so <code>SupplyCatalogItems</code> exists
        <br />
        2) Run <code>POST /api/v1/seed</code> again (inserts 16 SKUs only if table is empty)
      </p>
    );
  }

  return (
    <div>
      <div style={{ fontSize: 11, color: "#6B7280", marginBottom: 8, lineHeight: 1.4 }}>
        FOE catalog ({items.length} SKUs). Client sees <strong>Platform price</strong> only — WP and
        margin split are hidden.
      </div>
      <div
        style={{
          maxHeight: 280,
          overflowY: "auto",
          border: "1px solid #E5E7EB",
          borderRadius: 8,
        }}
      >
        {items.map((item) => {
          const qty = picks[item.id] ?? 0;
          const selected = qty > 0;
          return (
            <div
              key={item.id}
              style={{
                display: "grid",
                gridTemplateColumns: "1fr auto auto",
                gap: 10,
                alignItems: "center",
                padding: "8px 10px",
                borderBottom: "1px solid #F3F4F6",
                background: selected ? "#F0F9FF" : "#fff",
              }}
            >
              <div>
                <div style={{ fontSize: 13, fontWeight: 700, color: "#1F2A3A" }}>{item.name}</div>
                <div style={{ fontSize: 11, color: "#6B7280" }}>
                  {item.sku} · {item.category}
                </div>
              </div>
              <div style={{ fontSize: 13, fontWeight: 700, color: "#1F2A3A", whiteSpace: "nowrap" }}>
                {formatPlatformPrice(item.platformPriceCents)}
              </div>
              <input
                type="number"
                min={0}
                max={10000}
                value={qty}
                onChange={(e) => onChange(item.id, Math.max(0, Number(e.target.value) || 0))}
                style={{
                  width: 64,
                  padding: "6px 8px",
                  border: "1px solid #E5E7EB",
                  borderRadius: 6,
                  fontSize: 13,
                }}
                aria-label={`Qty for ${item.sku}`}
              />
            </div>
          );
        })}
      </div>
    </div>
  );
}

export function picksToLines(picks: Record<string, number>): CatalogPick[] {
  return Object.entries(picks)
    .filter(([, qty]) => qty > 0)
    .map(([catalogItemId, quantity]) => ({ catalogItemId, quantity }));
}
