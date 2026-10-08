"use client";

import { useEffect, useRef, useState } from "react";
import * as tasksApi from "@/lib/api/tasks";
import type { TaskItem } from "@/lib/types/task";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/lib/hooks/useAuth";

interface TaskSearchProps {
  onResults: (tasks: TaskItem[]) => void;
  onLoading: (loading: boolean) => void;
}

export function TaskSearch({ onResults, onLoading }: TaskSearchProps) {
  const [query, setQuery] = useState("");
  const [myTasks, setMyTasks] = useState(false);
  const { user } = useAuth();
  const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);

    if (!query.trim()) return;

    debounceRef.current = setTimeout(async () => {
      onLoading(true);
      try {
        const results = await tasksApi.search(query.trim(), myTasks);
        onResults(results);
      } finally {
        onLoading(false);
      }
    }, 400);

    return () => {
      if (debounceRef.current) clearTimeout(debounceRef.current);
    };
  }, [query, myTasks, onResults, onLoading]);

  return (
    <div className="flex items-center gap-3">
      <Input
        type="search"
        placeholder="Search tasks…"
        value={query}
        onChange={(e) => setQuery(e.target.value)}
        className="max-w-xs"
      />
      {user && (
        <label className="flex items-center gap-1.5 text-sm text-gray-600">
          <input
            type="checkbox"
            checked={myTasks}
            onChange={(e) => setMyTasks(e.target.checked)}
            className="h-4 w-4 rounded border-gray-300"
          />
          My tasks
        </label>
      )}
    </div>
  );
}
