import React from "react";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

interface ItemProps {
    id: string;
}

export function WorkflowItem(props: ItemProps) {
  const { id } = props;

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

  return <div style={style}>{id}</div>;
}

interface SortableItemProps {
    id: string;
}

export default function WorkflowSortableItem(props: SortableItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition
  } = useSortable({ id: props.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners}>
      <WorkflowItem id={props.id} />
    </div>
  );
}
