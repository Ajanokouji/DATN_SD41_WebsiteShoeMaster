import { combineReducers } from "@reduxjs/toolkit";
// import authReducer from "../redux/apps/auth/authSlice";
import categoryReducer from "../redux/apps/category/categorySlice";
import productReducer from "../redux/apps/product/productSlice";
import relationReducer from "../redux/apps/relation/relationSlice";
import voucherReducer from "../redux/apps/voucher/voucherSlice";
import contactReducer from "../redux/apps/contact/contactSlice";
import billReducer from "../redux/apps/bill/billSlice";
import customerReducer from "../redux/apps/customer/customerSlice";

import messageReducer from "../redux/apps/message/messageSlice";

const rootReducer = combineReducers({
//   auth: authReducer,
  category: categoryReducer,
  product: productReducer,
  relation: relationReducer,
  voucher: voucherReducer,
  contact: contactReducer,
  bill: billReducer,
  customer: customerReducer,
  
  messages: messageReducer,
});

export default rootReducer;
