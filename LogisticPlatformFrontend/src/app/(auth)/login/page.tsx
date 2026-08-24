import { Suspense } from "react";
import { LoginForm } from "./_components/login-form";

export default function LoginPage() {
  return (
    <Suspense fallback={<div className="auth-subtitle">Loading…</div>}>
      <LoginForm />
    </Suspense>
  );
}
