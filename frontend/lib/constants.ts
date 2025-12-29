import { Priority, Status } from './types/task';

export const PRIORITY_OPTIONS: { value: Priority; label: string; color: string }[] = [
  { value: 'LOW', label: 'Low', color: 'bg-blue-100 text-blue-800' },
  { value: 'MEDIUM', label: 'Medium', color: 'bg-yellow-100 text-yellow-800' },
  { value: 'HIGH', label: 'High', color: 'bg-red-100 text-red-800' },
];

export const STATUS_OPTIONS: { value: Status; label: string; color: string }[] = [
  { value: 'PENDING', label: 'Pending', color: 'bg-gray-100 text-gray-800' },
  { value: 'IN_PROGRESS', label: 'In Progress', color: 'bg-blue-100 text-blue-800' },
  { value: 'COMPLETED', label: 'Completed', color: 'bg-green-100 text-green-800' },
];

export const getPriorityColor = (priority: Priority) => {
  return PRIORITY_OPTIONS.find((p) => p.value === priority)?.color || '';
};

export const getStatusColor = (status: Status) => {
  return STATUS_OPTIONS.find((s) => s.value === status)?.color || '';
};