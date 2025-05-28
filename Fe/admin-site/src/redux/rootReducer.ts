import { combineReducers } from "@reduxjs/toolkit";
// import authReducer from "../redux/apps/auth/authSlice";
import categoryReducer from "../redux/apps/category/categorySlice";
import productReducer from "../redux/apps/product/productSlice";
import relationReducer from "../redux/apps/relation/relationSlice";
import voucherReducer from "../redux/apps/voucher/voucherSlice";
import contactReducer from "../redux/apps/contact/contactSlice";
import billReducer from "../redux/apps/bill/billSlice";
import userReducer from "../redux/apps/user/userSlice";
import customerReducer from "../redux/apps/customer/customerSlice";
import messageReducer from "../redux/apps/message/messageSlice";
import fileReducer from "./apps/file/fileSlice";
import VoucherProductReducer from "../redux/apps/voucherProduct/voucherProductSlice";
import VoucherUserReducer from "../redux/apps/voucherUser/voucherUserSlice";
import contentBaseReducer from "./apps/contentBase/contentBaseSlice";
import loginReducer from "./apps/login/loginSlice";
import commentReducer from "./apps/comment/commentSlice";

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
  file: fileReducer,
  voucherProduct: VoucherProductReducer,
  voucherUser: VoucherUserReducer,
  contentBase: contentBaseReducer,
  login: loginReducer,
  comment: commentReducer,
  user: userReducer
});

export default rootReducer;
