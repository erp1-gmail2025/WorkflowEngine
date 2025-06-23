"use client";
import React from "react";
import * as Dialog from "@/components/ui/dialog";

interface ModalProps {
  isOpen: boolean;
  onClose: () => void;
  className?: string;
  children: React.ReactNode;
  showCloseButton?: boolean;
  isFullscreen?: boolean;
  title?: string;
  description?: string;
  footer?: React.ReactNode;
}

export const Modal: React.FC<ModalProps> = ({
  isOpen,
  onClose,
  children,
  className,
  showCloseButton = true,
  isFullscreen = false,
  title,
  description,
  footer,
}) => {
  return (
    <Dialog.Dialog open={isOpen} onOpenChange={(open) => !open && onClose()}>
      <Dialog.DialogContent
        className={
          isFullscreen
            ? "w-full h-full max-w-full max-h-full rounded-none p-0"
            : className
        }
      >
        {showCloseButton && <Dialog.DialogClose />}
        {title && <Dialog.DialogTitle>{title}</Dialog.DialogTitle>}
        {description && (
          <Dialog.DialogDescription>{description}</Dialog.DialogDescription>
        )}
        <div>{children}</div>
        <Dialog.DialogFooter>{footer}</Dialog.DialogFooter>
      </Dialog.DialogContent>
    </Dialog.Dialog>
  );
};
