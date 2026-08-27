"use client";

import { useRef, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { mediaUrl, ordersService } from "@/api/services/orders.service";
import type { OrderPhoto } from "@/types/orders";
import { useSession } from "@/hooks/use-session";
import { ordersKeys } from "../../_hooks/orders-queries";
import { DetailModal } from "./detail-modal";

type Props = {
  orderId: string;
  photos: OrderPhoto[];
  open: boolean;
  onClose: () => void;
};

export function WarehousePhotosPanel({ orderId, photos, open, onClose }: Props) {
  const queryClient = useQueryClient();
  const { canWrite } = useSession();
  const inputRef = useRef<HTMLInputElement>(null);
  const [error, setError] = useState<string | null>(null);

  const uploadMutation = useMutation({
    mutationFn: (file: File) => ordersService.addWarehousePhoto(orderId, file),
    onSuccess: async () => {
      setError(null);
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Upload failed"),
  });

  const deleteMutation = useMutation({
    mutationFn: (photoId: string) => ordersService.deleteWarehousePhoto(orderId, photoId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ordersKeys.detail(orderId) });
    },
    onError: (err) => setError(err instanceof Error ? err.message : "Delete failed"),
  });

  return (
    <DetailModal open={open} title={`Warehouse photos (${photos.length})`} onClose={onClose} width={520}>
      <div style={{ display: "flex", flexWrap: "wrap", gap: 8, marginBottom: 14 }}>
        {photos.length === 0 && (
          <div style={{ color: "#6B7280", fontSize: 13 }}>No warehouse photos yet.</div>
        )}
        {photos.map((p) => (
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
            {canWrite ? (
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
            ) : null}
          </div>
        ))}
      </div>

      {canWrite ? (
        <>
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
        </>
      ) : null}
    </DetailModal>
  );
}
