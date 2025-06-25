import { State } from "@/components/test/ViewStatesComponent";
import { Button } from "@/components/ui/button";
import { CloseIcon } from "@/icons";
import { cn } from "@/lib/utils";
import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";

const SortableStateItem = ({ id, state }: { id: string, state?: State }) => {
    const {
        attributes,
        listeners,
        setNodeRef,
        transform,
        transition
    } = useSortable({ id });
    const style = {
        transform: CSS.Transform.toString(transform),
        transition,
        width: "100%",
        marginBottom: 8,
        border: "1px solid #ddd",
        borderRadius: 6,
        padding: 12,
        cursor: "grab"
    };
    if (!state) return null;
    return (
        <div ref={setNodeRef} style={style} {...attributes} {...listeners}
            className={
                cn(
                    state.name === 'Failed' ? 'bg-red-600 text-white' :
                        (state.name === 'Completed' ? 'bg-green-600 text-white' : 'bg-neutral-200'),
                    'relative'
                )
            }
        >
            {state.stateOrder !== 0 && (
                <Button className="absolute top-2 right-2 cursor-pointer hover:bg-neutral-300" variant={"default"} size={"icon"}>
                    <CloseIcon className='text-red-500'></CloseIcon>
                </Button>
            )}
            <div className="font-semibold">{state.name}</div>
            <div className="text-xs">ID: {state.stateID}</div>
            <div className="text-sm">{state.description}</div>
        </div>
    );
}

export default SortableStateItem;