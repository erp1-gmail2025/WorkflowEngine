import StateList from "@/components/process/state/StateList";

export default async function StatePage({ params }: { params: Promise<{ processId: number }> }) {
  const { processId } = await params;
  return (
    <StateList processId={processId}/>
  );
}