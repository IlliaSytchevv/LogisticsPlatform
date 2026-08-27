"use client";

import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "@/api/services/orders.service";
import { useSession } from "@/hooks/use-session";
import { orderTimelineOptions, ordersKeys } from "../../_hooks/orders-queries";
import { formatDetailDateTime } from "../_lib/format";
import { DetailModal } from "./detail-modal";

type Props = {
  orderId: string;
  open: boolean;
  onClose: () => void;
};

export function TimelinePanel({ orderId, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const { canWrite } = useSession();
  const { data = [], isLoading } = useQuery({
    ...orderTimelineOptions(orderId),
    enabled: open && Boolean(orderId),
  });
  const [text, setText] = useState("");
  const [error, setError] = useState<string | null>(null);

  const addMutation = useMutation({
    mutationFn: () =>
      ordersService.addTimelineEntry(orderId, { text }),
    onSuccess: async () => {
      setText("");
      setError(null);
      await queryClient.invalidateQueries({ queryKey: ordersKeys.timeline(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Failed to add entry"),
  });

  return (
    <DetailModal open={open} title="Timeline" onClose={onClose} width={480}>
      {isLoading ? (
        <div style={{ color: "#6B7280", fontSize: 13 }}>Loading…</div>
      ) : data.length === 0 ? (
        <div style={{ color: "#6B7280", fontSize: 13, marginBottom: 12 }}>No timeline entries.</div>
      ) : (
        <div style={{ display: "grid", gap: 10, marginBottom: 14 }}>
          {data.map((e) => (
            <div key={e.id} style={{ borderBottom: "1px solid #E5E7EB", paddingBottom: 8 }}>
              <div style={{ fontSize: 12, color: "#6B7280" }}>
                <span className="badge" style={{ background: "#F3F4F6", color: "#374151", fontSize: 10 }}>
                  {e.kind}
                </span>{" "}
                {e.authorName ?? "—"} · {formatDetailDateTime(e.createdAt)}
              </div>
              <div style={{ fontSize: 13, marginTop: 4 }}>{e.text}</div>
            </div>
          ))}
        </div>
      )}

      {canWrite ? (
        <>
          <textarea
            value={text}
            onChange={(e) => setText(e.target.value)}
            rows={3}
            placeholder="Add timeline note…"
            style={{ width: "100%", marginBottom: 8 }}
          />
          {error && <div style={{ color: "#DC2626", fontSize: 12, marginBottom: 8 }}>{error}</div>}
          <button
            type="button"
            className="btn btn-primary"
            disabled={!text.trim() || addMutation.isPending}
            onClick={() => addMutation.mutate()}
          >
            {addMutation.isPending ? "Saving…" : "Add entry"}
          </button>
        </>
      ) : null}
    </DetailModal>
  );
}
