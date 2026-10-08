import apiClient from "./client";
import type { TaskComment, CreateCommentDto } from "@/lib/types/comment";

export async function getComments(taskId: string): Promise<TaskComment[]> {
  const { data } = await apiClient.get<TaskComment[]>(`/tasks/${taskId}/comments`);
  return data;
}

export async function addComment(taskId: string, dto: CreateCommentDto): Promise<TaskComment> {
  const { data } = await apiClient.post<TaskComment>(`/tasks/${taskId}/comments`, dto);
  return data;
}
