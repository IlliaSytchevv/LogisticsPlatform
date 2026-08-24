"use client";

type Props = {
  open: boolean;
  onClose: () => void;
};

const SECTIONS = [
  {
    title: "How to create an order",
    body: "On the Orders page, click “+ New Order”. Choose Type (Cross-Dock / Consolidation), Hub, and optionally a Primary reference. A draft is created, then fill in the rest on the order page via Edit.",
  },
  {
    title: "What Draft / Alert means",
    body: "Draft: incomplete order (Continue editing). Alert: needs attention (e.g. missing photo, qty mismatch). The Alerts tab on the list shows only those orders.",
  },
  {
    title: "Where to use Chat on an order",
    body: "Open the order → 💬 in the action row. Those are order-level comments (visible to the team). For comments on a specific operation, use 💬 in the Operations table.",
  },
  {
    title: "Support",
    body: "Email: support@freitty.local · Phone: +1 (416) 555-0199.",
  },
] as const;

export function HelpModal({ open, onClose }: Props) {
  if (!open) return null;

  return (
    <div
      className="no-print"
      style={{
        position: "fixed",
        inset: 0,
        background: "rgba(15, 23, 42, 0.45)",
        display: "grid",
        placeItems: "center",
        zIndex: 60,
        padding: 16,
      }}
      onClick={onClose}
    >
      <div
        className="order-card"
        style={{
          width: "min(480px, 100%)",
          cursor: "default",
          margin: 0,
          maxHeight: "85vh",
          overflow: "auto",
        }}
        onClick={(e) => e.stopPropagation()}
      >
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            marginBottom: 14,
            gap: 12,
          }}
        >
          <div style={{ fontWeight: 700, fontSize: 16 }}>Help</div>
          <button type="button" className="btn btn-secondary" style={{ padding: "4px 10px" }} onClick={onClose}>
            ✕
          </button>
        </div>

        <div style={{ display: "grid", gap: 14 }}>
          {SECTIONS.map((s) => (
            <section key={s.title}>
              <div
                style={{
                  fontSize: 12,
                  fontWeight: 700,
                  textTransform: "uppercase",
                  letterSpacing: 0.4,
                  color: "#6B7280",
                  marginBottom: 6,
                }}
              >
                {s.title}
              </div>
              <p style={{ fontSize: 13, lineHeight: 1.55, color: "#1F2A3A", margin: 0 }}>{s.body}</p>
            </section>
          ))}
        </div>
      </div>
    </div>
  );
}
