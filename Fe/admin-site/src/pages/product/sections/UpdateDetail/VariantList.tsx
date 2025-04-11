import React from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Plus, Trash2 } from "lucide-react";
import { Label } from "@/components/ui/label";

interface VariantListProps {
  variants: {
    id: string;
    productId: string;
    size: string;
    sizeType: string;
    lowestAsk: number;
  }[];
  isEditing: boolean;
  onValueChange: (index: number, field: string, value: string | number) => void;
  onAdd: () => void;
  onRemove: (index: number) => void;
}

export const VariantList: React.FC<VariantListProps> = ({
  variants,
  isEditing,
  onValueChange,
  onAdd,
  onRemove,
}) => {
  return (
    <div className="space-y-4 mt-6">
      <div className="flex items-center justify-between">
        <Label className="text-lg font-medium">Biến thể sản phẩm</Label>
        {isEditing && (
          <Button type="button" variant="outline" size="sm" onClick={onAdd}>
            <Plus className="h-4 w-4 mr-2" />
            Thêm Variant
          </Button>
        )}
      </div>

      {variants.length > 0 ? (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Size</TableHead>
              <TableHead>Size Type</TableHead>
              <TableHead>Lowest Ask</TableHead>
              {isEditing && (
                <TableHead className="w-[100px]">Actions</TableHead>
              )}
            </TableRow>
          </TableHeader>
          <TableBody>
            {variants.map((variant, index) => (
              <TableRow key={index}>
                <TableCell>
                  {isEditing ? (
                    <Input
                      value={variant.size}
                      onChange={(e) =>
                        onValueChange(index, "size", e.target.value)
                      }
                    />
                  ) : (
                    variant.size
                  )}
                </TableCell>
                <TableCell>
                  {isEditing ? (
                    <Input
                      value={variant.sizeType}
                      onChange={(e) =>
                        onValueChange(index, "sizeType", e.target.value)
                      }
                    />
                  ) : (
                    variant.sizeType
                  )}
                </TableCell>
                <TableCell>
                  {isEditing ? (
                    <Input
                      type="number"
                      value={variant.lowestAsk}
                      onChange={(e) =>
                        onValueChange(
                          index,
                          "lowestAsk",
                          Number(e.target.value)
                        )
                      }
                    />
                  ) : (
                    variant.lowestAsk
                  )}
                </TableCell>
                {isEditing && (
                  <TableCell>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => onRemove(index)}
                    >
                      <Trash2 className="h-4 w-4 text-red-500" />
                    </Button>
                  </TableCell>
                )}
              </TableRow>
            ))}
          </TableBody>
        </Table>
      ) : (
        <div className="text-center p-4 border rounded-md bg-gray-50">
          Không có biến thể sản phẩm
          {isEditing && (
            <div className="mt-2">
              <Button type="button" variant="outline" size="sm" onClick={onAdd}>
                <Plus className="h-4 w-4 mr-2" />
                Thêm biến thể
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
