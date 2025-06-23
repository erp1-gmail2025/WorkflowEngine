// components/StateItem.tsx
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

interface StateItemProps {
  id: string;
  isCurrent: boolean;
}

export default function StateItem({ id, isCurrent }: StateItemProps) {
  const { attributes, listeners, setNodeRef, transform, transition } = useSortable({ id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    padding: "10px",
    margin: "5px",
    backgroundColor: isCurrent ? "#4CAF50" : "#e0f7fa",
    color: isCurrent ? "white" : "#26a69a",
    border: "1px solid #26a69a",
    borderRadius: "4px",
    cursor: "move",
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      {id}
    </div>
  );
}