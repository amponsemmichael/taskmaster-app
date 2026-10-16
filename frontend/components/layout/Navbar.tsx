"use client";

import Link from "next/link";
import toast from "react-hot-toast";
import { useAuth } from "@/lib/hooks/useAuth";
import { Button } from "@/components/ui/button";

export function Navbar() {
  const { user, logout } = useAuth();

  return (
    <header className="flex h-14 items-center justify-between border-b bg-white px-6">
      <Link href="/dashboard" className="text-lg font-semibold text-gray-900">
        TaskMaster
      </Link>
      <div className="flex items-center gap-4">
        {user && (
          <span className="text-sm text-gray-600">{user.email}</span>
        )}
        <Button
          variant="outline"
          size="sm"
          onClick={() =>
            void logout().catch(() =>
              toast.error(
                "Unable to revoke the server session. You have been signed out on this device."
              )
            )
          }
        >
          Logout
        </Button>
      </div>
    </header>
  );
}
