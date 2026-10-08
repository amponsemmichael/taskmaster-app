"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import toast from "react-hot-toast";
import * as tasksApi from "@/lib/api/tasks";
import type { CreateTaskFormValues } from "@/lib/utils/validator";
import { TaskForm } from "@/components/tasks/TaskForm";

export default function NewTaskPage() {
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (values: CreateTaskFormValues) => {
    setIsSubmitting(true);
    try {
      await tasksApi.create({
        ...values,
        dueDate: values.dueDate ? new Date(values.dueDate).toISOString() : null,
      });
      toast.success("Task created!");
      router.push("/tasks");
    } catch {
      toast.error("Failed to create task.");
      setIsSubmitting(false);
    }
  };

  return (
    <div className="max-w-xl space-y-6">
      <h1 className="text-2xl font-bold text-gray-900">New task</h1>
      <div className="rounded-lg border bg-white p-6 shadow-sm">
        <TaskForm onSubmit={handleSubmit} isSubmitting={isSubmitting} />
      </div>
    </div>
  );
}
