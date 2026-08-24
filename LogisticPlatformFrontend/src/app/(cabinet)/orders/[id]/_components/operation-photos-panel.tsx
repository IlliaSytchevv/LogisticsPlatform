"use client";

import { useRef, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { mediaUrl, ordersService } from "@/api/services/orders.service";
import { operationPhotosOptions, ordersKeys } from "../../_hooks/orders-queries";
import { DetailModal } from "./detail-modal";

type Props = {
  orderId: string;
  operationId: string;
  title: string;
  open: boolean;
  onClose: () => void;
};

export function OperationPhotosPanel({
  orderId,
  operationId,
  title,
  open,
  onClose,
}: Props) {
  const queryClient = useQueryClient();
  const inputRef = useRef<HTMLInputElement>(null);
  const { data = [], isLoading } = useQuery({
    ...operationPhotosOptions(orderId, operationId),
    enabled: open && Boolean(operationId),
  });
  const [error, setError] = useState<string | null>(null);

  const uploadMutation = useMutation({
    mutationFn: (file: File) => ordersService.addOperationPhoto(orderId, operationId, file),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ordersKeys.operationPhotos(orderId, operationId),
      });
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Upload failed"),
  });

  const deleteMutation = useMutation({
    mutationFn: (photoId: string) =>
      ordersService.deleteOperationPhoto(orderId, operationId, photoId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ordersKeys.operationPhotos(orderId, operationId),
      });
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Delete failed"),
  });

  return (
    <DetailModal open={open} title={`Photos · ${title}`} onClose={onClose} width={520}>
      {isLoading ? (
        <div style={{ color: "#6B7280", fontSize: 13 }}>Loading…</div>
      ) : (
        <div style={{ display: "flex", flexWrap: "wrap", gap: 8, marginBottom: 14 }}>
          {data.length === 0 && (
            <div style={{ color: "#6B7280", fontSize: 13 }}>No photos yet.</div>
          )}
          {data.map((p) => (
            <div key={p.id} style={{ position: "relative" }}>
              <a href={mediaUrl(p.downloadUrl)} target="_blank" rel="noreferrer">
                <img
                  src={mediaUrl(p.downloadUrl)}
                  alt={p.fileName}
                  style={{
                    width: 88,
                    height: 88,
                    objectFit: "cover",
                    borderRadius: 6,
                    border: "1px solid #E5E7EB",
                    background: "#F3F4F6",
                  }}
                />
              </a>
              <button
                type="button"
                title="Delete"
                onClick={() => deleteMutation.mutate(p.id)}
                style={{
                  position: "absolute",
                  top: 4,
                  right: 4,
                  border: "none",
                  background: "rgba(0,0,0,.55)",
                  color: "#fff",
                  borderRadius: 4,
                  width: 22,
                  height: 22,
                  cursor: "pointer",
                  fontSize: 11,
                }}
              >
                🗑
              </button>
            </div>
          ))}
        </div>
      )}
      <input
        ref={inputRef}
        type="file"
        accept="image/*"
        hidden
        onChange={(e) => {
          const file = e.target.files?.[0];
          if (file) uploadMutation.mutate(file);
          e.target.value = "";
        }}
      />
      {error && <div style={{ color: "#DC2626", fontSize: 12, marginBottom: 8 }}>{error}</div>}
      <button
        type="button"
        className="btn btn-primary"
        disabled={uploadMutation.isPending}
        onClick={() => inputRef.current?.click()}
      >
        {uploadMutation.isPending ? "Uploading…" : "+ Upload photo"}
      </button>
    </DetailModal>
  );
}
