import React from "react";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { Request } from "../process/KanbanBoard";

interface ItemProps {
    item: Request;
}

export function WorkflowItem(props: ItemProps) {
  const { item } = props;

  const style = {
    width: "100%",
    height: 50,
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    border: "1px solid black",
    margin: "10px 0",
    background: "white"
  };

  return <div style={style}>{item.title}</div>;
}

interface SortableItemProps {
    item: Request;
}

export default function WorkflowSortableItem(props: SortableItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition
  } = useSortable({ id: props.item.requestID });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      <WorkflowItem item={props.item} />
    </div>
  );
}
