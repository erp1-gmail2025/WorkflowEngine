import { useDroppable } from "@dnd-kit/core";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import WorkflowSortableItem from "./WorkflowSortableItem";

interface WorkflowContainerProps {
  id: string;
  items: string[];
}

export default function WorkflowContainer(props: WorkflowContainerProps) {
  const { id, items } = props;

  const { setNodeRef } = useDroppable({
    id
  });

  return (
    <SortableContext
      id={id}
      items={items}
      strategy={verticalListSortingStrategy}
    >
      <div ref={setNodeRef} className="p-10 flex flex-1 flex-col m-2 bg-neutral-200 rounded-lg shadow-md overflow-y-auto">
        {items.map((id) => (
          <WorkflowSortableItem key={id} id={id} />
        ))}
      </div>
    </SortableContext>
  );
}
