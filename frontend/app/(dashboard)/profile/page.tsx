"use client";

import { useEffect, useState } from "react";
import { ErrorMessage } from "@/components/shared/ErrorMessage";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { useAuth } from "@/lib/hooks/useAuth";
import * as usersApi from "@/lib/api/users";
import type { User } from "@/lib/types/user";

export default function ProfilePage() {
  const { user } = useAuth();
  const [profile, setProfile] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!user?.id) return;

    let cancelled = false;
    usersApi
      .getById(user.id)
      .then((result) => {
        if (!cancelled) setProfile(result);
      })
      .catch(() => {
        if (!cancelled) setError("Unable to load your profile.");
      });

    return () => {
      cancelled = true;
    };
  }, [user?.id]);

  if (!user?.id) {
    return <ErrorMessage message="Unable to identify the signed-in user." />;
  }
  if (!profile && !error) return <LoadingSpinner />;
  if (error) return <ErrorMessage message={error} />;
  if (!profile) return <ErrorMessage message="Profile information is unavailable." />;

  return (
    <section className="max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Your profile</h1>
        <p className="mt-1 text-sm text-gray-500">Account information</p>
      </div>
      <dl className="divide-y rounded-lg border bg-white">
        <div className="grid gap-1 p-4 sm:grid-cols-[10rem_1fr] sm:gap-4">
          <dt className="text-sm font-medium text-gray-500">Email</dt>
          <dd className="text-sm text-gray-900">{profile.email}</dd>
        </div>
        <div className="grid gap-1 p-4 sm:grid-cols-[10rem_1fr] sm:gap-4">
          <dt className="text-sm font-medium text-gray-500">Role</dt>
          <dd className="text-sm text-gray-900">{profile.role}</dd>
        </div>
        <div className="grid gap-1 p-4 sm:grid-cols-[10rem_1fr] sm:gap-4">
          <dt className="text-sm font-medium text-gray-500">Member since</dt>
          <dd className="text-sm text-gray-900">
            {new Date(profile.createdAt).toLocaleDateString()}
          </dd>
        </div>
      </dl>
    </section>
  );
}
