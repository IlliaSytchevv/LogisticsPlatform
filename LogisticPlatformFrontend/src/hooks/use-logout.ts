"use client";

import { useRouter } from "next/navigation";
import { useTransition } from "react";
import { logoutAction } from "@/actions/auth/logout.action";
import { useAuthStore } from "@/lib/auth/auth-store";

export function useLogout() {
  const router = useRouter();
  const setUser = useAuthStore((s) => s.setUser);
  const [pending, startTransition] = useTransition();

  function logout() {
    startTransition(async () => {
      setUser(null);
      try {
        await logoutAction();
      } catch {
        // redirect() throws; ignore
      }
      router.replace("/login");
      router.refresh();
    });
  }

  return { logout, pending };
}
