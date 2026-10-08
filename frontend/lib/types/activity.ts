export interface ActivityLog {
  id: string;
  taskItemId: string;
  userId: string;
  action: "CREATED" | "UPDATED" | "DELETED" | "ASSIGNED";
  description: string;
  createdAt: string;
}
