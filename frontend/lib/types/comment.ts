export interface TaskComment {
  id: string;
  taskItemId: string;
  userId: string;
  content: string;
  createdAt: string;
}

export interface CreateCommentDto {
  content: string;
}
