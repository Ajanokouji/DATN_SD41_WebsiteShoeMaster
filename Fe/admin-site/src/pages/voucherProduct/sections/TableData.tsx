import React, { useEffect, useState } from "react";
import Pagination from "@/components/Pagination";
import TableHeaderComponent from "@/components/TableHeader";
import TableRowComponent from "@/components/TableRow";
import { Table, TableBody } from "@/components/ui/table";
import TableProps from "@/types/common/table";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import { VoucherProductResDto } from "@/types/voucherProduct/voucherProduct";
import { ProductDetailResDto } from "@/types/product/product";
import DetailProductSheet from "@/pages/product/sections/DetailProductSheet";

import {
  deleteVoucherProduct,
  setVoucherProductPage,
  setVoucherProductPageSize,
  fetchVoucherProducts,
  fetchProductsByIds,
} from "@/redux/apps/voucherProduct/voucherProductSlice";

import {
  selectVoucherProductPagination,
  selectVoucherProducts,
} from "@/redux/apps/voucherProduct/voucherProductSelector";
import { formatVietnamTime } from "@/utils/format";

const VoucherProductTable = <T extends { id: string }>({
  headers,
  data,
  columns,
}: TableProps<T>) => (
  <div className="border border-gray-300 rounded-t-xl overflow-hidden">
      <div className="max-h-[58vh] max-w-full overflow-x-auto overflow-y-auto">
        <Table className="w-full">
          <TableHeaderComponent headers={headers} className="text-center" />
          <TableBody>
            {data.length ? (
              data.map((row, index) => (
                <TableRowComponent key={index} data={row} columns={columns} />
              ))
            ) : (
              <TableRowComponent
                key="empty-row"
                columns={columns}
                data={undefined}
              />
            )}
          </TableBody>
        </Table>
      </div>
    </div>
);

const VoucherProductsTable: React.FC<{ voucherId: string }> = ({ voucherId }) => {
  const dispatch = useAppDispatch();
  const [isOpenDetail, setIsOpenDetail] = useState(false);
  const [selectedDetailId, setSelectedDetailId] = useState<string | null>(
    null
  );
  const handleOpenDetail = (id: string) => {
    setIsOpenDetail(true);
    setSelectedDetailId(id);
  };

  const voucherProducts = useAppSelector(selectVoucherProducts);
  const pagination = useAppSelector(selectVoucherProductPagination);
  const [products, setProducts] = useState<ProductDetailResDto[]>([]);

  useEffect(() => {
    if (voucherId) {
      dispatch(
        fetchVoucherProducts({
          voucherId,
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [voucherId, dispatch, pagination.currentPage, pagination.pageSize]);

  useEffect(() => {
    const productIds = [...new Set(voucherProducts.map((v) => v.productId))];
    if (productIds.length > 0) {
      dispatch(fetchProductsByIds(productIds))
        .unwrap()
        .then((fetchedProducts) => {
          setProducts(fetchedProducts);
        })
        .catch((error) => {
          console.error("Error fetching products by IDs:", error);
        });
    }
  }, [dispatch, voucherProducts]);

  const renderDate = (date: string) => {
    if(date.includes("T")) {
      date = date.replace("T", " ").replace("Z", "");
    }
    return `${formatVietnamTime(date)}`;
  };

  const headers = [
    { label: "Tên sản phẩm / Mã sản phẩm" },
    { label: "Mã biến thể" },
    { label: "Giá" },
    { label: "Ngày áp dụng" },
    { label: "Thao tác"},
  ];

  const columns: {
    key?: keyof VoucherProductResDto;
    isActionColumn?: boolean;
    render?: (value: any, row?: VoucherProductResDto) => React.ReactNode;
    action?: (data: VoucherProductResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    {
      key: "productId",
      render: (value) => {
        const product = products.find((product) => product.id === value);

        if (product) {
          return (
            <div className="flex items-center">
              <img
                src={product?.imageUrl}
                alt={product?.name}
                className="w-20 border border-gray-300 rounded-lg p-1"
              />
              <div className="ml-2">
                <div className="text-start">{product?.name}</div>
                <div className="text-start text-sm text-gray-500">{product?.code}</div>
                <button onClick={() => handleOpenDetail(product.id)} className="flex items-start text-blue-500 hover:text-blue-700" >
                  Chi tiết
                </button>
              </div>
            </div>
          );
        }
        else{
          return (
            <div className="flex items-center gap-2">
              <span>Không xác định</span>
            </div>
          );
        }
      }
    },    
    {
      key: "varientProductId",
      render: (value) => {return (<div>{value? value : "Không xác định"}</div>) },
    },
    {
      key: "productId",
      render: (_: any, record?: VoucherProductResDto) => {
        const product = products.find((product) => product.id === _);
        if (product && product.variantObjs) {
          const variant = product.variantObjs.find( v => v.sku === record?.varientProductId);
          if (variant) {
            return <div>{variant.price} VNĐ</div>;
          } else {
            return <div>Không xác định</div>;
          }
        } else {
          return <div>Không xác định</div>;
        }
      },
    },
    {
      key: "lastModifiedOnDate",
      render: (value) => {
        return <div>{renderDate(value)}</div>
      }
    },
    {
      isActionColumn: true,
      deleteAction: (id: string) => {
        dispatch(deleteVoucherProduct(id));
      },
    },
  ];

  const handlePageChange = (newPage: number) => {
    dispatch(setVoucherProductPage(newPage));
  };

  const handlePageSizeChange = (newSize: number) => {
    dispatch(setVoucherProductPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <div className="max-w-full overflow-x-auto">
        <div className="max-w-full mx-auto">
          <VoucherProductTable<VoucherProductResDto>
            headers={headers}
            data={voucherProducts}
            columns={columns}
          />
        </div>
      </div>

      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />

      {isOpenDetail && selectedDetailId && (
        <DetailProductSheet
          productId={selectedDetailId}
          isOpen={isOpenDetail}
          onClose={() => setIsOpenDetail(false)}
        />
      )}
    </section>
  );
};

export default VoucherProductsTable;