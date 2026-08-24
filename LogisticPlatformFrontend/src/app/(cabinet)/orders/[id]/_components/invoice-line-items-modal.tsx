"use client";

import type { OrderSupply } from "@/types/orders";
import { formatMoneyCents } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  open: boolean;
  onClose: () => void;
  orderNumber?: string;
  supplies: OrderSupply[];
  subtotalCents: number;
};

export function InvoiceLineItemsModal({
  open,
  onClose,
  orderNumber,
  supplies,
  subtotalCents,
}: Props) {
  return (
    <DetailModal
      open={open}
      title={orderNumber ? `Invoice · ${orderNumber}` : "Invoice line items"}
      onClose={onClose}
      width={560}
    >
      <p style={{ fontSize: 12, color: "#6B7280", marginBottom: 14 }}>
        Platform sale supplies for this order
      </p>

      {supplies.length === 0 ? (
        <div style={{ color: "#6B7280", fontSize: 13 }}>No line items yet.</div>
      ) : (
        <table className="xd-table" style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>SKU</th>
              <th style={{ padding: "8px 6px" }}>Category</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Q-ty</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Unit $</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Line total</th>
            </tr>
          </thead>
          <tbody>
            {supplies.map((s) => (
              <tr key={s.id}>
                <td>{s.name || s.sku}</td>
                <td style={{ padding: "10px 6px" }}>
                  <span
                    className="badge"
                    style={{ background: "#DBEAFE", color: "#1E40AF", fontSize: 10 }}
                  >
                    {s.category || "—"}
                  </span>
                </td>
                <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>
                  {s.quantity}
                </td>
                <td style={{ padding: "10px 6px", textAlign: "right" }}>
                  {formatMoneyCents(s.unitPriceCents)}
                </td>
                <td style={{ padding: "10px 6px", textAlign: "right", fontWeight: 600 }}>
                  {formatMoneyCents(s.lineTotalCents)}
                </td>
              </tr>
            ))}
          </tbody>
          <tfoot style={{ background: "#F9FAFB", fontSize: 12 }}>
            <tr style={{ borderTop: "1px solid #E5E7EB" }}>
              <td
                colSpan={4}
                style={{
                  padding: "10px 14px",
                  textAlign: "right",
                  color: "#6B7280",
                  fontWeight: 600,
                }}
              >
                Subtotal
              </td>
              <td
                style={{
                  padding: "10px 6px",
                  textAlign: "right",
                  fontWeight: 700,
                  color: "#1F2A3A",
                }}
              >
                {formatMoneyCents(subtotalCents)}
              </td>
            </tr>
          </tfoot>
        </table>
      )}
    </DetailModal>
  );
}
