'use client';

import { useState, useEffect } from 'react';
import * as Select from '@radix-ui/react-select';
import * as Toast from '@radix-ui/react-toast';
import axios from 'axios';
import { ChevronDownIcon } from '@radix-ui/react-icons';
import { Request } from './KanbanBoard';

interface State {
  stateID: number;
  name: string;
  description: string;
  isFinal: boolean;
}

interface CreateRequestFormProps {
  processId: number;
  onRequestCreated?: (request: Request) => void;
}

const CreateRequestForm = ({ processId, onRequestCreated }: CreateRequestFormProps) => {
  const [title, setTitle] = useState('');
  const [initialStateName, setInitialStateName] = useState<string | undefined>();
  const [states, setStates] = useState<State[]>([]);
  const [toastOpen, setToastOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchStates = async () => {
      try {
        const res = await axios.get(`https://localhost:7262/api/process/${processId}/states`);
        const fetchedStates: State[] = res.data.$values || [];
        const uniqueStates = Array.from(
          new Map(fetchedStates.map(state => [state.stateID, state])).values()
        ).filter(s => s.stateID != null && !s.isFinal);
        setStates(uniqueStates);
        if (uniqueStates.length === 0) {
          setToastMessage('No valid states available for this process');
          setToastOpen(true);
        }
      } catch (error: any) {
        console.error('Fetch states error:', error);
        setToastMessage(error.response?.data?.message || 'Failed to load states');
        setToastOpen(true);
      }
    };
    fetchStates();
  }, [processId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title) {
      setToastMessage('Please enter request title');
      setToastOpen(true);
      return;
    }

    try {
      setLoading(true);
      const res = await axios.post('https://localhost:7262/api/requests', {
        processId: processId,
        userId: parseInt(localStorage.getItem('userId') || '1'),
        title: title,
        data: { days: '3' },
        initialStateName: initialStateName || undefined,
      });
      setToastMessage('Request created successfully');
      setToastOpen(true);
      setTitle('');
      setInitialStateName(undefined);
      if (onRequestCreated) {
        onRequestCreated(res.data);
      }
    } catch (error: any) {
      console.error('Create request error:', error);
      setToastMessage(error.response?.data?.message || 'Failed to create request');
      setToastOpen(true);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <form onSubmit={handleSubmit} className="mb-6 flex space-x-4">
        <input
          type="text"
          value={title}
          onChange={e => setTitle(e.target.value)}
          placeholder="Request title"
          className="w-64 p-2 border border-gray-300 rounded-md"
          disabled={loading}
        />
        <Select.Root
          value={initialStateName}
          onValueChange={setInitialStateName}
          disabled={loading || states.length === 0}
        >
          <Select.Trigger
            className="w-64 p-2 border border-gray-300 rounded-md flex justify-between items-center"
            aria-label="Select initial state"
          >
            <Select.Value placeholder={states.length === 0 ? 'No states available' : 'Select initial state'} />
            <Select.Icon>
              <ChevronDownIcon />
            </Select.Icon>
          </Select.Trigger>
          <Select.Portal>
            <Select.Content className="bg-white border border-gray-300 rounded-md shadow-lg z-50">
              <Select.Viewport>
                {states.map(state => (
                  <Select.Item
                    key={`state-${state.stateID}`}
                    value={state.name}
                    className="p-2 hover:bg-gray-100 cursor-pointer"
                  >
                    <Select.ItemText>{state.name} ({state.description})</Select.ItemText>
                  </Select.Item>
                ))}
              </Select.Viewport>
            </Select.Content>
          </Select.Portal>
        </Select.Root>
        <button
          type="submit"
          className="px-4 py-2 bg-blue-600 text-white rounded-md disabled:opacity-50"
          disabled={loading}
        >
          {loading ? 'Creating...' : 'Create Request'}
        </button>
      </form>
      <Toast.Root open={toastOpen} onOpenChange={setToastOpen} duration={3000}>
        <Toast.Description>{toastMessage}</Toast.Description>
      </Toast.Root>
    </div>
  );
};

export default CreateRequestForm;