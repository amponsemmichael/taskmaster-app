"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/lib/hooks/useAuth";
import { LoginForm } from "@/components/auth/LoinForm";

export default function LoginPage() {
  const { token, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && token) {
      router.replace("/dashboard");
    }
  }, [token, isLoading, router]);

  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50">
      <div className="w-full max-w-sm rounded-xl border bg-white p-8 shadow-sm">
        <h1 className="mb-6 text-xl font-bold text-gray-900">Sign in to TaskMaster</h1>
        <LoginForm />
        <p className="mt-4 text-center text-sm text-gray-500">
          No account?{" "}
          <a href="/register" className="font-medium text-blue-600 hover:underline">
            Register
          </a>
        </p>
      </div>
    </div>
  );
}
