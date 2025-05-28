// import { useAppSelector } from "@/hooks/use-app-selector";
// import { RootState } from "@/redux/store";
import Layout from "@/layout";
import { lazy, Suspense } from "react";
import { Navigate, Outlet, useRoutes } from "react-router-dom";
// Import useAppSelector và RootState để kiểm tra trạng thái đăng nhập
import { useAppSelector } from "@/hooks/use-app-selector";
import type { RootState } from "@/redux/store"; // Sử dụng import type

export const LoadingPage = lazy(() => import("@/pages/shared/LoadingPage"));
export const LoginPage = lazy(() => import("@/pages/login")); // Component trang đăng nhập
// Giả sử bạn có một trang Dashboard sau khi đăng nhập
export const DashboardPage = lazy(() => import("@/pages/dashboard")); // <-- Bạn cần đảm bảo file src/pages/dashboard/index.tsx tồn tại

export const CategoryPage = lazy(() => import("@/pages/category"));
export const ProductPage = lazy(() => import("@/pages/product"));
export const RelationPage = lazy(() => import("@/pages/relation"));
export const VoucherPage = lazy(() => import("@/pages/voucher"));
export const ContactPage = lazy(() => import("@/pages/contact"));
export const BillPage = lazy(() => import("@/pages/bill"));
export const FileManagerPage = lazy(() => import("@/pages/file-manager"));
export const CustomerPage = lazy(() => import("@/pages/customer"));
export const VoucherProductPage = lazy(() => import("@/pages/voucherProduct"));
export const VoucherUserPage = lazy(() => import("@/pages/voucherUser"));
export const ContentBasePage = lazy(() => import("@/pages/contentBase"));
export const CommentPage = lazy(() => import("@/pages/comment"));
// export const LoginPage = lazy(() => import("../pages/auth/Login"));
// export const RegisterPage = lazy(() => import("../pages/auth/Register"));
export const UserPage = lazy(() => import("@/pages/user"));
export const Page404 = lazy(() => import("../pages/shared/NotFoundPage"));

// ----------------------------------------------------------------------

// Hook để lấy trạng thái đăng nhập từ Redux store (login slice)
const useAuth = () => {
  // Sử dụng useAppSelector và RootState để truy cập đúng trạng thái
  return useAppSelector((state: RootState) => state.login.isAuthenticated);
};

export default function Router() {
  const isAuthenticated = useAuth();

  // Component Protected Route: chỉ cho phép truy cập nếu đã đăng nhập
  const PrivateRoute = ({ children }: { children: JSX.Element }) => {
    return isAuthenticated ? children : <Navigate to="/login" replace />;
  };

  const routes = useRoutes([
    // Route cho trang đăng nhập: KHÔNG BỌC TRONG LAYOUT
    {
      path: "/login",
      element: isAuthenticated ? <Navigate to="/" replace /> : <LoginPage />,
    },
    // Route gốc: kiểm tra đăng nhập và chuyển hướng
    {
      path: "/",
      element: isAuthenticated ? <Navigate to="/dashboard" replace /> : <Navigate to="/login" replace />,
    },
    // Route cho Dashboard (trang chính sau đăng nhập)
    {
      path: "/dashboard",
      element: (
        <PrivateRoute>
          {
            <>
              {/* Chỉ cho phép truy cập nếu đã đăng nhập */}
              <Layout> {/* Bọc trong Layout chính */}
                <Suspense fallback={<LoadingPage />}>
                  <DashboardPage />
                </Suspense>
              </Layout>
            </>
          }
        </PrivateRoute>
      ),
    },
    // Các route chức năng khác: BỌC TRONG LAYOUT VÀ PRIVATE ROUTE
    {
      element: (
        <PrivateRoute>
          <>
            {/* Chỉ cho phép truy cập nếu đã đăng nhập */}
            <Layout> {/* Bọc trong Layout chính */}
              <Suspense fallback={<LoadingPage />}>
                <Outlet /> {/* Outlet để render các children routes */}
              </Suspense>
            </Layout>
          </>
        </PrivateRoute>
      ),
      children: [
        // { element: <DashboardPage />, index: true }, // Xóa dòng này nếu dùng route /dashboard riêng
        {
          path: "category",
          element: <CategoryPage />,
        },
        {
          path: "product",
          element: <ProductPage />,
        },
        {
          path: "relation",
          element: <RelationPage />,
        },
        {
          path: "voucher",
          element: <VoucherPage />,
        },
        {
          path: "employee",
          element: <ContactPage />,
        },
        {
          path: "customer",
          element: <CustomerPage />,
        },
        {
          path: "bill",
          element: <BillPage />,
        },
        {
          path: "file-manager",
          element: <FileManagerPage />,
        },
        {
          path: "voucher-product/:voucherId",
          element: <VoucherProductPage />,
        },
        {
          path: "voucher-user/:voucherId",
          element: <VoucherUserPage />,
        },
        {
          path: "contentBase",
          element: <ContentBasePage />,
        },
        {
          path: "comment/:objectId",
          element: <CommentPage />,
        },
        {
          path: "user",
          element: <UserPage />,
        },
      ],
    },
    // {
    //   path: "login",
    //   element: isAuthenticated ? <Navigate to="/" /> : <LoginPage />,
    // },
    // {
    //   path: "register",
    //   element: isAuthenticated ? <Navigate to="/" /> : <RegisterPage />,
    // },
    // Route 404 cho các path không khớp
    {
      path: "*",
      element: <Page404 />,
    },
  ]);

  return routes;
}
