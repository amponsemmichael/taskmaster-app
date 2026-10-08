"use client";

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { createTaskSchema, type CreateTaskFormValues } from "@/lib/utils/validator";
import { TASK_STATUSES, TASK_PRIORITIES, STATUS_LABELS, PRIORITY_LABELS } from "@/lib/constants";
import type { TaskItem } from "@/lib/types/task";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

interface TaskFormProps {
  task?: TaskItem;
  onSubmit: (values: CreateTaskFormValues) => Promise<void>;
  isSubmitting?: boolean;
}

export function TaskForm({ task, onSubmit, isSubmitting }: TaskFormProps) {
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateTaskFormValues>({
    resolver: zodResolver(createTaskSchema),
    defaultValues: {
      title: task?.title ?? "",
      description: task?.description ?? "",
      dueDate: task?.dueDate ? task.dueDate.slice(0, 10) : "",
      priority: task?.priority ?? "MEDIUM",
      status: task?.status ?? "PENDING",
    },
  });

  useEffect(() => {
    if (task) {
      reset({
        title: task.title,
        description: task.description,
        dueDate: task.dueDate ? task.dueDate.slice(0, 10) : "",
        priority: task.priority,
        status: task.status,
      });
    }
  }, [task, reset]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="space-y-1">
        <Label htmlFor="title">Title *</Label>
        <Input id="title" placeholder="Task title" {...register("title")} />
        {errors.title && (
          <p className="text-xs text-red-600">{errors.title.message}</p>
        )}
      </div>
      <div className="space-y-1">
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          placeholder="Optional description"
          rows={3}
          {...register("description")}
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-1">
          <Label htmlFor="priority">Priority</Label>
          <select
            id="priority"
            {...register("priority")}
            className="h-9 w-full rounded-md border border-input bg-white px-2 text-sm"
          >
            {TASK_PRIORITIES.map((p) => (
              <option key={p} value={p}>
                {PRIORITY_LABELS[p]}
              </option>
            ))}
          </select>
          {errors.priority && (
            <p className="text-xs text-red-600">{errors.priority.message}</p>
          )}
        </div>
        <div className="space-y-1">
          <Label htmlFor="status">Status</Label>
          <select
            id="status"
            {...register("status")}
            className="h-9 w-full rounded-md border border-input bg-white px-2 text-sm"
          >
            {TASK_STATUSES.map((s) => (
              <option key={s} value={s}>
                {STATUS_LABELS[s]}
              </option>
            ))}
          </select>
          {errors.status && (
            <p className="text-xs text-red-600">{errors.status.message}</p>
          )}
        </div>
      </div>
      <div className="space-y-1">
        <Label htmlFor="dueDate">Due date</Label>
        <Input id="dueDate" type="date" {...register("dueDate")} className="w-48" />
      </div>
      <Button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving…" : task ? "Update task" : "Create task"}
      </Button>
    </form>
  );
}
