'use client';

import { Button } from "@/components/ui/button";
import { Modal } from "@/components/ui/modal";
import { useModal } from "@/hooks/useModal";
import axios from "axios";
import Link from "next/link";
import { useEffect, useState } from "react";

export interface Process {
    processID: number;
    name: string;
}

export default function ProcessPage() {
    const [processes, setProcesses] = useState<Process[]>([]);
    const { closeModal, isOpen, openModal } = useModal();
    const [dialogInput, setDialogInput] = useState("");
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        const fetchProcesses = async () => {
            try {
                const res = await axios.get('https://localhost:7262/api/process/processes');
                setProcesses(res.data.$values || []);
            } catch (error) {
                console.error('Failed to load processes:', error);
            } finally {
                setLoading(false);
            }
        };
        fetchProcesses();
    }, []);

    const confirmNewProcess = async () => {
        const res = await axios.post('https://localhost:7262/api/process/processes', {
            name: dialogInput,
            adminId: 1,
            userId: 1
        });
        if (res.status === 200) {
            setProcesses(prev => [...prev, res.data]);
            setDialogInput("");
            closeModal();
        } else {
            console.error('Failed to create new process:', res);
        }
    }

    if (loading) {
        return <div className="container mx-auto max-w-5xl">Loading processes...</div>;
    }
    if (processes.length === 0) {
        return <div className="container mx-auto max-w-5xl">No processes found.</div>;
    }
    return (
        <>
            <div>
                <h1 className="text-3xl font-bold mb-6 text-gray-800">Workflow Management</h1>
                <p className="text-gray-600 mb-4">Select a process to view its Kanban board:</p>
                <Button variant={"default"} onClick={openModal}>Thêm quy trình</Button>
                <table className="min-w-full border border-gray-300 rounded mt-4">
                    <thead className="bg-gray-100">
                        <tr>
                            <th className="px-4 py-2 border-b text-left">Tên quy trình</th>
                            <th className="px-4 py-2 border-b text-left">ID</th>
                            <th className="px-4 py-2 border-b text-left">Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {processes.map(process => (
                            <tr key={process.processID} className="hover:bg-gray-50">
                                <td className="px-4 py-2 border-b">{process.name}</td>
                                <td className="px-4 py-2 border-b">{process.processID}</td>
                                <td className="px-4 py-2 border-b">
                                    <div className="d-flex">
                                        <Link href={`/test/process/${process.processID}`} className="text-blue-600 hover:underline">
                                            Xem Kanban
                                        </Link>
                                        <Link href={`/test/process/${process.processID}`} className="text-blue-600 hover:underline">
                                            Chỉnh sửa giai đoạn
                                        </Link>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <Modal
                isOpen={isOpen}
                onClose={closeModal}
                title="Thêm quy trình mới"
                description={`Vui lòng nhập tên quy trình mới`}
                footer={
                    <>
                        <button
                            className="px-4 py-2 bg-gray-200 rounded mr-2"
                            onClick={closeModal}
                        >
                            Huỷ
                        </button>
                        <button
                            className="px-4 py-2 bg-blue-600 text-white rounded"
                            onClick={confirmNewProcess}
                        >
                            Xác nhận
                        </button>
                    </>
                }
            >
                <p className="text-gray-600">Nhập tên quy trình mới:</p>
                <input
                    className="w-full border rounded px-2 py-1 mt-2"
                    placeholder="Nhập tên quy trình..."
                    value={dialogInput}
                    onChange={e => setDialogInput(e.target.value)}
                />
            </Modal>
        </>);
}