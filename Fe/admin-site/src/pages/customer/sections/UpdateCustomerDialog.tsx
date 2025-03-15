// src/components/Customer/UpdateCustomerDialog.tsx
import React, { useEffect, useState } from "react";
import { CustomerReqDto, CustomerResDto } from "@/types/customer/customer";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import customerService from "@/redux/api/customerApi";

interface UpdateCustomerDialogProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
    customer: CustomerResDto;
}

const UpdateCustomerDialog: React.FC<UpdateCustomerDialogProps> = ({
    isOpen,
    onClose,
    onSuccess,
    customer,
}) => {
    const [formData, setFormData] = useState<CustomerReqDto>({
        id: "",
        name: "",
        email: "",
        phoneNumber: "",
        address: "",
        TTLHRelatedIds: [], // Default value
        ttthidMain: "3fa85f64-5717-4562-b3fc-2c963f66afa6", // Default value
    });

    useEffect(() => {
        if (customer) {
            setFormData({
                id: customer.id,
                name: customer.name,
                email: customer.email,
                phoneNumber: customer.phoneNumber,
                address: customer.address,
                TTLHRelatedIds: [],
                ttthidMain: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
            });
        }
    }, [customer]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault(); // 🛑 Ngăn chặn reload trang

        try {
            await customerService.updateCustomerReq(customer.id, formData);
            onSuccess();
            onClose();
        } catch (error) {
            console.error("Error updating customer", error);
        }
    };

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="max-w-lg">
                <DialogHeader>
                    <DialogTitle>Edit Customer</DialogTitle>
                </DialogHeader>
                <form>
                    <div className="mt-4">
                        <input
                            className="disabled: bg-none "
                            type="text"
                            value={formData.id}
                            onChange={(e) => setFormData({ ...formData, id: e.target.value })}
                        />
                    </div>
                    <div className="mt-4">
                        <label className="block text-sm font-medium">Name</label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            className="w-full p-2 mt-1 border border-gray-300 rounded-md"
                        />
                    </div>
                    <div className="mt-4">
                        <label className="block text-sm font-medium">Email</label>
                        <input
                            type="email"
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                            className="w-full p-2 mt-1 border border-gray-300 rounded-md"
                        />
                    </div>
                    <div className="mt-4">
                        <label className="block text-sm font-medium">Phone</label>
                        <input
                            type="text"
                            value={formData.phoneNumber}
                            onChange={(e) => setFormData({ ...formData, phoneNumber: e.target.value })}
                            className="w-full p-2 mt-1 border border-gray-300 rounded-md"
                        />
                    </div>
                    <div className="mt-4">
                        <label className="block text-sm font-medium">Address</label>
                        <input
                            type="text"
                            value={formData.address}
                            onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                            className="w-full p-2 mt-1 border border-gray-300 rounded-md"
                        />
                    </div>

                    <DialogFooter className="mt-4">
                        <Button onClick={handleSubmit}>Update</Button>
                        <Button onClick={onClose} variant="outline">Cancel</Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    );
};

export default UpdateCustomerDialog;
