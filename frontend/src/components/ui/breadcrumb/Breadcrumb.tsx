import { ChevronUpIcon } from "@/icons";
import * as Separator from "@radix-ui/react-separator";

export interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
  className?: string;
}

const Breadcrumb = ({ items, className }: BreadcrumbProps) => (
  <nav
    className={`flex items-center text-sm text-gray-500 mb-4 ${className ?? ""}`}
    aria-label="Breadcrumb"
  >
    {items.map((item, idx) => (
      <span key={item.label} className="flex items-center">
        {item.href && idx !== items.length - 1 ? (
          <a href={item.href} className="hover:underline">
            {item.label}
          </a>
        ) : (
          <span className="text-gray-700 font-medium">{item.label}</span>
        )}
        {idx < items.length - 1 && (
          <ChevronUpIcon
            className="text-gray-500 transition-transform duration-200 mx-2 rotate-90"
          />
        )}
      </span>
    ))}
  </nav>
);

export default Breadcrumb;