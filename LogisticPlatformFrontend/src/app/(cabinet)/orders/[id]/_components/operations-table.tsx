"use client";

import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import type { OrderOperation } from "@/types/orders";
import { useSession } from "@/hooks/use-session";
import { ordersKeys } from "../../_hooks/orders-queries";
import { formatDetailDateTime } from "../_lib/format";
import { AddOperationModal } from "./add-operation-modal";
import { OperationCommentsPanel } from "./operation-comments-panel";
import { OperationPhotosPanel } from "./operation-photos-panel";

type Props = {
  orderId: string;
  operations: OrderOperation[];
  defaultTrailer?: string | null;
};

export function OperationsTable({ orderId, operations, defaultTrailer }: Props) {
  const queryClient = useQueryClient();
  const { canWrite, loading: sessionLoading } = useSession();
  const [addOpen, setAddOpen] = useState(false);
  const [commentsOp, setCommentsOp] = useState<OrderOperation | null>(null);
  const [photosOp, setPhotosOp] = useState<OrderOperation | null>(null);
  const [error, setError] = useState<string | null>(null);

  const deleteMutation = useMutation({
    mutationFn: (operationId: string) => ordersService.deleteOperation(orderId, operationId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Delete failed"),
  });

  return (
    <>
      <div className="xd-table-wrap">
        <div className="xd-table-head">
          <div style={{ fontWeight: 700, color: "#1F2A3A" }}>Operations</div>
          {!sessionLoading && canWrite ? (
            <button
              type="button"
              className="btn btn-secondary"
              style={{ fontSize: 12, padding: "4px 10px" }}
              onClick={() => setAddOpen(true)}
            >
              + Operation
            </button>
          ) : null}
        </div>
        {error && (
          <div style={{ padding: "8px 14px", color: "#DC2626", fontSize: 12 }}>{error}</div>
        )}
        <table className="xd-table">
          <thead>
            <tr>
              <th>Operation</th>
              <th style={{ padding: "8px 6px" }}>Trailer</th>
              <th style={{ padding: "8px 6px", textAlign: "right" }}>Q-ty</th>
              <th style={{ padding: "8px 6px" }}>Unit</th>
              <th style={{ padding: "8px 6px" }}>Applied at</th>
              <th style={{ padding: "8px 6px" }}>Action</th>
            </tr>
          </thead>
          <tbody>
            {operations.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ color: "#6B7280", padding: 14 }}>
                  No operations yet.
                </td>
              </tr>
            ) : (
              operations.map((op) => {
                const isDisposal = op.type === 2;
                return (
                  <tr key={op.id} style={isDisposal ? { background: "#FEF2F2" } : undefined}>
                    <td>
                      {isDisposal ? (
                        <span className="badge" style={{ background: "#FEE2E2", color: "#991B1B" }}>
                          {op.typeLabel}
                        </span>
                      ) : (
                        <>
                          {(op.type === 1 || op.type === 4) && (
                            <span style={{ color: "#059669", fontWeight: 700 }}>$ </span>
                          )}
                          {op.typeLabel}
                        </>
                      )}
                    </td>
                    <td style={{ padding: "10px 6px" }}>{op.trailer || "—"}</td>
                    <td
                      style={{
                        padding: "10px 6px",
                        textAlign: "right",
                        color: isDisposal ? "#DC2626" : "#2563EB",
                        fontWeight: 600,
                      }}
                    >
                      {op.quantity}
                    </td>
                    <td style={{ padding: "10px 6px" }}>{op.unitLabel || "—"}</td>
                    <td style={{ padding: "10px 6px", color: "#6B7280" }}>
                      {formatDetailDateTime(op.appliedAt)}
                    </td>
                    <td style={{ padding: "10px 6px", color: "#6B7280", whiteSpace: "nowrap" }}>
                      <button
                        type="button"
                        className="btn btn-secondary"
                        style={{ padding: "2px 6px", fontSize: 11, marginRight: 4 }}
                        title="Comments"
                        onClick={() => setCommentsOp(op)}
                      >
                        💬 {op.commentCount}
                      </button>
                      <button
                        type="button"
                        className="btn btn-secondary"
                        style={{ padding: "2px 6px", fontSize: 11, marginRight: 4 }}
                        title="Photos"
                        onClick={() => setPhotosOp(op)}
                      >
                        📷 {op.photoCount}
                      </button>
                      {!sessionLoading && canWrite ? (
                        <button
                          type="button"
                          className="btn btn-secondary"
                          style={{ padding: "2px 6px", fontSize: 11 }}
                          title="Delete"
                          disabled={deleteMutation.isPending}
                          onClick={() => {
                            if (confirm(`Delete operation “${op.typeLabel}”?`)) {
                              deleteMutation.mutate(op.id);
                            }
                          }}
                        >
                          🗑
                        </button>
                      ) : null}
                    </td>
                  </tr>
                );
              })
            )}
          </tbody>
        </table>
      </div>

      <AddOperationModal
        orderId={orderId}
        defaultTrailer={defaultTrailer}
        open={addOpen}
        onClose={() => setAddOpen(false)}
      />
      {commentsOp && (
        <OperationCommentsPanel
          orderId={orderId}
          operationId={commentsOp.id}
          title={commentsOp.typeLabel}
          open
          onClose={() => setCommentsOp(null)}
        />
      )}
      {photosOp && (
        <OperationPhotosPanel
          orderId={orderId}
          operationId={photosOp.id}
          title={photosOp.typeLabel}
          open
          onClose={() => setPhotosOp(null)}
        />
      )}
    </>
  );
}
