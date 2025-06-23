'use client';

import { useState, useEffect } from 'react';
import Link from 'next/link';
import axios from 'axios';
import * as Toast from '@radix-ui/react-toast';

interface Process {
  processID: number;
  name: string;
}

export default function Home() {
  const [processes, setProcesses] = useState<Process[]>([]);
  const [loading, setLoading] = useState(true);
  const [toastOpen, setToastOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState('');

  useEffect(() => {
    const fetchProcesses = async () => {
      try {
        const res = await axios.get('https://localhost:7262/api/process/processes');
        setProcesses(res.data.$values || []);
      } catch (error) {
        setToastMessage('Failed to load processes');
        setToastOpen(true);
      } finally {
        setLoading(false);
      }
    };
    fetchProcesses();
  }, []);

  return (
    <div className="container mx-auto max-w-5xl">
      <h1 className="text-3xl font-bold mb-6 text-gray-800">Workflow Management</h1>
      {loading ? (
        <p className="text-gray-600">Loading processes...</p>
      ) : processes.length === 0 ? (
        <p className="text-gray-600">No processes found.</p>
      ) : (
        <>
          <p className="text-gray-600 mb-4">Select a process to view its Kanban board:</p>
          <ul className="list-disc pl-6">
            {processes.map(process => (
              <li key={process.processID}>
                <Link href={`/processes/${process.processID}`} className="text-blue-600 hover:underline">
                  {process.name} (ID: {process.processID})
                </Link>
              </li>
            ))}
          </ul>
        </>
      )}
      <Toast.Root open={toastOpen} onOpenChange={setToastOpen} duration={3000}>
        <Toast.Description>{toastMessage}</Toast.Description>
      </Toast.Root>
    </div>
  );
}