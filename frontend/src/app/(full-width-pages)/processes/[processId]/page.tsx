'use client';

import CreateRequestForm from "@/components/process/CreateRequestForm";
import KanbanBoard from "@/components/process/KanbanBoard";


export default async function ProcessPage({ params }: { params: Promise<{ processId: number }> }) {
  const { processId } = await params;
  return (
    <div className="container mx-auto max-w-7xl">
      <h1 className="text-3xl font-bold mb-6 text-gray-800">Kanban Board - Process {processId}</h1>
      <CreateRequestForm processId={processId} />
      <KanbanBoard processId={processId} />
    </div>
  );
}