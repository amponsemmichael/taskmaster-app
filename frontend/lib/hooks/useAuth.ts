import { useAuthContext } from "@/lib/context/AuthContext";

export function useAuth() {
  return useAuthContext();
}
