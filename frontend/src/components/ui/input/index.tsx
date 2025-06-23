import * as React from "react";

import { cn } from "@/lib/utils";
import { cva } from "class-variance-authority";
import { Slot } from "@radix-ui/react-slot";


const InputVariants = cva(
    "",
    {
        variants: {
            // variant: {
            //     default:
            //         "bg-primary-500 text-neutral-100 shadow hover:bg-primary/90",
            //     destructive:
            //         "bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90",
            //     outline:
            //         "border border-input bg-background shadow-sm hover:bg-accent hover:text-accent-foreground",
            //     secondary:
            //         "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80",
            //     ghost: "hover:bg-accent hover:text-accent-foreground",
            //     link: "text-primary underline-offset-4 hover:underline",
            // },
            inputSize: {
                default: "h-8",
                sm: "h-8 text-xs",
                md: "h-9 rounded-md text-sm",
                lg: "h-10 rounded-md",
            },
            rounded: {
                none: "rounded-none",
                sm: "rounded-sm",
                md: "rounded-md",
                lg: "rounded-lg",
                full: "rounded-full",
            }
        },
        defaultVariants: {
            // variant: "default",
            inputSize: "default",
            rounded: "sm",
        },
    }
);

export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    asChild?: boolean;
    variant?: 'default' | 'destructive' | 'outline' | 'secondary' | 'ghost' | 'link';
    inputSize?: 'default' | 'sm' | 'md' | 'lg';
    rounded?: 'none' | 'sm' | 'md' | 'lg' | 'full';
}

const Input = React.forwardRef<HTMLInputElement, InputProps>(
    ({ className, variant, inputSize, rounded, asChild = false, ...props }, ref) => {

        const Comp = asChild ? Slot : "input";
        return (
            <Comp
                className={cn(
                    InputVariants({ inputSize, rounded }),
                    "flex w-full border border-input bg-transparent px-3 py-1 text-sm transition-colors file:border-0 file:bg-transparent file:text-sm file:font-medium placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:cursor-not-allowed disabled:opacity-50",
                    className
                )}
                ref={ref}
                {...props}
            />
        );
    }
);
Input.displayName = "Input";

export { Input };
