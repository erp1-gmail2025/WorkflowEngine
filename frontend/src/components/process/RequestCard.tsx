'use client';

import { useSortable } from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';

interface Request {
  requestID: number;
  title: string;
}

interface RequestCardProps {
  request: Request;
  isDroppable: boolean;
  onClick: () => void;
}

const RequestCard = ({ request, isDroppable, onClick }: RequestCardProps) => {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: `request-${request.requestID}`,
    disabled: !isDroppable,
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
    cursor: isDroppable ? 'grab' : 'not-allowed',
    userSelect: 'none' as const,
  };

  const handleClick = (e: React.MouseEvent) => {
    if (isDragging) return;
    onClick();
  };

  return (
    <div
      ref={setNodeRef}
      style={style}
      {...attributes}
      {...listeners}
      onClick={handleClick}
      className={`p-3 bg-white border border-gray-200 rounded-md shadow-sm ${isDroppable ? 'hover:bg-gray-50' : 'opacity-50'}`}
    >
      <p className="font-medium text-gray-800">{request.title}</p>
    </div>
  );
};

export default RequestCard;