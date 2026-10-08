import apiClient from "./client";
import type { TaskItem, CreateTaskDto, UpdateTaskDto, FilterParams } from "@/lib/types/task";

export async function getAll(): Promise<TaskItem[]> {
  const { data } = await apiClient.get<TaskItem[]>("/tasks");
  return data;
}

export async function getById(id: string): Promise<TaskItem> {
  const { data } = await apiClient.get<TaskItem>(`/tasks/${id}`);
  return data;
}

export async function create(dto: CreateTaskDto): Promise<TaskItem> {
  const { data } = await apiClient.post<TaskItem>("/tasks", dto);
  return data;
}

export async function update(id: string, dto: UpdateTaskDto): Promise<TaskItem> {
  const { data } = await apiClient.put<TaskItem>(`/tasks/${id}`, dto);
  return data;
}

export async function remove(id: string): Promise<void> {
  await apiClient.delete(`/tasks/${id}`);
}

export async function assign(id: string, userId: string): Promise<void> {
  await apiClient.put(`/tasks/${id}/assign`, { userId });
}

export async function filter(params: FilterParams): Promise<TaskItem[]> {
  const { data } = await apiClient.get<TaskItem[]>("/tasks/filter", { params });
  return data;
}

export async function search(q: string, myTasks = false): Promise<TaskItem[]> {
  const { data } = await apiClient.get<TaskItem[]>("/tasks/search", {
    params: { q, myTasks },
  });
  return data;
}
