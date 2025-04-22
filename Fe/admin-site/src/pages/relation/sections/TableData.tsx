import React, { useEffect } from "react";
import Pagination from "@/components/Pagination";
import { useAppDispatch } from "@/hooks/use-app-dispatch";
import { useAppSelector } from "@/hooks/use-app-selector";
import {
  createRelation,
  deleteRelation,
  fetchRelations,
} from "@/redux/apps/relation/relationSlice";
import { selectRelations } from "@/redux/apps/relation/relationSelector";
import {
  selectPagination,
  selectProducts,
} from "@/redux/apps/product/productSelector";
import { ProductResDto } from "@/types/product/product";
import { formatVietnamTime } from "@/utils/format";
import {
  fetchProducts,
  setPage,
  setPageSize,
} from "@/redux/apps/product/productSlice";
import RelationTable from "./RelationTable";
import RelationReqDto from "@/types/category/relation";
import { Input } from "@/components/ui/input";

interface RelationsTableProps {
  selectedCategoryId: string | null;
}

const RelationsTable: React.FC<RelationsTableProps> = ({
  selectedCategoryId,
}) => {
  const dispatch = useAppDispatch();
  const relations = useAppSelector(selectRelations);
  const products = useAppSelector(selectProducts);
  const pagination = useAppSelector(selectPagination);

  const renderCreatedDate = (value: string) => {
    return formatVietnamTime(value);
  };

  const handleToggleRelation = (productId: string) => {
    if (!selectedCategoryId) return;
   
    const existingRelation = relations.find(
      (relation) => relation.idProduct === productId && relation.categoriesId === selectedCategoryId
    );

    if (existingRelation) {
      dispatch(deleteRelation(existingRelation.id));
    } else {
      const productToRelate = products.find(
        (product) => product.id === productId
      );

      if (productToRelate) {
        const relationData: RelationReqDto = {
          categoriesId: selectedCategoryId,
          idProduct: productId,
          productName: productToRelate.name,
          categoryName: "",
          description: "",
          relationType: "default",
          status: "active",
          createdByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          createdOnDate: new Date().toISOString(),
          lastModifiedByUserId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
          lastModifiedOnDate: new Date().toISOString(),
          isdeleted: false,
          isPublish: true,
          order: 0,
        };

        dispatch(createRelation(relationData));
      }
    }
  };

  const headers = [
    { label: "Select", className: "text-center" },
    { label: "Code", className: "text-center" },
    { label: "Name" },
    { label: "Image" },
    { label: "Description" },
    { label: "Status" },
    { label: "Create At" },
    { label: " " },
  ];

  const columns: {
    key?: keyof ProductResDto;
    className?: string;
    isActionColumn?: boolean;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    render?: (value: any, data: ProductResDto) => React.ReactNode;
    action?: (data: ProductResDto) => void;
    deleteAction?: (id: string) => void;
  }[] = [
    {
      key: "id",
      render: (_, product) => {
        return (
          <Input
            type="checkbox"
            checked={relations.some(
              (relation) => relation.idProduct === product.id
            )}
            onChange={() => handleToggleRelation(product.id)}
          />
        );
      },
      className: "text-center",
    },
    { key: "code", className: "text-center" },
    { key: "name" },
    { key: "imageUrl" },
    { key: "description" },
    { key: "status" },
    {
      key: "createdOnDate",
      render: renderCreatedDate,
    },
    {
      isActionColumn: true,
      className: "text-center",
      action: (category) => {
        console.log("Performing action for:", category);
      },
      deleteAction: (id: string) => {
        dispatch(deleteRelation(id));
      },
    },
  ];

  useEffect(() => {
    if (selectedCategoryId) {
      dispatch(
        fetchRelations({
          IdDanhMuc: selectedCategoryId,
          CurrentPage: pagination.currentPage,
          PageSize: pagination.pageSize,
        })
      );
    }
  }, [
    dispatch,
    pagination.currentPage,
    pagination.pageSize,
    selectedCategoryId,
  ]);

  useEffect(() => {
    dispatch(
      fetchProducts({
        CurrentPage: pagination.currentPage,
        PageSize: pagination.pageSize,
      })
    );
  }, [dispatch, pagination.currentPage, pagination.pageSize]);

  // Xử lý khi thay đổi trang
  const handlePageChange = (newPage: number) => {
    dispatch(setPage(newPage));
  };

  // Xử lý khi thay đổi số lượng sản phẩm trên trang
  const handlePageSizeChange = (newSize: number) => {
    dispatch(setPageSize(newSize));
  };

  return (
    <section className="mt-10">
      <RelationTable<ProductResDto>
        headers={headers}
        data={products}
        columns={columns}
      />
      <Pagination
        currentPage={pagination.currentPage}
        totalPages={pagination.totalPages}
        pageSize={pagination.pageSize}
        totalRecords={pagination.totalRecords}
        onPageChange={handlePageChange}
        onPageSizeChange={handlePageSizeChange}
      />
    </section>
  );
};

export default RelationsTable;
