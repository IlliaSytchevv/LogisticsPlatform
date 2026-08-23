import type { Metadata } from "next";
import { QueryProvider } from "@/components/providers/query-provider";
import "./globals.css";

export const metadata: Metadata = {
  title: "Freitty — Client Cabinet",
  description: "Freitty logistics client cabinet",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="uk">
      <body>
        <QueryProvider>{children}</QueryProvider>
      </body>
    </html>
  );
}
