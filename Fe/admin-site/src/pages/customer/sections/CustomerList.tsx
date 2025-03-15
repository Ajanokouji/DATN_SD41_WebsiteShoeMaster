// src/components/Customer/CustomerList.tsx

import React, { useEffect, useState } from "react";
import { CustomerResDto } from "@/types/customer/customer";
import { PaginationParams } from "@/types/common/pagination";
import customerService from "@/redux/api/customerApi";
import StatCard from "@/components/StatCard";
import { RiDeleteBin3Line } from "react-icons/ri";
import { LuSquarePen } from "react-icons/lu";
import Pagination from "@/components/Pagination";
import { Button } from "@/components/ui/button";
import { PlusCircle, UserPlus, UsersIcon, UserX } from "lucide-react";
import CreateCustomerModal from "./CreateCustomerModal";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import UpdateCustomerDialog from "./UpdateCustomerDialog";
import DeleteCustomerDialog from "./DeleteCustomerDialog";

// Define the correct PaginatedResponse structure based on your API
interface PaginatedResponse<T> {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  numberOfRecords: number;
  totalRecords: number;
  content: T[];
}

const CustomerList: React.FC = () => {
  // Update state to match the correct response structure
  const [customers, setCustomers] = useState<PaginatedResponse<CustomerResDto>>({
    currentPage: 1,
    totalPages: 1,
    pageSize: 20,
    numberOfRecords: 0,
    totalRecords: 0,
    content: [],
  });

  const [loading, setLoading] = useState(false);
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [deleteCustomerId, setDeleteCustomerId] = useState<string | null>(null);
  const [editCustomer, setEditCustomer] = useState<CustomerResDto | null>(null);
  const [paginationParams, setPaginationParams] = useState<PaginationParams>({
    CurrentPage: 1,
    PageSize: 20,
  });

  // Fetch customer data on page change
  const fetchCustomers = async (params: PaginationParams) => {
    setLoading(true);
    try {
      const response = await customerService.getCustomers(params);
      setCustomers(response.data);
    } catch (error) {
      console.error("Failed to fetch customers:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCustomers(paginationParams);
  }, [paginationParams]);

  const handlePageChange = (page: number) => {
    setPaginationParams((prev) => ({ ...prev, CurrentPage: page }));
  };

  const handlePageSizeChange = (pageSize: number) => {
    setPaginationParams((prev) => ({ ...prev, PageSize: pageSize }));
  };

  const handleDelete = (id: string) => {
    setDeleteCustomerId(id);
    setIsDeleteModalOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (deleteCustomerId) {
      await customerService.deleteCustomerReq(deleteCustomerId);
      setIsDeleteModalOpen(false);
    }
  };

  const handleEdit = (customer: CustomerResDto) => {
    setEditCustomer(customer);
    setIsUpdateModalOpen(true);
  };

  const openCreateModal = () => {
    setIsCreateModalOpen(true);
  };

  const closeCreateModal = () => {
    setIsCreateModalOpen(false);
  };

  // Refresh customer list after successful creation
  const handleCustomerCreated = () => {
    fetchCustomers(paginationParams);
  };

  return (
    <div className="p-6">
      {/* Header with Title and Add Button */}
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Customer Management</h1>
        <Button onClick={openCreateModal} className="flex items-center gap-2">
          <PlusCircle size={16} />
          <span>Add Customer</span>
        </Button>
      </div>

      {/* StatCard */}
      <div className="flex gap-6 mb-6">
        <StatCard name="Total Customers" icon={UsersIcon}
          value={customers.totalRecords} color='#6366F1' />

        <StatCard name="New Customers" icon={UserPlus} value={customers.totalRecords} color='#10B981' />
        <StatCard name="Purchase Rate" icon={UserX} value='2.4%' color='#EF4444' />
      </div>
      {/* Table */}
      <div className="mb-4 bg-white shadow-md rounded-lg overflow-hidden">
        {loading ? (
          <div className="p-4 text-center">
            <div className="flex items-center justify-center space-x-2">
              <div className="w-4 h-4 rounded-full animate-pulse bg-indigo-600"></div>
              <div className="w-4 h-4 rounded-full animate-pulse bg-indigo-600 delay-75"></div>
              <div className="w-4 h-4 rounded-full animate-pulse bg-indigo-600 delay-150"></div>
            </div>
            <p className="mt-2 text-sm text-gray-500">Loading customers...</p>
          </div>
        ) : customers.content.length === 0 ? (
          <div className="p-8 text-center text-gray-500">
            <p className="text-lg font-medium">No customers found</p>
            <p className="text-sm mt-1">Add your first customer using the button above.</p>
          </div>
        ) : (
          <div className="border-b text-center">
            <Table className="w-full">
              <TableHeader>
                <TableRow className="bg-gray-50 hover:bg-gray-50 text-center">
                  <TableHead className="font-medium">Code</TableHead>
                  <TableHead className="font-medium">Name</TableHead>
                  <TableHead className="font-medium">Email</TableHead>
                  <TableHead className="font-medium">Phone</TableHead>
                  <TableHead className="font-medium text-right">Address</TableHead>
                  <TableHead className="font-medium text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {customers.content.map((customer: CustomerResDto) => (
                  <TableRow key={customer.id} className="hover:bg-gray-50">
                    <TableCell className="font-medium">{customer.code}</TableCell>
                    <TableCell>{customer.name}</TableCell>
                    <TableCell>{customer.email}</TableCell>
                    <TableCell>{customer.phoneNumber}</TableCell>
                    <TableCell className="text-right">{customer.address}</TableCell>
                    <TableCell className="text-right">
                      <div className="flex justify-end gap-2 items-center">
                        <Button
                          onClick={() => handleEdit(customer)}
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-indigo-600 hover:text-indigo-900 hover:bg-indigo-50"
                        >
                          <LuSquarePen size={18} />
                        </Button>
                        <Button
                          onClick={() => handleDelete(customer.id)}
                          variant="ghost"
                          size="icon"
                          className="h-8 w-8 text-red-600 hover:text-red-900 hover:bg-red-50"
                        >
                          <RiDeleteBin3Line size={18} />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </div>

      {/* Pagination */}
      {customers.content.length > 0 && (
        <div className="mt-4">
          <Pagination
            currentPage={customers.currentPage}
            totalPages={customers.totalPages}
            pageSize={customers.pageSize}
            totalRecords={customers.totalRecords}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
        </div>
      )}

      {/* Create Customer Modal */}
      <CreateCustomerModal
        isOpen={isCreateModalOpen}
        onClose={closeCreateModal}
        onSuccess={handleCustomerCreated}
      />

      {/* Update Customer Dialog */}
      <UpdateCustomerDialog
        isOpen={isUpdateModalOpen}
        onClose={() => setIsUpdateModalOpen(false)}
        onSuccess={() => fetchCustomers(paginationParams)}
        customer={editCustomer!} // Pass the selected customer to edit
      />

      {/* Delete Customer Confirmation Dialog */}
      <DeleteCustomerDialog
        isOpen={isDeleteModalOpen}
        onClose={() => setIsDeleteModalOpen(false)}
        onConfirm={handleDeleteConfirm}
        customerName={deleteCustomerId || "Customer"}
      />
    </div>
  );
};

export default CustomerList;