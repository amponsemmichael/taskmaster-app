"use client";

import { useCallback, useState } from "react";
import Link from "next/link";
import { useTasks } from "@/lib/hooks/useTasks";
import type { TaskItem } from "@/lib/types/task";
import { TaskList } from "@/components/tasks/TaskList";
import { TaskSearch } from "@/components/tasks/TaskSearch";
import { TaskFilters } from "@/components/tasks/TaskFilters";
import { ErrorMessage } from "@/components/shared/ErrorMessage";
import { Button } from "@/components/ui/button";

export default function TasksPage() {
  const { tasks: allTasks, loading: defaultLoading, error } = useTasks();
  const [filteredTasks, setFilteredTasks] = useState<TaskItem[] | null>(null);
  const [filterLoading, setFilterLoading] = useState(false);

  const displayTasks = filteredTasks ?? allTasks;
  const loading = filterLoading || (filteredTasks === null && defaultLoading);

  const handleResults = useCallback((tasks: TaskItem[]) => {
    setFilteredTasks(tasks);
  }, []);

  const handleLoading = useCallback((v: boolean) => {
    setFilterLoading(v);
  }, []);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">Tasks</h1>
        <Button asChild>
          <Link href="/tasks/new">+ New task</Link>
        </Button>
      </div>

      {error && <ErrorMessage message={error} />}

      <div className="flex flex-col gap-4">
        <TaskSearch onResults={handleResults} onLoading={handleLoading} />
        <TaskFilters onResults={handleResults} onLoading={handleLoading} />
      </div>

      <TaskList tasks={displayTasks} loading={loading} />
    </div>
  );
}
