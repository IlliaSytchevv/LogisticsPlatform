"use client";

import { useEffect, useState } from "react";
import { getSessionAction } from "@/actions/auth/get-session.action";
import type { AuthUser } from "@/types/auth";

export function useSession() {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    void getSessionAction().then((session) => {
      if (!cancelled) {
        setUser(session);
        setLoading(false);
      }
    });
    return () => {
      cancelled = true;
    };
  }, []);

  const roles = user?.roles ?? [];
  const isAdmin = roles.some((r) => r.toLowerCase() === "admin");

  return { user, loading, roles, isAdmin };
}
