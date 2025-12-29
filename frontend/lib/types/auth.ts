export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
}

export interface DecodedToken {
  nameid: string;
  email: string;
  role: string;
  exp: number;
}