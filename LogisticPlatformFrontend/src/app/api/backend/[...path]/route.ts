import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";
import { refreshAccessTokenOnce } from "@/api/server/refresh-mutex";
import {
  ACCESS_TOKEN_COOKIE,
  REFRESH_TOKEN_COOKIE,
} from "@/lib/auth/constants";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5217";

type RouteContext = { params: Promise<{ path: string[] }> };

async function proxy(request: NextRequest, context: RouteContext, retried = false) {
  const { path } = await context.params;
  const targetPath = `/${path.map(encodeURIComponent).join("/")}`;
  const url = new URL(request.url);
  const target = `${API_URL}${targetPath}${url.search}`;

  const jar = await cookies();
  const access = jar.get(ACCESS_TOKEN_COOKIE)?.value;

  const headers = new Headers();
  const contentType = request.headers.get("content-type");
  if (contentType) headers.set("content-type", contentType);
  if (access) headers.set("Authorization", `Bearer ${access}`);

  const method = request.method.toUpperCase();
  const hasBody = method !== "GET" && method !== "HEAD";
  const body = hasBody ? await request.arrayBuffer() : undefined;

  const upstream = await fetch(target, {
    method,
    headers,
    body: body && body.byteLength > 0 ? body : undefined,
    cache: "no-store",
  });

  if (upstream.status === 401 && !retried) {
    const refresh = jar.get(REFRESH_TOKEN_COOKIE)?.value;
    if (refresh) {
      const ok = await refreshAccessTokenOnce(refresh);
      if (ok) {
        return proxy(request, context, true);
      }
    }
  }

  const responseHeaders = new Headers();
  const passThrough = ["content-type", "content-disposition", "content-length"];
  for (const key of passThrough) {
    const value = upstream.headers.get(key);
    if (value) responseHeaders.set(key, value);
  }

  return new NextResponse(upstream.body, {
    status: upstream.status,
    headers: responseHeaders,
  });
}

export async function GET(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function POST(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function PUT(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function PATCH(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function DELETE(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}
