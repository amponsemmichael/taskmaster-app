import type { TaskComment } from "@/lib/types/comment";
import { formatRelative } from "@/lib/utils";

interface CommentListProps {
  comments: TaskComment[];
}

export function CommentList({ comments }: CommentListProps) {
  if (comments.length === 0) {
    return <p className="text-sm text-gray-400">No comments yet.</p>;
  }

  return (
    <ul className="space-y-3">
      {comments.map((c) => (
        <li key={c.id} className="rounded-md border bg-gray-50 p-3">
          <p className="text-sm text-gray-800">{c.content}</p>
          <p className="mt-1 text-xs text-gray-400">{formatRelative(c.createdAt)}</p>
        </li>
      ))}
    </ul>
  );
}
