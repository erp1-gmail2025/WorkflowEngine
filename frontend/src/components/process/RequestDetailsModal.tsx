'use client';

import * as Dialog from '@radix-ui/react-dialog';
import * as Toast from '@radix-ui/react-toast';
import axios from 'axios';
import { useState } from 'react';
import { Request } from './KanbanBoard';


interface RequestDetailsModalProps {
  request: Request;
  onClose: () => void;
}

const RequestDetailsModal = ({ request, onClose }: RequestDetailsModalProps) => {
  const [note, setNote] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [toastOpen, setToastOpen] = useState(false);
  const [toastMessage, setToastMessage] = useState('');
  const [loadingNote, setLoadingNote] = useState(false);
  const [loadingFile, setLoadingFile] = useState(false);

  const handleAddNote = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!note) {
      setToastMessage('Please enter a note');
      setToastOpen(true);
      return;
    }

    try {
      setLoadingNote(true);
      await axios.post(`https://localhost:7262/api/request/${request.requestID}/notes`, {
        UserId: parseInt(localStorage.getItem('userId') || '2'),
        Note: note,
      });
      setToastMessage('Note added successfully');
      setToastOpen(true);
      setNote('');
    } catch (error: any) {
      setToastMessage(error.response?.data?.message || 'Failed to add note');
      setToastOpen(true);
    } finally {
      setLoadingNote(false);
    }
  };

  const handleUploadFile = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file) {
      setToastMessage('Please select a file');
      setToastOpen(true);
      return;
    }

    const formData = new FormData();
    formData.append('UserId', localStorage.getItem('userId') || '2');
    formData.append('File', file);

    try {
      setLoadingFile(true);
      await axios.post(`https://localhost:7262/api/request/${request.requestID}/files`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      setToastMessage('File uploaded successfully');
      setToastOpen(true);
      setFile(null);
    } catch (error: any) {
      setToastMessage(error.response?.data?.message || 'Failed to upload file');
      setToastOpen(true);
    } finally {
      setLoadingFile(false);
    }
  };

  return (
    <Dialog.Root open={true} onOpenChange={onClose}>
      <Dialog.Portal>
        <Dialog.Overlay className="fixed inset-0 bg-black bg-opacity-50" />
        <Dialog.Content className="fixed top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-lg shadow-lg w-[600px]">
          <Dialog.Title className="text-lg font-semibold mb-4">Request: {request.title}</Dialog.Title>
          <div className="space-y-4">
            <p>
              <strong>Current State:</strong> {request.currentStateName} ({request.currentStateDescription})
            </p>
            <div>
              <h3 className="text-lg font-semibold mb-2">Add Note</h3>
              <form onSubmit={handleAddNote} className="space-y-2">
                <textarea
                  value={note}
                  onChange={e => setNote(e.target.value)}
                  placeholder="Enter note"
                  className="w-full p-2 border border-gray-300 rounded-md"
                  rows={3}
                  disabled={loadingNote}
                />
                <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded-md" disabled={loadingNote}>
                  {loadingNote ? 'Adding...' : 'Add Note'}
                </button>
              </form>
            </div>
            <div>
              <h3 className="text-lg font-semibold mb-2">Upload File</h3>
              <form onSubmit={handleUploadFile} className="space-y-2">
                <input
                  type="file"
                  onChange={e => setFile(e.target.files?.[0] || null)}
                  className="w-full p-2 border border-gray-300 rounded-md"
                  disabled={loadingFile}
                />
                <button type="submit" className="px-4 py-2 bg-blue-600 text-white rounded-md" disabled={loadingFile}>
                  {loadingFile ? 'Uploading...' : 'Upload File'}
                </button>
              </form>
            </div>
          </div>
          <Dialog.Close asChild>
            <button className="absolute top-4 right-4 text-gray-600 hover:text-gray-800">✕</button>
          </Dialog.Close>
        </Dialog.Content>
      </Dialog.Portal>
      <Toast.Root open={toastOpen} onOpenChange={setToastOpen} duration={3000}>
        <Toast.Description>{toastMessage}</Toast.Description>
      </Toast.Root>
    </Dialog.Root>
  );
};

export default RequestDetailsModal;