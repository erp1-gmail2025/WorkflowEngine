'use client';

import { State } from "@/components/test/ViewStatesComponent";
import { useEffect, useState } from "react";
import {
    DndContext,
    closestCenter,
    PointerSensor,
    useSensor,
    useSensors,
    DragEndEvent
} from "@dnd-kit/core";
import {
    arrayMove,
    SortableContext,
    verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import SortableStateItem from "./SortableStateItem";
import { useModal } from "@/hooks/useModal";
import { Modal } from "@/components/ui/modal";
import { Button } from "@/components/ui/button";
import z from 'zod';

interface StateListProps {
    processId?: number;
}


const StateList = (props: StateListProps) => {
    const { processId } = props;
    const [states, setStates] = useState<State[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [items, setItems] = useState<string[]>([]); // id dạng string
    const { closeModal, isOpen, openModal } = useModal();

    // const FormFieldDataSchema = z.object({
    //     key: z.string()
    //         .min(1, "Mã không được để trống!")
    //         .regex(/^[A-Z0-9_]+$/, "Mã chỉ được viết hoa, số và gạch dưới (A-Z, 0-9, _)"),
    //     value: z.string().min(1, { message: `${GroupCodeTitle[groupCode]} không được để trống!` }),
    //     value_EN: z.string().min(1, { message: `${GroupCodeTitle[groupCode]}(EN) không được để trống!` }),
    //     notes: z.string().optional(),
    //     ...(groupCode === 'LOCATION' && { wareHouseCode: z.string().min(1, 'Kho không được để trống!') })
    // });

    useEffect(() => {
        const fetchStates = async () => {
            const response = await fetch(`https://localhost:7262/api/process/${processId}/states`);
            if (!response.ok) {
                throw new Error("Failed to fetch states");
            }
            const data = await response.json();
            const loadedStates: State[] = data.$values || [];
            setStates(loadedStates);
            // Đưa state có stateOrder = 0 về cuối, còn lại giữ nguyên
            const withOrder = loadedStates.filter(s => s.stateOrder !== 0);
            const noOrder = loadedStates.filter(s => s.stateOrder === 0);
            setItems([
                "create",
                ...withOrder.map(s => String(s.stateID)),
                ...noOrder.map(s => String(s.stateID))
            ]);
            setLoading(false);
        }
        fetchStates();
    }, [processId]);

    const sensors = useSensors(
        useSensor(PointerSensor)
    );

    function handleDragEnd(event: DragEndEvent) {
        const { active, over } = event;
        if (!over || active.id === over.id) return;
        setItems((items) => {
            const oldIndex = items.indexOf(String(active.id));
            const newIndex = items.indexOf(String(over.id));
            return arrayMove(items, oldIndex, newIndex);
        });
    }

    function handleCreateState() {
        openModal();
    }

    return (
        <>
            <div className="flex flex-col items-center justify-center p-4 w-full max-w-md mx-auto">
                <h2 className="mb-2 font-bold">State List</h2>
                <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
                    <SortableContext items={items} strategy={verticalListSortingStrategy}>
                        {items.map((id, idx) =>
                            id === "create" ? (
                                <div key="create" className="w-full mb-2">
                                    <button
                                        className="w-full py-2 px-4 bg-blue-500 text-white rounded hover:bg-blue-600"
                                        onClick={handleCreateState}
                                    >
                                        + Tạo state mới
                                    </button>
                                </div>
                            ) : (
                                <SortableStateItem key={id} id={id} state={states.find(s => String(s.stateID) === id)} />
                            )
                        )}
                    </SortableContext>
                </DndContext>
            </div>
            <Modal
                isOpen={isOpen}
                onClose={closeModal}
                title="State List"
                description="Drag and drop states to reorder them."
                footer={
                    <Button
                        className="hover:bg-neutral-100"
                        variant={"secondary"}
                        onClick={closeModal}
                    >
                        Đóng
                    </Button>
                }
            >
                <>
                    <div className="p-4">
                        <h3 className="text-lg font-semibold mb-2">Create New State</h3>
                        <form>
                            <div className="mb-4">
                                <label className="block text-sm font-medium mb-1">State Name</label>
                                <input
                                    type="text"
                                    className="w-full p-2 border border-gray-300 rounded"
                                    placeholder="Enter state name"
                                />
                            </div>
                            <div className="mb-4">
                                <label className="block text-sm font-medium mb-1">Description</label>
                                <textarea
                                    className="w-full p-2 border border-gray-300 rounded"
                                    placeholder="Enter description"
                                ></textarea>
                            </div>
                            <Button type="submit" className="w-full bg-blue-500 text-white hover:bg-blue-600">
                                Create State
                            </Button>
                        </form>
                    </div>
                </>
            </Modal>
        </>

    );
}

export default StateList;