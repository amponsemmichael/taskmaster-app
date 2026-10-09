"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import toast from "react-hot-toast";
import { commentSchema, type CommentFormValues } from "@/lib/utils/validator";
import * as commentsApi from "@/lib/api/comments";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";

interface CommentFormProps {
  taskId: string;
  onAdded: () => void;
}

export function CommentForm({ taskId, onAdded }: CommentFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CommentFormValues>({
    resolver: zodResolver(commentSchema),
  });

  const onSubmit = async (values: CommentFormValues) => {
    setIsSubmitting(true);
    try {
      await commentsApi.addComment(taskId, values);
      toast.success("Comment added.");
      reset();
      onAdded();
    } catch {
      toast.error("Failed to add comment.");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-2">
      <Textarea
        placeholder="Write a comment…"
        rows={3}
        {...register("content")}
      />
      {errors.content && (
        <p className="text-xs text-red-600">{errors.content.message}</p>
      )}
      <Button type="submit" size="sm" disabled={isSubmitting}>
        {isSubmitting ? "Posting…" : "Post comment"}
      </Button>
    </form>
  );
}
