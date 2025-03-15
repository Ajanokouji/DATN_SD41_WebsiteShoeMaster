// types/category/category.ts

export interface CategoryReqDto {
    name: string;
    description: string;
}

export interface CategoryResDto {
    id: string;
    name: string;
    description: string;
    createdAt: string;
    updatedAt: string;
}

export interface CategoryDetailResDto extends CategoryResDto {
    itemsCount: number;
}
