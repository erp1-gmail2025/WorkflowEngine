'use client';

import { useState, useEffect, useCallback } from 'react';
import { DndContext, closestCenter, DragStartEvent, DragEndEvent, DragOverlay } from '@dnd-kit/core';
import { SortableContext, verticalListSortingStrategy } from '@dnd-kit/sortable';
import axios from 'axios';
import * as Dialog from '@radix-ui/react-dialog';
import * as Toast from '@radix-ui/react-toast';
import RequestCard from './RequestCard';
import RequestDetailsModal from './RequestDetailsModal';

interface State {
  stateID: number;
  name: string;
  description: string;
  stateOrder: number;
  isFinal: boolean;
}

export interface Request {
  requestID: number;
  title: string;
  dateRequested: string;
  currentStateID: number;
  currentStateName: string;
  currentStateDescription: string;
}

interface Column {
  id: string;
  title: string;
  requests: Request[];
  isDroppable: boolean;
}

const getUserId = () => {
  return parseInt(localStorage.getItem('userId') || '1');
};

interface KanbanBoardProps {
  processId: number;
  onRequestCreated?: (request: Request) => void;
}

const KanbanBoard = ({ processId, onRequestCreated }: KanbanBoardProps) => {
  const [columns, setColumns] = useState<{ [key: string]: Column }>({});
  const [selectedRequest, setSelectedRequest] = useState<Request | null>(null);
  const [isFailureModalOpen, setIsFailureModalOpen] = useState(false);
  const [failureReason, setFailureReason] = useState('');
  const [draggedRequestID, setDraggedRequestID] = useState<number | null>(null);
  const [targetStateId, setTargetStateId] = useState<number | null>(null);
  const [toastOpen, setToastOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState('');
  const [loading, setLoading] = useState(true);
  const [activeRequest, setActiveRequest] = useState<Request | null>(null);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      const statesRes = await axios.get(`https://localhost:7262/api/process/${processId}/states`);
      const states: State[] = statesRes.data.$values || [];
      const requestsRes = await axios.get(`https://localhost:7262/api/request/${processId}`);
      const requests: Request[] = requestsRes.data.$values || [];

      const uniqueRequests = Array.from(
        new Map(requests.map(req => [req.requestID, req])).values()
      );

      const newColumns: { [key: string]: Column } = {};
      states.forEach(state => {
        newColumns[state.stateID] = {
          id: state.stateID.toString(),
          title: `${state.name} (${state.description})`,
          requests: uniqueRequests.filter(r => r.currentStateID === state.stateID),
          isDroppable: false,
        };
      });

      for (const request of uniqueRequests) {
        try {
          const nextStatesRes = await axios.get(`https://localhost:7262/api/requests/${request.requestID}/next-states`);
          const nextStates: State[] = nextStatesRes.data.$values || [];
          nextStates.forEach(state => {
            if (newColumns[state.stateID]) {
              newColumns[state.stateID].isDroppable = true;
            }
          });
          if (newColumns[request.currentStateID]) {
            newColumns[request.currentStateID].isDroppable = true;
          }
        } catch (error) {
          console.error(`Error fetching next states for request ${request.requestID}:`, error);
        }
      }

      const sortedColumns = Object.values(newColumns).sort((a, b) => {
        const stateA = states.find(s => s.stateID === parseInt(a.id));
        const stateB = states.find(s => s.stateID === parseInt(b.id));
        return (stateA?.stateOrder || 0) - (stateB?.stateOrder || 0);
      });

      const sortedColumnsObj: { [key: string]: Column } = {};
      sortedColumns.forEach(col => {
        sortedColumnsObj[col.id] = col;
      });
      setColumns(sortedColumnsObj);
      console.log('Columns initialized:', sortedColumnsObj);
    } catch (error: any) {
      console.error('Fetch data error:', error);
      setToastMessage(error.response?.data?.message || 'Failed to load Kanban data');
      setToastOpen(true);
    } finally {
      setLoading(false);
    }
  }, [processId]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  const handleDragStart = (event: DragStartEvent) => {
    const { active } = event;
    const requestID = parseInt(active.id.toString().replace('request-', ''));
    const request = Object.values(columns)
      .flatMap(col => col.requests)
      .find(r => r.requestID === requestID);
    setActiveRequest(request || null);
    console.log('Drag start:', { requestID, request });
  };

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;
    setActiveRequest(null);
    if (!over) {
      console.log('Drag end: No over target');
      return;
    }

    const sourceColumnId = active.data.current?.sortable.containerId;
    const destColumnId = over.id.toString();
    if (sourceColumnId === destColumnId) {
      console.log('Drag end: Same column');
      return;
    }

    const requestID = parseInt(active.id.toString().replace('request-', ''));
    const nextStateId = parseInt(destColumnId);

    console.log(`Dragging request ${requestID} from column ${sourceColumnId} to ${destColumnId}`);

    if (columns[destColumnId].title.toLowerCase().includes('failed')) {
      setDraggedRequestID(requestID);
      setTargetStateId(nextStateId);
      setIsFailureModalOpen(true);
      return;
    }

    try {
      setLoading(true);
      await axios.post(`https://localhost:7262/api/requests/${requestID}/action/by-state-id`, {
        nextStateId: nextStateId,
        userId: getUserId(),
      });

      setColumns(prev => {
        const newColumns = { ...prev };
        const request = newColumns[sourceColumnId].requests.find(r => r.requestID === requestID);
        if (request) {
          newColumns[sourceColumnId].requests = newColumns[sourceColumnId].requests.filter(r => r.requestID !== requestID);
          newColumns[destColumnId].requests = [
            ...newColumns[destColumnId].requests,
            {
              ...request,
              currentStateID: nextStateId,
              currentStateName: newColumns[destColumnId].title.split(' ')[0],
              currentStateDescription: newColumns[destColumnId].title.match(/\(([^)]+)\)/)?.[1] || '',
            },
          ];
        }
        return newColumns;
      });
      setToastMessage('Request moved successfully');
      setToastOpen(true);
    } catch (error: any) {
      console.error('Drag end error:', error);
      setToastMessage(error.response?.data?.message || 'Failed to move request');
      setToastOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleFailureSubmit = async () => {
    if (failureReason.length < 10) {
      setToastMessage('Failure reason must be at least 10 characters long');
      setToastOpen(true);
      return;
    }

    try {
      setLoading(true);
      await axios.post(`https://localhost:7262/api/requests/${draggedRequestID}/action/by-state-id`, {
        nextStateId: targetStateId,
        userId: getUserId(),
        failureReason: failureReason,
      });

      setColumns(prev => {
        const newColumns = { ...prev };
        const sourceColumn = Object.values(newColumns).find(col => col.requests.some(r => r.requestID === draggedRequestID));
        if (!sourceColumn) return prev;
        const request = sourceColumn.requests.find(r => r.requestID === draggedRequestID);
        if (request) {
          sourceColumn.requests = sourceColumn.requests.filter(r => r.requestID !== draggedRequestID);
          newColumns[targetStateId!.toString()].requests = [
            ...newColumns[targetStateId!.toString()].requests,
            {
              ...request,
              currentStateID: targetStateId!,
              currentStateName: newColumns[targetStateId!.toString()].title.split(' ')[0],
              currentStateDescription: newColumns[targetStateId!.toString()].title.match(/\(([^)]+)\)/)?.[1] || '',
            },
          ];
        }
        return newColumns;
      });

      setIsFailureModalOpen(false);
      setFailureReason('');
      setDraggedRequestID(null);
      setTargetStateId(null);
      setToastMessage('Request moved to Failed');
      setToastOpen(true);
    } catch (error: any) {
      console.error('Failure submit error:', error);
      setToastMessage(error.response?.data?.message || 'Failed to move request');
      setToastOpen(true);
    } finally {
      setLoading(false);
    }
  };

  const handleCardClick = (request: Request) => {
    setSelectedRequest(request);
  };

  const handleRequestCreated = (newRequest: Request) => {
    setColumns(prev => {
      const newColumns = { ...prev };
      const targetColumn = newColumns[newRequest.currentStateID.toString()];
      if (targetColumn) {
        targetColumn.requests = [...targetColumn.requests, newRequest];
      }
      return newColumns;
    });
    if (onRequestCreated) {
      onRequestCreated(newRequest);
    }
  };

  return (
    <div>
      {loading ? (
        <p className="text-gray-600">Loading Kanban board...</p>
      ) : (
        <DndContext
          collisionDetection={closestCenter}
          onDragStart={handleDragStart}
          onDragEnd={handleDragEnd}
        >
          <div className="flex overflow-x-auto space-x-4">
            {Object.values(columns).map(column => (
              <Column
                key={column.id}
                column={column}
                onCardClick={handleCardClick}
              />
            ))}
          </div>
          <DragOverlay>
            {activeRequest ? (
              <RequestCard
                request={activeRequest}
                isDroppable={true}
                onClick={() => {}}
              />
            ) : null}
          </DragOverlay>
        </DndContext>
      )}
      <Dialog.Root open={isFailureModalOpen} onOpenChange={setIsFailureModalOpen}>
        <Dialog.Portal>
          <Dialog.Overlay className="fixed inset-0 bg-black bg-opacity-50" />
          <Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-lg shadow-lg w-96">
            <Dialog.Title className="text-lg font-semibold mb-4">Enter Failure Reason</Dialog.Title>
            <textarea
              value={failureReason}
              onChange={e => setFailureReason(e.target.value)}
              placeholder="Enter reason for failure (min 10 characters)"
              className="w-full p-2 border border-gray-300 rounded-md mb-4"
              rows={4}
            />
            <div className="flex justify-end space-x-2">
              <Dialog.Close asChild>
                <button className="px-4 py-2 bg-gray-200 rounded-md">Cancel</button>
              </Dialog.Close>
              <button
                onClick={handleFailureSubmit}
                disabled={failureReason.length < 10}
                className={`px-4 py-2 bg-blue-600 text-white rounded-md ${failureReason.length < 10 ? 'opacity-50 cursor-not-allowed' : ''}`}
              >
                Submit
              </button>
            </div>
          </Dialog.Content>
        </Dialog.Portal>
      </Dialog.Root>
      {selectedRequest && (
        <RequestDetailsModal
          request={selectedRequest}
          onClose={() => setSelectedRequest(null)}
        />
      )}
      <Toast.Root open={toastOpen} onOpenChange={setToastOpen} duration={3000}>
        <Toast.Description>{toastMessage}</Toast.Description>
      </Toast.Root>
    </div>
  );
};

interface ColumnProps {
  column: Column;
  onCardClick: (request: Request) => void;
}

const Column = ({ column, onCardClick }: ColumnProps) => {
  let bgColor = column.isDroppable ? 'white' : 'gray-100';
  if (column.title.toLowerCase().includes('failed')) {
    bgColor = 'red-100';
  } else if (column.title.toLowerCase().includes('completed')) {
    bgColor = 'green-100';
  }

  return (
    <div
      className={`flex-shrink-0 m-2 p-4 w-72 min-h-[400px] bg-${bgColor} border border-gray-300 rounded-lg shadow-sm`}
      id={column.id}
    >
      <h3 className="text-lg font-semibold mb-3 text-gray-800">{column.title}</h3>
      <SortableContext items={column.requests.map(r => `request-${r.requestID}`)} strategy={verticalListSortingStrategy}>
        <div className="space-y-2">
          {column.requests.map(request => (
            <RequestCard
              key={`request-${request.requestID}`}
              request={request}
              isDroppable={column.isDroppable}
              onClick={() => onCardClick(request)}
            />
          ))}
        </div>
      </SortableContext>
    </div>
  );
};

export default KanbanBoard;