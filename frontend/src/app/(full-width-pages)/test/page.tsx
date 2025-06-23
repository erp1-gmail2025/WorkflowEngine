// app/page.tsx
"use client";

import React, { useEffect, useState } from "react";
import {
  DndContext,
  DragOverlay,
  closestCorners,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  DragOverEvent,
  DragStartEvent,
  DragEndEvent,
  DragCancelEvent
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  verticalListSortingStrategy
} from "@dnd-kit/sortable";
import WorkflowContainer from "@/components/workflow/WorkflowContainer";
import { WorkflowItem } from "@/components/workflow/WorkflowSortableItem";
import { useModal } from "@/hooks/useModal";
import { Modal } from "@/components/ui/modal";
import { State } from "@/components/test/ViewStatesComponent";
import axios from "axios";

export default function App() {
  const { isOpen, openModal, closeModal, toggleModal } = useModal();
  const [states, setStates] = useState<State[]>([]);
  const [items, setItems] = useState<{String,}>({
    root: ["1", "2", "3"],
    container1: ["4", "5", "6"],
    container2: ["7", "8", "9"],
    container3: []
  });
  const [activeId, setActiveId] = useState<string | null>(null);
  const [dialogInput, setDialogInput] = useState("");
  const [activeMonitor, setActiveMonitor] = useState<{
    itemId: string | null;
    fromContainer: string | null;
    toContainer: string | null;
    overId: string | null;
  }>({ itemId: null, fromContainer: null, toContainer: null, overId: null });
  const [lastItems, setLastItems] = useState<typeof items | null>(null);

  useEffect(() => {
    const fetchStates = async () => {
      const response = await axios.get(`https://localhost:7262/api/process/1/states`);
      const data = response.data.$values;
      setStates(data ?? []);
    };
    fetchStates();
  }, []);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates
    })
  );

  const handleDialogCancel = () => {
    if (lastItems) {
      setItems(lastItems);
    }
    setLastItems(null);
    setActiveMonitor({ itemId: null, fromContainer: null, toContainer: null, overId: null });
    setActiveId(null);
    setDialogInput("");
    toggleModal();
  }

  function isContainerId(id: string): boolean {
    return states.some(q => `container-${q.stateID}` === id);
  }

  function findContainer(id: string): keyof typeof items | undefined {
    if (id in items) {
      return id as keyof typeof items;
    }
    return (Object.keys(items) as Array<keyof typeof items>).find((key) => items[key].includes(id));
  }

  function handleDragStart(event: DragStartEvent) {
    const { active } = event;
    const fromContainer = findContainer(active.id as string) || null;
    setActiveId(active.id as string);
    setActiveMonitor((prev) => ({ ...prev, itemId: active.id as string, fromContainer, toContainer: null, overId: null }));
    setLastItems(items); // Lưu lại trạng thái trước khi drag
    console.log("Drag started for item:", active.id);
  }

  function handleDragOver(event: DragOverEvent) {
    const { active, over } = event;
    if (!over) return;
    const id = active.id as string;
    const overId = over.id as string;
    const activeContainer = findContainer(id);
    const overContainer = findContainer(overId);
    if (!activeContainer || !overContainer || activeContainer === overContainer) {
      return;
    }
    setItems((prev) => {
      const activeItems = prev[activeContainer];
      const overItems = prev[overContainer];
      const activeIndex = activeItems.indexOf(id);
      const overIndex = overItems.indexOf(overId);
      const newIndex = overIndex >= 0 ? overIndex : overItems.length;
      return {
        ...prev,
        [activeContainer]: prev[activeContainer].filter((item) => item !== id),
        [overContainer]: [
          ...prev[overContainer].slice(0, newIndex),
          prev[activeContainer][activeIndex],
          ...prev[overContainer].slice(newIndex)
        ]
      };
    });
    console.log("Drag over item:", overId);
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    const itemId = active.id as string;
    const overId = over ? (over.id as string) : null;
    const fromContainer = activeMonitor.fromContainer;
    const toContainer = overId ? findContainer(overId) || null : null;
    setActiveMonitor({
      itemId,
      fromContainer,
      toContainer,
      overId
    });
    if (!over) {
      setActiveId(null);
      return;
    }
    if (!fromContainer || !toContainer) {
      setActiveId(null);
      return;
    }
    if (fromContainer !== toContainer) {
      openModal();
      return;
    }
    const activeIndex = items[fromContainer].indexOf(itemId);
    const overIndex = items[toContainer].indexOf(overId!);
    if (activeIndex !== overIndex) {
      setItems((items) => ({
        ...items,
        [toContainer]: arrayMove(items[toContainer], activeIndex, overIndex)
      }));
    }
    setActiveId(null);
    console.log("Drag ended for item:", itemId, "\nover item:", overId, "\nin container:", toContainer);
    console.log("Active monitor state:", activeMonitor);
    console.log("Check is container ID:", isContainerId(overId!));
  }

  function handleDragCancel(event: DragCancelEvent) {
    setActiveId(null);
  }

  const handleDialogConfirm = () => {
    const { itemId, fromContainer, toContainer } = activeMonitor;
    if (itemId && fromContainer && toContainer) {
      console.log(`Confirmed move from ${fromContainer} to ${toContainer} for item ${itemId}`);
      // Thực hiện logic chuyển mục ở đây
      // Ví dụ: cập nhật cơ sở dữ liệu hoặc trạng thái ứng dụng
    }
    closeModal();
    setActiveMonitor({ itemId: null, fromContainer: null, toContainer: null, overId: null });
    setActiveId(null);
  }
  return (
    <div className={"flex flex-row"}>
      <DndContext
        sensors={sensors}
        collisionDetection={closestCorners}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragEnd={handleDragEnd}
        onDragCancel={handleDragCancel}
      >
        {/* Sử dụng SortableContext cho từng container */}
        {
          states.map((state) => (
            <SortableContext
              key={`container-${state.stateID}`}
              items={items[`container-${state.stateID}`]}
              strategy={verticalListSortingStrategy}
            >
              <WorkflowContainer id={`container-${state.stateID}`} items={items[`container-${state.stateID}`]} />
            </SortableContext>
          ))
        }
        {/* <SortableContext items={items.root} strategy={verticalListSortingStrategy}>
          <WorkflowContainer id="root" items={items.root} />
        </SortableContext>
        <SortableContext items={items.container1} strategy={verticalListSortingStrategy}>
          <WorkflowContainer id="container1" items={items.container1} />
        </SortableContext>
        <SortableContext items={items.container2} strategy={verticalListSortingStrategy}>
          <WorkflowContainer id="container2" items={items.container2} />
        </SortableContext>
        <SortableContext items={items.container3} strategy={verticalListSortingStrategy}>
          <WorkflowContainer id="container3" items={items.container3} />
        </SortableContext> */}
        <DragOverlay>{activeId ? <WorkflowItem id={activeId} /> : null}</DragOverlay>
      </DndContext>
      {isOpen && (
        <Modal
          isOpen={isOpen}
          onClose={handleDialogCancel}
          title="Xác nhận chuyển mục"
          description={`Bạn có muốn chuyển item '${activeMonitor.itemId}' từ container '${activeMonitor.fromContainer}' sang container '${activeMonitor.toContainer}' (over: '${activeMonitor.overId}') không?`}
          footer={
            <>
              <button
                className="px-4 py-2 bg-gray-200 rounded mr-2"
                onClick={handleDialogCancel}
              >
                Huỷ
              </button>
              <button
                className="px-4 py-2 bg-blue-600 text-white rounded"
                onClick={handleDialogConfirm}
                disabled={dialogInput.trim() === ""}
              >
                Xác nhận
              </button>
            </>
          }
        >
          <input
            className="w-full border rounded px-2 py-1 mt-2"
            placeholder="Nhập xác nhận..."
            value={dialogInput}
            onChange={e => setDialogInput(e.target.value)}
          />
        </Modal>
      )}
    </div>
  );
}
