"use client";

import { useEffect, useState } from "react";
import * as usersApi from "@/lib/api/users";
import type { User } from "@/lib/types/user";

export function useUsers() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    usersApi
      .getAll()
      .then(setUsers)
      .catch(() => setUsers([]))
      .finally(() => setLoading(false));
  }, []);

  return { users, loading };
}
