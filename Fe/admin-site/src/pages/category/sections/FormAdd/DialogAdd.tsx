import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import React from "react";
// import TableForm from "./TableForm";
import ContactInfoForm from "./ContactInfoForm";
import AddressInfoForm from "./AddressInfoForm";
import BasicInfoForm from "./BasicInfoForm";
// import { useAppDispatch } from "@/hooks/use-app-dispatch";
// import { createCustomer } from "@/redux/apps/customer/customerSlice";
// import * as Yup from "yup";
// import CustomerReqDto from "@/types/category/category";
// import { Form, Formik } from "formik";

// const customerValidationSchema = Yup.object({
//   full_name: Yup.string().required("Họ tên khách hàng là bắt buộc"),
// });

interface AddCustomerDialogProps {
  isOpen: boolean;
  onClose: () => void;
}

const AddCustomerDialog: React.FC<AddCustomerDialogProps> = ({
  isOpen,
  onClose,
}) => {
  // const dispatch = useAppDispatch();

  // const handleSubmit = async (values: CustomerReqDto) => {
  //   try {
  //     // await dispatch(createCustomer(values));
  //     console.log("Create customer success", values);
  //   } catch (error) {
  //     console.error("Create customer error details:", error);
  //   }
  // };

  // const initialValues: CustomerReqDto = {
  //   full_name: "",
  //   gender: "Nam",
  //   date_of_birth: "",
  //   status: 0,
  //   source: 0,
  //   phone_number: "",
  //   email: "",
  //   address: "",
  //   city: "",
  //   district: "",
  //   ward: "",
  //   detailed_info: "",
  //   follow_up_date: "",
  //   follow_down_date: "",
  //   notes: "",
  //   comments: [
  //     {
  //       time: "",
  //       title: "",
  //       status_id: 0,
  //     },
  //   ],
  //   service: [],
  //   social_media: 0,
  // };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogDescription />
      <DialogContent className="max-w-7xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-xl font-semibold text-yellow-700">
            Tạo khách hàng
          </DialogTitle>
        </DialogHeader>
        {/* <Formik
          initialValues={initialValues}
          validationSchema={customerValidationSchema}
          onSubmit={handleSubmit}
        >
          {({ values, errors, touched, handleChange, setFieldValue }) => (
            <Form className="space-y-5"> */}
              <div className="container space-y-5">
                {/* Basic Info */}
                <BasicInfoForm
                  // values={values}
                  // errors={errors}
                  // touched={touched}
                  // handleChange={handleChange}
                  // setFieldValue={setFieldValue}
                />

                <div className="border-b-2 pt-6"></div>

                {/* Contact Info */}
                <h5 className="text-lg font-semibold pt-4">
                  Thông tin liên hệ
                </h5>
                <ContactInfoForm
                  // values={values}
                  // errors={errors}
                  // touched={touched}
                  // handleChange={handleChange}
                  // setFieldValue={setFieldValue}
                />
                <AddressInfoForm
                  // values={values}
                  // errors={errors}
                  // touched={touched}
                  // handleChange={handleChange}
                  // setFieldValue={setFieldValue}
                />
              </div>

              {/* Care Info Table */}
              <div className="mt-6">
                <h3 className="text-lg font-medium">
                  Thông tin chăm sóc khách hàng
                </h3>
                {/* <TableForm
                  values={values.comments}
                  handleChange={handleChange}
                  setFieldValue={setFieldValue}
                /> */}
              </div>

              <DialogFooter className="mt-6 flex justify-between">
                <Button variant="outline" onClick={onClose}>
                  Hủy
                </Button>
                <Button type="submit">Xác nhận</Button>
              </DialogFooter>
            {/* </Form>
          )}
        </Formik> */}
      </DialogContent>
    </Dialog>
  );
};

export default AddCustomerDialog;
