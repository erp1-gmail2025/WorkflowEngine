// app/page.tsx
"use client";

import React, { use, useEffect, useState } from "react";
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
import { Request } from "@/components/process/KanbanBoard";

export default function App() {
  const { isOpen, openModal, closeModal, toggleModal } = useModal();
  const [states, setStates] = useState<State[]>([]);
  const [request, setRequest] = useState<Request[]>([]);
  const [items, setItems] = useState<{
    [key: string]: Request[];
  }>({
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

  useEffect(() => {
    const fetchRequests = async () => {
      const response = await axios.get(`https://localhost:7262/api/Request/1`);
      const data = response.data.$values;
      setRequest(data ?? []);
    }
    fetchRequests();
  }, []);

  useEffect(() => {
    if (states.length === 0 || request.length === 0) return;
    const initialItems: { [key: string]: Request[] } = {};
    states.forEach(state => {
      initialItems[`container-${state.stateID}`] = request.filter(req => req.currentStateID === state.stateID);
    });
    setItems(initialItems);
  }, [states, request]);

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
    return (Object.keys(items) as Array<keyof typeof items>).find((key) =>
      items[key].some((q) => String(q.requestID) == id)
    );
  }

  function handleDragStart(event: DragStartEvent) {
    const { active } = event;
    const foundContainer = findContainer(active.id as string);
    const fromContainer = typeof foundContainer === "string" ? foundContainer : null;
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
      const activeIndex = activeItems.findIndex(q => String(q.requestID) == id);
      const overIndex = overItems.findIndex(q => String(q.requestID) == overId);
      const newIndex = overIndex >= 0 ? overIndex : overItems.length;
      const movingItem = activeItems[activeIndex];
      if (!movingItem) return prev;
      return {
        ...prev,
        [activeContainer]: prev[activeContainer].filter((item) => String(item.requestID) != id),
        [overContainer]: [
          ...prev[overContainer].slice(0, newIndex),
          movingItem,
          ...prev[overContainer].slice(newIndex)
        ]
      };
    });
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    const itemId = active.id as string;
    const overId = over ? (over.id as string) : null;
    const fromContainer = activeMonitor.fromContainer;
    const toContainer = overId ? String(findContainer(overId)) || null : null;
    setActiveMonitor({
      itemId,
      fromContainer,
      toContainer,
      overId
    });
    if (!over) {
      setActiveId(null);
      if (lastItems) setItems(lastItems); // rollback nếu không thả vào đâu
      return;
    }
    if (!fromContainer || !toContainer) {
      setActiveId(null);
      if (lastItems) setItems(lastItems);
      return;
    }
    if (fromContainer !== toContainer) {
      openModal();
      return;
    }
    // Nếu cùng container thì move bằng arrayMove
    const activeIndex = items[fromContainer].findIndex(item => String(item.requestID) == itemId);
    const overIndex = items[toContainer].findIndex(item => String(item.requestID) == overId!);
    if (activeIndex !== overIndex) {
      setItems((items) => ({
        ...items,
        [toContainer]: arrayMove(items[toContainer], activeIndex, overIndex)
      }));
    }
    setActiveId(null);
  }

  function handleDragCancel(event: DragCancelEvent) {
    setActiveId(null);
    if (lastItems) setItems(lastItems); // rollback nếu huỷ drag
  }

  const handleDialogConfirm = () => {
    // Đã move realtime rồi, chỉ cần reset monitor
    closeModal();
    setActiveMonitor({ itemId: null, fromContainer: null, toContainer: null, overId: null });
    setActiveId(null);
    setLastItems(null);
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
        {
          states.map((state) => {
            const containerId = `container-${state.stateID}`;
            return (
              <SortableContext
                key={containerId}
                items={(items[containerId] ?? []).map(item => String(item.requestID))}
                strategy={verticalListSortingStrategy}
              >
                <WorkflowContainer
                  id={containerId}
                  items={items[`container-${state.stateID}`] ?? []}
                  name={state.name}
                  description={state.description}
                />
              </SortableContext>
            )
          })
        }
        {/* <DragOverlay>
          {activeId ? (
            <WorkflowItem id={activeId} />
            // (() => {
            //   console.log("Active ID:", activeId);
            //   const item = request.find(q => String(q.requestID) == activeId);
            //   return item ? <WorkflowItem id={activeId} /> : null;
            // })()
          ) : null}
        </DragOverlay> */}
        <DragOverlay>
          {activeId ? (
            (() => {
              const allItems = Object.values(items).flat();
              const item = allItems.find(q => String(q.requestID) == activeId);
              return item ? <WorkflowItem item={item} /> : null;
            })()
          ) : null}
        </DragOverlay>
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
