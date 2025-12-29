import { apiClient } from './client';
import { Comment, CreateCommentRequest, ActivityLog } from '../types/comment';

export const commentsApi = {
  getByTaskId: async (taskId: string): Promise<Comment[]> => {
    const response = await apiClient.get<Comment[]>(`/tasks/${taskId}/comments`);
    return response.data;
  },

  create: async (taskId: string, data: CreateCommentRequest): Promise<void> => {
    await apiClient.post(`/tasks/${taskId}/comments`, data);
  },
};

export const activityApi = {
  getByTaskId: async (taskId: string): Promise<ActivityLog[]> => {
    const response = await apiClient.get<ActivityLog[]>(`/tasks/${taskId}/activity`);
    return response.data;
  },
};