export interface Comment {
  id: string;
  content: string;
  userId: string;
  createdAt: string;
}

export interface CreateCommentRequest {
  content: string;
}

export interface ActivityLog {
  id: string;
  action: string;
  description: string;
  userId: string;
  createdAt: string;
}