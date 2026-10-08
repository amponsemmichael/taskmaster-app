"use client";

import { useEffect, useState } from "react";
import * as activityApi from "@/lib/api/activity";
import type { ActivityLog as ActivityLogType } from "@/lib/types/activity";
import { formatRelative } from "@/lib/utils";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";

const actionLabels: Record<string, string> = {
  CREATED: "Created",
  UPDATED: "Updated",
  DELETED: "Deleted",
  ASSIGNED: "Assigned",
};

const actionColors: Record<string, string> = {
  CREATED: "text-green-700 bg-green-50",
  UPDATED: "text-blue-700 bg-blue-50",
  DELETED: "text-red-700 bg-red-50",
  ASSIGNED: "text-purple-700 bg-purple-50",
};

interface ActivityLogProps {
  taskId: string;
}

export function ActivityLog({ taskId }: ActivityLogProps) {
  const [logs, setLogs] = useState<ActivityLogType[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    activityApi
      .getActivity(taskId)
      .then(setLogs)
      .catch(() => setLogs([]))
      .finally(() => setLoading(false));
  }, [taskId]);

  if (loading) return <LoadingSpinner />;

  if (logs.length === 0) {
    return <p className="text-sm text-gray-400">No activity yet.</p>;
  }

  return (
    <ul className="space-y-2">
      {logs.map((log) => (
        <li key={log.id} className="flex items-start gap-3 text-sm">
          <span
            className={`shrink-0 rounded px-1.5 py-0.5 text-xs font-medium ${actionColors[log.action] ?? "text-gray-700 bg-gray-50"}`}
          >
            {actionLabels[log.action] ?? log.action}
          </span>
          <span className="text-gray-700">{log.description}</span>
          <span className="ml-auto shrink-0 text-xs text-gray-400">
            {formatRelative(log.createdAt)}
          </span>
        </li>
      ))}
    </ul>
  );
}
