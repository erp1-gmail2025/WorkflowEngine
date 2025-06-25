import { useDroppable } from "@dnd-kit/core";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";
import WorkflowSortableItem from "./WorkflowSortableItem";
import { cn } from "@/lib/utils";
import { Request } from "../process/KanbanBoard";

// interface WorkflowContainerProps {
//   id: string;
//   items: Request[];
//   name?: string;
//   description?: string;
// }

// export default function WorkflowContainer(props: WorkflowContainerProps) {
//   const { id, items, name, description } = props;

//   const { setNodeRef } = useDroppable({
//     id
//   });
//   console.log(`WorkflowContainer: ${id}`, items);
//   return (
//     <SortableContext
//       id={id}
//       items={items.map(item => String(item.requestID))}
//       strategy={verticalListSortingStrategy}
//     >
//       <div ref={setNodeRef} className={
//         cn(
//           `flex flex-1 flex-col min-w-[300px] h-[95vh]`,
//           `p-10 m-2`,
//           `rounded-lg shadow-md overflow-y-auto`,
//           `${name === 'Failed' ? 'bg-red-600 text-white' : (name === 'Completed' ? 'bg-green-600 text-white' : 'bg-neutral-200')}`,
//           )}>
//         <h1>{name === 'Failed' || name === 'Completed' ? name : description}</h1>
//         {items.map((item) => (
//           <WorkflowSortableItem key={item.requestID} id={String(item.requestID)} />
//         ))}
//       </div>
//     </SortableContext>
//   );
// }


interface WorkflowContainerProps {
  id: string;
  items: Request[];
  name?: string;
  description?: string;
}

export default function WorkflowContainer(props: WorkflowContainerProps) {
  const { id, items, name, description } = props;
  // const { id, items} = props;

  const { setNodeRef } = useDroppable({
    id
  });

  return (
    <SortableContext
      id={id}
      items={items.map(item => String(item.requestID))}
      strategy={verticalListSortingStrategy}
    >
      <div ref={setNodeRef} className={
        cn(
          `flex flex-1 flex-col min-w-[300px]`,
          `p-10 m-2`,
          `rounded-lg shadow-md overflow-y-auto`,
          `${name === 'Failed' ? 'bg-red-600 text-white' : (name === 'Completed' ? 'bg-green-600 text-white' : 'bg-neutral-200')}`,
        )}>
        <h1>{name === 'Failed' || name === 'Completed' ? name : description}</h1>
        {items.map((item) => (
          <WorkflowSortableItem key={item.requestID} item={item} />
        ))}
      </div>
    </SortableContext>
  );
}
