"use client";

import React, {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import * as authApi from "@/lib/api/auth";
import type { LoginRequest, RegisterRequest } from "@/lib/types/auth";
import type { User } from "@/lib/types/user";

interface JwtPayload {
  nameid?: string;
  email?: string;
  role?: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"?: string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"?: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string;
  exp?: number;
}

function decodeJwt(token: string): JwtPayload | null {
  try {
    const payload = token.split(".")[1]?.replace(/-/g, "+").replace(/_/g, "/");
    if (!payload) return null;
    const decoded = atob(payload.padEnd(Math.ceil(payload.length / 4) * 4, "="));
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

function buildUserFromToken(token: string): User | null {
  const payload = decodeJwt(token);
  const id =
    payload?.nameid ??
    payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
  const email =
    payload?.email ??
    payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
  if (!id || !email) return null;
  return {
    id,
    email,
    role:
      (payload?.role ??
        payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]) ===
        "ADMIN"
        ? "ADMIN"
        : "USER",
    createdAt: "",
  };
}

interface AuthContextValue {
  user: User | null;
  token: string | null;
  isAdmin: boolean;
  isLoading: boolean;
  login: (dto: LoginRequest) => Promise<void>;
  register: (dto: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = localStorage.getItem("accessToken");
    const storedUser = storedToken ? buildUserFromToken(storedToken) : null;

    if (storedToken && !storedUser) {
      localStorage.removeItem("accessToken");
    } else if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(storedUser);
    }

    setIsLoading(false);
  }, []);

  const login = useCallback(async (dto: LoginRequest) => {
    const tokens = await authApi.login(dto);
    const authenticatedUser = buildUserFromToken(tokens.accessToken);
    if (!authenticatedUser) throw new Error("The server returned an invalid access token.");
    localStorage.setItem("accessToken", tokens.accessToken);
    localStorage.setItem("refreshToken", tokens.refreshToken);
    setToken(tokens.accessToken);
    setUser(authenticatedUser);
  }, []);

  const register = useCallback(async (dto: RegisterRequest) => {
    const tokens = await authApi.register(dto);
    const authenticatedUser = buildUserFromToken(tokens.accessToken);
    if (!authenticatedUser) throw new Error("The server returned an invalid access token.");
    localStorage.setItem("accessToken", tokens.accessToken);
    localStorage.setItem("refreshToken", tokens.refreshToken);
    setToken(tokens.accessToken);
    setUser(authenticatedUser);
  }, []);

  const logout = useCallback(async () => {
    const refreshToken = localStorage.getItem("refreshToken");
    try {
      if (refreshToken) await authApi.logout(refreshToken);
    } finally {
      localStorage.removeItem("accessToken");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("token");
      setToken(null);
      setUser(null);
    }
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      token,
      isAdmin: user?.role === "ADMIN",
      isLoading,
      login,
      register,
      logout,
    }),
    [user, token, isLoading, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuthContext(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuthContext must be used within AuthProvider");
  return ctx;
}
