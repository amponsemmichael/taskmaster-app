"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import type { TaskItem } from "@/lib/types/task";
import type { CreateTaskFormValues } from "@/lib/utils/validator";
import * as tasksApi from "@/lib/api/tasks";
import { useAuth } from "@/lib/hooks/useAuth";
import { useUsers } from "@/lib/hooks/useUsers";
import { formatDate, isOverdue, cn } from "@/lib/utils";
import { STATUS_LABELS, PRIORITY_LABELS } from "@/lib/constants";
import { TaskForm } from "./TaskForm";
import { CommentList } from "@/components/comments/CommentList";
import { CommentForm } from "@/components/comments/CommentForm";
import { ActivityLog } from "@/components/activity/ActivityLog";
import { Button } from "@/components/ui/button";
import { Separator } from "@/components/ui/separator";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";

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

interface TaskDetailsProps {
  task: TaskItem;
  onTaskUpdated: (task: TaskItem) => void;
}

export function TaskDetails({ task, onTaskUpdated }: TaskDetailsProps) {
  const { isAdmin } = useAuth();
  const { users } = useUsers();
  const router = useRouter();
  const [isEditing, setIsEditing] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [assignUserId, setAssignUserId] = useState(task.assignedToUserId ?? "");
  const [isAssigning, setIsAssigning] = useState(false);
  const [commentRefreshKey, setCommentRefreshKey] = useState(0);

  const overdue = task.status !== "COMPLETED" && isOverdue(task.dueDate);

  const handleUpdate = async (values: CreateTaskFormValues) => {
    setIsSubmitting(true);
    try {
      const updated = await tasksApi.update(task.id, {
        ...values,
        dueDate: values.dueDate ? new Date(values.dueDate).toISOString() : null,
      });
      onTaskUpdated(updated);
      setIsEditing(false);
      toast.success("Task updated.");
    } catch {
      toast.error("Failed to update task.");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!confirm("Delete this task?")) return;
    try {
      await tasksApi.remove(task.id);
      toast.success("Task deleted.");
      router.push("/tasks");
    } catch {
      toast.error("Failed to delete task.");
    }
  };

  const handleAssign = async () => {
    if (!assignUserId) return;
    setIsAssigning(true);
    try {
      await tasksApi.assign(task.id, assignUserId);
      toast.success("Task assigned.");
    } catch {
      toast.error("Failed to assign task.");
    } finally {
      setIsAssigning(false);
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-start justify-between gap-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold text-gray-900">{task.title}</h1>
          <div className="flex items-center gap-2 flex-wrap">
            <span
              className={cn(
                "rounded-full px-2.5 py-0.5 text-xs font-medium",
                statusColors[task.status] ?? "bg-gray-100 text-gray-700"
              )}
            >
              {STATUS_LABELS[task.status] ?? task.status}
            </span>
            <span
              className={cn(
                "rounded-full px-2.5 py-0.5 text-xs font-medium",
                priorityColors[task.priority] ?? "bg-gray-100 text-gray-700"
              )}
            >
              {PRIORITY_LABELS[task.priority] ?? task.priority}
            </span>
            {task.dueDate && (
              <span className={cn("text-xs text-gray-500", overdue && "font-medium text-red-600")}>
                Due: {formatDate(task.dueDate)}{overdue && " · Overdue"}
              </span>
            )}
          </div>
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => setIsEditing((v) => !v)}
          >
            {isEditing ? "Cancel" : "Edit"}
          </Button>
          <Button variant="destructive" size="sm" onClick={handleDelete}>
            Delete
          </Button>
        </div>
      </div>

      {task.description && !isEditing && (
        <p className="text-sm text-gray-700">{task.description}</p>
      )}

      {isEditing && (
        <div className="rounded-lg border p-4">
          <TaskForm task={task} onSubmit={handleUpdate} isSubmitting={isSubmitting} />
        </div>
      )}

      {/* Admin: assign user */}
      {isAdmin && !isEditing && (
        <div className="flex items-end gap-3 rounded-lg border bg-gray-50 p-4">
          <div className="space-y-1">
            <label className="text-sm font-medium text-gray-700">Assign to</label>
            <select
              value={assignUserId}
              onChange={(e) => setAssignUserId(e.target.value)}
              className="h-9 rounded-md border border-input bg-white px-2 text-sm"
            >
              <option value="">Select a user</option>
              {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.email}
                </option>
              ))}
            </select>
          </div>
          <Button size="sm" onClick={handleAssign} disabled={isAssigning || !assignUserId}>
            {isAssigning ? "Assigning…" : "Assign"}
          </Button>
        </div>
      )}

      <Separator />

      {/* Tabs: Comments + Activity */}
      <Tabs defaultValue="comments">
        <TabsList>
          <TabsTrigger value="comments">Comments</TabsTrigger>
          <TabsTrigger value="activity">Activity</TabsTrigger>
        </TabsList>
        <TabsContent value="comments" className="space-y-4 pt-4">
          <CommentList taskId={task.id} refreshKey={commentRefreshKey} />
          <CommentForm
            taskId={task.id}
            onAdded={() => setCommentRefreshKey((key) => key + 1)}
          />
        </TabsContent>
        <TabsContent value="activity" className="pt-4">
          <ActivityLog taskId={task.id} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
