// import { useAppSelector } from "@/hooks/use-app-selector";
// import { RootState } from "@/redux/store";
import Layout from "@/layout";
import { lazy, Suspense } from "react";
import { Navigate, Outlet, useRoutes } from "react-router-dom";

export const LoadingPage = lazy(() => import("@/pages/shared/LoadingPage"));
export const IndexPage = lazy(() => import("@/pages/dashboard"));

export const CategoryPage = lazy(() => import("@/pages/category"));
export const ProductPage = lazy(() => import("@/pages/product"));
export const RelationPage = lazy(() => import("@/pages/relation"));
export const VoucherPage = lazy(() => import("@/pages/voucher"));
export const ContactPage = lazy(() => import("@/pages/contact"));
export const BillPage = lazy(() => import("@/pages/bill"));

export const CustomerPage = lazy(() => import("@/pages/customer"));
// export const LoginPage = lazy(() => import("../pages/auth/Login"));
// export const RegisterPage = lazy(() => import("../pages/auth/Register"));
export const Page404 = lazy(() => import("../pages/shared/NotFoundPage"));

// ----------------------------------------------------------------------

// const useAuth = () => {
//   return useAppSelector((state: RootState) => state.auth.isAuthenticated);
// };

export default function Router() {
  //   const isAuthenticated = useAuth();

  const PrivateRoute = ({ children }: { children: JSX.Element }) => {
    // const isAuthenticated = useAuth();
    const isAuthenticated = true;
    return isAuthenticated ? children : <Navigate to="/login" />;
  };

  const routes = useRoutes([
    {
      element: (
        <PrivateRoute>
          <Layout>
            <Suspense fallback={<LoadingPage />}>
              <Outlet />
            </Suspense>
          </Layout>
        </PrivateRoute>
      ),
      children: [
        {
          element: <IndexPage />,
          index: true,
        },
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
          path: "contact",
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

    {
      path: "*",
      element: <Page404 />,
    },
  ]);

  return routes;
}
