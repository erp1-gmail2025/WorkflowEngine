import { useEffect, useState } from "react";

export interface State {
    stateID: number;
    name: string;
    description: string;
    isFinal: boolean;
    stateOrder: number;
}

export interface ViewStatesComponentProps {
    processID: number;
}

export default function ViewStatesComponent(props: ViewStatesComponentProps) {
    
    const [states, setStates] = useState<State[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    useEffect(() => {
        const fetchStates = async () => {
            try {
                const response = await fetch(`https://localhost:7262/api/process/${props.processID}/states`);
                if (!response.ok) {
                    throw new Error("Failed to fetch states");
                }
                const data = await response.json();
                setStates(data.$values || []);
            } catch (error) {
                console.error("Error fetching states:", error);
            } finally {
                setLoading(false);
            }
        };
        fetchStates();
    });

    if(loading) {
        return <div className="container mx-auto max-w-5xl">Loading states...</div>;
    }

    if(states.length === 0) {
        return <div className="container mx-auto max-w-5xl">No states found for this process.</div>;
    }

    return (
        <div className="container mx-auto max-w-5xl">
            <h1 className="text-3xl font-bold mb-6 text-gray-800">Process States for Process ID: {props.processID}</h1>
            <ul className="list-disc pl-6">
                {states.map(state => (
                    <li key={state.stateID} className="mb-2">
                        <strong>{state.name}</strong> (ID: {state.stateID}) - {state.description} 
                        {state.isFinal ? " (Final State)" : ""}
                    </li>
                ))}
            </ul>
        </div>
    );
}