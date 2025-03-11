import { combineReducers } from "@reduxjs/toolkit";
// import authReducer from "../redux/apps/auth/authSlice";
// import customerReducer from "../redux/apps/customer/customerSlice";

import messageReducer from "../redux/apps/message/messageSlice";

const rootReducer = combineReducers({
//   auth: authReducer,
//   customer: customerReducer,

  messages: messageReducer,
});

export default rootReducer;
