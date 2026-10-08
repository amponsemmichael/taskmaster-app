"use client";

import { useState } from "react";
import type { FilterParams } from "@/lib/types/task";
import * as tasksApi from "@/lib/api/tasks";
import { TASK_STATUSES, TASK_PRIORITIES, STATUS_LABELS, PRIORITY_LABELS } from "@/lib/constants";
import type { TaskItem } from "@/lib/types/task";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface TaskFiltersProps {
  onResults: (tasks: TaskItem[]) => void;
  onLoading: (loading: boolean) => void;
}

export function TaskFilters({ onResults, onLoading }: TaskFiltersProps) {
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [dueBefore, setDueBefore] = useState("");

  const handleFilter = async () => {
    const params: FilterParams = {};
    if (status) params.status = status;
    if (priority) params.priority = priority;
    if (dueBefore) params.dueBefore = new Date(dueBefore).toISOString();
    onLoading(true);
    try {
      const results = await tasksApi.filter(params);
      onResults(results);
    } finally {
      onLoading(false);
    }
  };

  const handleReset = async () => {
    setStatus("");
    setPriority("");
    setDueBefore("");
    onLoading(true);
    try {
      const results = await tasksApi.getAll();
      onResults(results);
    } finally {
      onLoading(false);
    }
  };

  return (
    <div className="flex flex-wrap items-end gap-3 rounded-lg border bg-gray-50 p-4">
      <div className="space-y-1">
        <Label htmlFor="filter-status">Status</Label>
        <select
          id="filter-status"
          value={status}
          onChange={(e) => setStatus(e.target.value)}
          className="h-9 rounded-md border border-input bg-white px-2 text-sm"
        >
          <option value="">All statuses</option>
          {TASK_STATUSES.map((s) => (
            <option key={s} value={s}>
              {STATUS_LABELS[s]}
            </option>
          ))}
        </select>
      </div>
      <div className="space-y-1">
        <Label htmlFor="filter-priority">Priority</Label>
        <select
          id="filter-priority"
          value={priority}
          onChange={(e) => setPriority(e.target.value)}
          className="h-9 rounded-md border border-input bg-white px-2 text-sm"
        >
          <option value="">All priorities</option>
          {TASK_PRIORITIES.map((p) => (
            <option key={p} value={p}>
              {PRIORITY_LABELS[p]}
            </option>
          ))}
        </select>
      </div>
      <div className="space-y-1">
        <Label htmlFor="filter-due">Due before</Label>
        <Input
          id="filter-due"
          type="date"
          value={dueBefore}
          onChange={(e) => setDueBefore(e.target.value)}
          className="w-36"
        />
      </div>
      <Button onClick={handleFilter} size="sm">
        Apply
      </Button>
      <Button onClick={handleReset} variant="outline" size="sm">
        Reset
      </Button>
    </div>
  );
}
