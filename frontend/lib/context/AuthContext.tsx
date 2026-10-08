"use client";

import React, {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
} from "react";
import * as authApi from "@/lib/api/auth";
import type { LoginRequest, RegisterRequest } from "@/lib/types/auth";
import type { User } from "@/lib/types/user";

interface JwtPayload {
  nameid: string;
  email: string;
  role: string;
  exp: number;
}

function decodeJwt(token: string): JwtPayload | null {
  try {
    const payload = token.split(".")[1];
    const decoded = JSON.parse(atob(payload));
    return decoded as JwtPayload;
  } catch {
    return null;
  }
}

function buildUserFromToken(token: string): User | null {
  const payload = decodeJwt(token);
  if (!payload) return null;
  return {
    id: payload.nameid,
    email: payload.email,
    role: (payload.role as "USER" | "ADMIN") ?? "USER",
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
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

function initFromStorage<T>(key: string, transform: (raw: string) => T | null): T | null {
  if (typeof window === "undefined") return null;
  const raw = localStorage.getItem(key);
  if (!raw) return null;
  const result = transform(raw);
  if (!result) {
    localStorage.removeItem(key);
    return null;
  }
  return result;
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(() =>
    initFromStorage("token", (t) => (buildUserFromToken(t) ? t : null))
  );
  const [user, setUser] = useState<User | null>(() =>
    initFromStorage("token", buildUserFromToken)
  );
  const [isLoading] = useState(false);

  const login = useCallback(async (dto: LoginRequest) => {
    const { token: t } = await authApi.login(dto);
    localStorage.setItem("token", t);
    setToken(t);
    setUser(buildUserFromToken(t));
  }, []);

  const register = useCallback(async (dto: RegisterRequest) => {
    const { token: t } = await authApi.register(dto);
    localStorage.setItem("token", t);
    setToken(t);
    setUser(buildUserFromToken(t));
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem("token");
    setToken(null);
    setUser(null);
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
