"use client";

import { useTasks } from "@/lib/hooks/useTasks";
import { TaskList } from "@/components/tasks/TaskList";
import { ErrorMessage } from "@/components/shared/ErrorMessage";

export default function DashboardPage() {
  const { tasks, loading, error } = useTasks();

  const counts = {
    total: tasks.length,
    pending: tasks.filter((t) => t.status === "PENDING").length,
    inProgress: tasks.filter((t) => t.status === "IN_PROGRESS").length,
    completed: tasks.filter((t) => t.status === "COMPLETED").length,
  };

  const recentTasks = tasks.slice(0, 6);

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>

      {error && <ErrorMessage message={error} />}

      {/* Stats */}
      <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
        {[
          { label: "Total", value: counts.total, color: "text-gray-900" },
          { label: "Pending", value: counts.pending, color: "text-yellow-600" },
          { label: "In Progress", value: counts.inProgress, color: "text-blue-600" },
          { label: "Completed", value: counts.completed, color: "text-green-600" },
        ].map(({ label, value, color }) => (
          <div key={label} className="rounded-lg border bg-white p-4 shadow-sm">
            <p className="text-xs text-gray-500">{label}</p>
            <p className={`mt-1 text-3xl font-bold ${color}`}>{value}</p>
          </div>
        ))}
      </div>

      {/* Recent tasks */}
      <div>
        <h2 className="mb-4 text-lg font-semibold text-gray-800">Recent tasks</h2>
        <TaskList tasks={recentTasks} loading={loading} />
      </div>
    </div>
  );
}
