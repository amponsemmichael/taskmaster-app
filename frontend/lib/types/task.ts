export type TaskStatus = "PENDING" | "IN_PROGRESS" | "COMPLETED";
export type TaskPriority = "LOW" | "MEDIUM" | "HIGH";

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  dueDate: string | null;
  priority: TaskPriority;
  status: TaskStatus;
  assignedToUserId: string | null;
  createdByUserId: string;
  createdAt: string;
  comments: import("./comment").TaskComment[];
}

export interface CreateTaskDto {
  title: string;
  description: string;
  dueDate?: string | null;
  priority: TaskPriority;
  status: TaskStatus;
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  dueDate?: string | null;
  priority?: TaskPriority;
  status?: TaskStatus;
}

export interface FilterParams {
  status?: string;
  priority?: string;
  dueBefore?: string;
  page?: number;
  pageSize?: number;
}
