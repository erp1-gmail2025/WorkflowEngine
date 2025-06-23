import * as TabsPrimitive from "@radix-ui/react-tabs";

export interface TabItem {
  value: string;
  label: string;
  content: React.ReactNode;
}

interface TabsProps {
  items: TabItem[];
  defaultValue?: string;
  className?: string;
}

const Tabs = ({ items, defaultValue, className }: TabsProps) => (
  <TabsPrimitive.Root
    className={className}
    defaultValue={defaultValue || items[0]?.value}
  >
    <TabsPrimitive.List className="ps-4 flex border-b mb-1">
      {items.map((item) => (
        <TabsPrimitive.Trigger
          key={item.value}
          value={item.value}
          className="py-[18px] px-4 text-sm font-medium text-gray-600 data-[state=active]:text-primary-500 data-[state=active]:border-b-2 data-[state=active]:border-primary-500 outline-none cursor-pointer transition-colors"
        >
          {item.label}
        </TabsPrimitive.Trigger>
      ))}
    </TabsPrimitive.List>
    {items.map((item) => (
      <TabsPrimitive.Content
        key={item.value}
        value={item.value}
        className="pt-2"
      >
        {item.content}
      </TabsPrimitive.Content>
    ))}
  </TabsPrimitive.Root>
);

export default Tabs;