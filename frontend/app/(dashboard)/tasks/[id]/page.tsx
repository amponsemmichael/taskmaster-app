"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import * as tasksApi from "@/lib/api/tasks";
import type { TaskItem } from "@/lib/types/task";
import { TaskDetails } from "@/components/tasks/TaskDetails";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { ErrorMessage } from "@/components/shared/ErrorMessage";

export default function TaskDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [task, setTask] = useState<TaskItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    tasksApi
      .getById(id)
      .then(setTask)
      .catch(() => setError("Task not found."))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message={error} />;
  if (!task) return null;

  return (
    <div className="max-w-3xl">
      <TaskDetails task={task} onTaskUpdated={setTask} />
    </div>
  );
}
