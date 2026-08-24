"use client";

import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import type { OrderSupply } from "@/types/orders";
import { useSession } from "@/hooks/use-session";
import { ordersKeys } from "../../_hooks/orders-queries";
import { formatMoneyCents } from "../_lib/format";
import { InvoiceLineItemsModal } from "./invoice-line-items-modal";
import { SupplyModal } from "./supply-modal";

type Props = {
  orderId: string;
  orderNumber?: string;
  supplies: OrderSupply[];
  subtotalCents: number;
};

type EditTarget =
  | { kind: "admin"; supply: OrderSupply }
  | { kind: "qty"; supply: OrderSupply }
  | null;

export function SuppliesTable({ orderId, orderNumber, supplies, subtotalCents }: Props) {
  const queryClient = useQueryClient();
  const { isAdmin, loading: sessionLoading } = useSession();
  const [addOpen, setAddOpen] = useState(false);
  const [editTarget, setEditTarget] = useState<EditTarget>(null);
  const [invoiceOpen, setInvoiceOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const deleteMutation = useMutation({
    mutationFn: (supplyId: string) => ordersService.deleteSupply(orderId, supplyId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Delete failed"),
  });

  return (
    <>
      <div className="xd-table-wrap" style={{ marginBottom: 0 }}>
        <div className="xd-table-head" style={{ padding: "10px 14px" }}>
          <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
            <strong style={{ color: "#1F2A3A" }}>Supplies</strong>
            <span style={{ fontSize: 11, color: "#6B7280" }}>
              FOE catalog · Platform price only (WP / margin hidden)
            </span>
          </div>
          <button
            type="button"
            className="btn btn-secondary"
            style={{ fontSize: 12, padding: "4px 10px" }}
            onClick={() => setAddOpen(true)}
          >
            + Supply
          </button>
        </div>
        {error && (
          <div style={{ padding: "8px 14px", color: "#DC2626", fontSize: 12 }}>{error}</div>
        )}
        <table className="xd-table">
          <thead>
            <tr>
              <th>SKU</th>
              <th style={{ padding: "8px 6px" }}>Category</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Q-ty</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Unit $</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Line total</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {supplies.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ color: "#6B7280", padding: 14 }}>
                  No supplies yet.
                </td>
              </tr>
            ) : (
              supplies.map((s) => (
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
                  <td style={{ color: "#6B7280", whiteSpace: "nowrap" }}>
                    {!sessionLoading && isAdmin ? (
                      <button
                        type="button"
                        className="btn btn-secondary"
                        style={{ padding: "2px 6px", fontSize: 11, marginRight: 4 }}
                        title="Admin: edit SKU / price"
                        onClick={() => setEditTarget({ kind: "admin", supply: s })}
                      >
                        ✏️
                      </button>
                    ) : null}
                    {!sessionLoading ? (
                      <button
                        type="button"
                        className="btn btn-secondary"
                        style={{ padding: "2px 6px", fontSize: 11, marginRight: 4 }}
                        title="Change quantity"
                        onClick={() => setEditTarget({ kind: "qty", supply: s })}
                      >
                        Qty
                      </button>
                    ) : null}
                    <button
                      type="button"
                      className="btn btn-secondary"
                      style={{ padding: "2px 6px", fontSize: 11 }}
                      disabled={deleteMutation.isPending}
                      onClick={() => {
                        if (confirm(`Delete supply “${s.name || s.sku}”?`)) {
                          deleteMutation.mutate(s.id);
                        }
                      }}
                    >
                      🗑
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
          <tfoot style={{ background: "#F9FAFB", fontSize: 12 }}>
            <tr style={{ borderTop: "1px solid #E5E7EB" }}>
              <td
                colSpan={4}
                style={{ padding: "10px 14px", textAlign: "right", color: "#6B7280", fontWeight: 600 }}
              >
                Supply subtotal
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
              <td style={{ padding: "10px 14px", fontSize: 11 }}>
                <button
                  type="button"
                  onClick={() => setInvoiceOpen(true)}
                  style={{
                    border: "none",
                    background: "none",
                    padding: 0,
                    color: "#2563EB",
                    fontWeight: 700,
                    cursor: "pointer",
                    fontSize: 11,
                  }}
                >
                  → Invoice line items
                </button>
              </td>
            </tr>
          </tfoot>
        </table>
      </div>

      {addOpen && (
        <SupplyModal orderId={orderId} mode="add" open onClose={() => setAddOpen(false)} />
      )}
      {editTarget?.kind === "admin" && (
        <SupplyModal
          orderId={orderId}
          supply={editTarget.supply}
          mode="edit-admin"
          open
          onClose={() => setEditTarget(null)}
        />
      )}
      {editTarget?.kind === "qty" && (
        <SupplyModal
          orderId={orderId}
          supply={editTarget.supply}
          mode="edit-qty"
          open
          onClose={() => setEditTarget(null)}
        />
      )}
      <InvoiceLineItemsModal
        open={invoiceOpen}
        onClose={() => setInvoiceOpen(false)}
        orderNumber={orderNumber}
        supplies={supplies}
        subtotalCents={subtotalCents}
      />
    </>
  );
}
