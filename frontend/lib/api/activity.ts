import apiClient from "./client";
import type { ActivityLog } from "@/lib/types/activity";

export async function getActivity(taskId: string): Promise<ActivityLog[]> {
  const { data } = await apiClient.get<ActivityLog[]>(`/tasks/${taskId}/activity`);
  return data;
}
