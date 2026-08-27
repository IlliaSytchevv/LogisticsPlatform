"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { useState } from "react";
import { loginAction } from "@/actions/auth/login.action";
import { useAuthStore } from "@/lib/auth/auth-store";

export function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const setUser = useAuthStore((s) => s.setUser);

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [pending, setPending] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setPending(true);
    try {
      const result = await loginAction(username, password);
      if (!result.ok) {
        setError(result.message);
        return;
      }
      setUser(result.user);
      router.replace(searchParams.get("next") || "/dashboard");
      router.refresh();
    } catch {
      setError("Login failed. Check credentials.");
    } finally {
      setPending(false);
    }
  }

  return (
    <div className="auth-card">
      <div className="auth-brand">
        FREITT<span>Y</span>
      </div>
      <h1 className="auth-title">Welcome to Freitty</h1>
      <p className="auth-subtitle">Sign in to open your cabinet</p>

      <form className="auth-form" onSubmit={onSubmit}>
        <label className="auth-label">
          Username
          <input
            className="auth-input"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            autoComplete="username"
            required
          />
        </label>
        <label className="auth-label">
          Password
          <input
            className="auth-input"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="current-password"
            required
          />
        </label>

        {error ? <div className="auth-error">{error}</div> : null}

        <button type="submit" className="auth-submit" disabled={pending}>
          {pending ? "Signing in…" : "Sign in"}
        </button>
      </form>
    </div>
  );
}
