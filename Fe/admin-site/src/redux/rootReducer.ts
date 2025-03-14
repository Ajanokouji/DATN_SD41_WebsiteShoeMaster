import { combineReducers } from "@reduxjs/toolkit";
// import authReducer from "../redux/apps/auth/authSlice";
import categoryReducer from "../redux/apps/category/categorySlice";
import productReducer from "../redux/apps/product/productSlice";

import messageReducer from "../redux/apps/message/messageSlice";

const rootReducer = combineReducers({
//   auth: authReducer,
  category: categoryReducer,
  product: productReducer,

  messages: messageReducer,
});

export default rootReducer;
