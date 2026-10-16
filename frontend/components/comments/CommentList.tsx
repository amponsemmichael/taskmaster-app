"use client";

import { useEffect, useState } from "react";
import * as commentsApi from "@/lib/api/comments";
import type { TaskComment } from "@/lib/types/comment";
import { formatRelative } from "@/lib/utils";
import { ErrorMessage } from "@/components/shared/ErrorMessage";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";

interface CommentListProps {
  taskId: string;
  refreshKey: number;
}

interface CommentResult {
  requestKey: string;
  comments: TaskComment[];
  error: string | null;
}

export function CommentList({ taskId, refreshKey }: CommentListProps) {
  const requestKey = `${taskId}:${refreshKey}`;
  const [result, setResult] = useState<CommentResult | null>(null);

  useEffect(() => {
    let cancelled = false;
    commentsApi
      .getComments(taskId)
      .then((result) => {
        if (!cancelled) {
          setResult({ requestKey, comments: result, error: null });
        }
      })
      .catch(() => {
        if (!cancelled) {
          setResult({
            requestKey,
            comments: [],
            error: "Unable to load comments.",
          });
        }
      });

    return () => {
      cancelled = true;
    };
  }, [taskId, requestKey]);

  if (!result || result.requestKey !== requestKey) return <LoadingSpinner />;
  if (result.error) return <ErrorMessage message={result.error} />;
  const { comments } = result;
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
