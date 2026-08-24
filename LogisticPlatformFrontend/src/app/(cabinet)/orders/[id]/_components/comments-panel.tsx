"use client";

import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import {
  orderCommentsOptions,
  ordersKeys,
} from "../../_hooks/orders-queries";
import { formatDetailDateTime } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  orderId: string;
  open: boolean;
  onClose: () => void;
};

export function CommentsPanel({ orderId, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const { data = [], isLoading } = useQuery({
    ...orderCommentsOptions(orderId),
    enabled: open && Boolean(orderId),
  });
  const [text, setText] = useState("");
  const [error, setError] = useState<string | null>(null);

  const addMutation = useMutation({
    mutationFn: () => ordersService.addComment(orderId, { text, authorName: "You" }),
    onSuccess: async () => {
      setText("");
      setError(null);
      await queryClient.invalidateQueries({ queryKey: ordersKeys.comments(orderId) });
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Failed to add comment"),
  });

  return (
    <DetailModal open={open} title="Comments" onClose={onClose} width={480}>
      {isLoading ? (
        <div style={{ color: "#6B7280", fontSize: 13 }}>Loading…</div>
      ) : data.length === 0 ? (
        <div style={{ color: "#6B7280", fontSize: 13, marginBottom: 12 }}>No comments yet.</div>
      ) : (
        <div style={{ display: "grid", gap: 10, marginBottom: 14 }}>
          {data.map((c) => (
            <div key={c.id} style={{ borderBottom: "1px solid #E5E7EB", paddingBottom: 8 }}>
              <div style={{ fontSize: 12, color: "#6B7280" }}>
                {c.authorName ?? "—"} · {formatDetailDateTime(c.createdAt)}
              </div>
              <div style={{ fontSize: 13, marginTop: 4 }}>{c.text}</div>
            </div>
          ))}
        </div>
      )}

      <textarea
        value={text}
        onChange={(e) => setText(e.target.value)}
        rows={3}
        placeholder="Add a comment…"
        style={{ width: "100%", marginBottom: 8 }}
      />
      {error && <div style={{ color: "#DC2626", fontSize: 12, marginBottom: 8 }}>{error}</div>}
      <button
        type="button"
        className="btn btn-primary"
        disabled={!text.trim() || addMutation.isPending}
        onClick={() => addMutation.mutate()}
      >
        {addMutation.isPending ? "Saving…" : "Add comment"}
      </button>
    </DetailModal>
  );
}
