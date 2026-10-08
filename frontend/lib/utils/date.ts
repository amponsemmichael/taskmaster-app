import { format, formatDistanceToNow, isPast, parseISO } from "date-fns";

export function formatDate(date: string | null | undefined): string {
  if (!date) return "No due date";
  return format(parseISO(date), "MMM d, yyyy");
}

export function formatRelative(date: string | null | undefined): string {
  if (!date) return "";
  return formatDistanceToNow(parseISO(date), { addSuffix: true });
}

export function isOverdue(date: string | null | undefined): boolean {
  if (!date) return false;
  return isPast(parseISO(date));
}
