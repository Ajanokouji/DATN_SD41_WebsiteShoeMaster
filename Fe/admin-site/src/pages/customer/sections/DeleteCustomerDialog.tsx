// src/components/Customer/DeleteCustomerDialog.tsx
import React from "react";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";

interface DeleteCustomerDialogProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    customerName: string;
}

const DeleteCustomerDialog: React.FC<DeleteCustomerDialogProps> = ({
    isOpen,
    onClose,
    onConfirm,
    customerName,
}) => {
    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="max-w-md">
                <DialogHeader>
                    <DialogTitle>Confirm Deletion</DialogTitle>
                </DialogHeader>
                <p className="text-gray-600">
                    Are you sure you want to delete the customer: <strong>{customerName}</strong>?
                </p>
                <DialogFooter className="flex justify-end gap-2">
                    <Button onClick={onConfirm} variant="destructive">Yes, Delete</Button>
                    <Button onClick={onClose} variant="outline">Cancel</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};

export default DeleteCustomerDialog;
