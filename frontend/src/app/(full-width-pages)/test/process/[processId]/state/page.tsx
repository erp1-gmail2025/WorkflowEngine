export default async function StatePage({ params }: { params: Promise<{ processId: number }> }) {
  const { processId } = await params;
  return (
    <div className="container mx-auto max-w-7xl">
      <h1 className="text-3xl font-bold mb-6 text-gray-800">Process State - {processId}</h1>
      <p className="text-gray-600 mb-4">This page will display the state of the process with ID: {processId}</p>
    </div>
  );
}