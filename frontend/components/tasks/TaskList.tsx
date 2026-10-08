import type { TaskItem } from "@/lib/types/task";
import { TaskCard } from "./TaskCard";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";

interface TaskListProps {
  tasks: TaskItem[];
  loading: boolean;
}

export function TaskList({ tasks, loading }: TaskListProps) {
  if (loading) return <LoadingSpinner />;

  if (tasks.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center py-16 text-gray-500">
        <p className="text-sm">No tasks found.</p>
      </div>
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {tasks.map((task) => (
        <TaskCard key={task.id} task={task} />
      ))}
    </div>
  );
}
