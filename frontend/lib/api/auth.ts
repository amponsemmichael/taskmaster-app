import apiClient from "./client";
import type { AuthResponse, LoginRequest, RegisterRequest } from "@/lib/types/auth";

export async function login(dto: LoginRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/auth/login", dto);
  return data;
}

export async function register(dto: RegisterRequest): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/auth/register", dto);
  return data;
}
