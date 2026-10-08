import Link from "next/link";
import type { TaskItem } from "@/lib/types/task";
import { cn, formatDate, isOverdue } from "@/lib/utils";
import { STATUS_LABELS, PRIORITY_LABELS } from "@/lib/constants";

const statusColors: Record<string, string> = {
  PENDING: "bg-yellow-100 text-yellow-800",
  IN_PROGRESS: "bg-blue-100 text-blue-800",
  COMPLETED: "bg-green-100 text-green-800",
};

const priorityColors: Record<string, string> = {
  LOW: "bg-gray-100 text-gray-700",
  MEDIUM: "bg-orange-100 text-orange-700",
  HIGH: "bg-red-100 text-red-700",
};

interface TaskCardProps {
  task: TaskItem;
}

export function TaskCard({ task }: TaskCardProps) {
  const overdue = task.status !== "COMPLETED" && isOverdue(task.dueDate);

  return (
    <Link href={`/tasks/${task.id}`}>
      <div className="flex cursor-pointer flex-col gap-2 rounded-lg border bg-white p-4 shadow-sm transition-shadow hover:shadow-md">
        <div className="flex items-start justify-between gap-2">
          <h3 className="text-sm font-semibold text-gray-900 line-clamp-2">{task.title}</h3>
          <span
            className={cn(
              "shrink-0 rounded-full px-2 py-0.5 text-xs font-medium",
              statusColors[task.status] ?? "bg-gray-100 text-gray-700"
            )}
          >
            {STATUS_LABELS[task.status] ?? task.status}
          </span>
        </div>
        {task.description && (
          <p className="text-xs text-gray-500 line-clamp-2">{task.description}</p>
        )}
        <div className="flex items-center justify-between text-xs">
          <span
            className={cn(
              "rounded-full px-2 py-0.5 font-medium",
              priorityColors[task.priority] ?? "bg-gray-100 text-gray-700"
            )}
          >
            {PRIORITY_LABELS[task.priority] ?? task.priority}
          </span>
          <span className={cn("text-gray-500", overdue && "font-medium text-red-600")}>
            {formatDate(task.dueDate)}
            {overdue && " · Overdue"}
          </span>
        </div>
      </div>
    </Link>
  );
}
