// components/WorkflowDiagram.tsx
import React, { useState } from "react";
import StateItem from "./StateItem";

interface WorkflowDiagramProps {
  states: string[];
  workflowId: number;
  onTransitionAdded: (from: string, to: string, isFinal: boolean) => void;
}

export default function WorkflowDiagram({ states, workflowId, onTransitionAdded }: WorkflowDiagramProps) {
  const [selectedState, setSelectedState] = useState<string | null>(null);

  const handleTransition = async (toState: string) => {
    if (selectedState && toState !== selectedState) {
      const isFinal = toState === states[states.length - 1];
      await fetch(`/api/processes/${workflowId}/transitions`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          currentStateName: selectedState,
          nextStateName: toState,
          isFinal,
        }),
      });
      onTransitionAdded(selectedState, toState, isFinal);
      setSelectedState(toState);
    }
  };

  return (
    <div className="space-y-2">
      {states.map((state) => (
        <div key={state} className="flex items-center space-x-2">
          <StateItem id={state} isCurrent={false}/>
          <select
            value={selectedState === state ? "selected" : ""}
            onChange={(e) => setSelectedState(e.target.value || null)}
            className="border p-1 rounded"
          >
            <option value="">Chọn</option>
            {states
              .filter((s) => s !== state)
              .map((s) => (
                <option key={s} value={s}>
                  {s}
                </option>
              ))}
          </select>
          <button
            onClick={() => handleTransition(state)}
            disabled={!selectedState || selectedState === state}
            className="bg-purple-500 text-white p-1 rounded disabled:bg-gray-400"
          >
            Kết Nối
          </button>
        </div>
      ))}
    </div>
  );
}