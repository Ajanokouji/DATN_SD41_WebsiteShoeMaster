import React from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { FieldSelectionValue } from "@/types/product/form";

interface FieldSelectionValuesListProps {
  fieldSelectionValues: FieldSelectionValue[];
  isEditing: boolean;
  onValueChange: (index: number, field: keyof FieldSelectionValue, value: string | number) => void;
  onAdd: () => void;
  onRemove: (index: number) => void;
}

export const FieldSelectionValuesList: React.FC<FieldSelectionValuesListProps> = ({
  fieldSelectionValues,
  isEditing,
  onValueChange,
  onAdd,
  onRemove
}) => {
  return (
    <div className="space-y-2">
      <h3 className="font-medium">Thuộc Tính sản phẩm</h3>
      {fieldSelectionValues.map((field, index) => (
        <div key={index} className="flex space-x-2">
          <Input
            value={field.value}
            onChange={(e) => onValueChange(index, 'value', e.target.value)}
            placeholder="Giá trị"
            disabled={!isEditing}
          />
          <Input
            value={field.code}
            onChange={(e) => onValueChange(index, 'code', e.target.value)}
            placeholder="Mã"
            disabled={!isEditing}
          />
          <Input
            value={field.key}
            onChange={(e) => onValueChange(index, 'key', e.target.value)}
            placeholder="Key"
            disabled={!isEditing}
          />
          <Input
            value={field.order}
            onChange={(e) => onValueChange(index, 'order', Number(e.target.value))}
            placeholder="Thứ tự"
            disabled={!isEditing}
          />
          <Input
            value={field.fieldName || ''}
            onChange={(e) => onValueChange(index, 'fieldName', e.target.value)}
            placeholder="Field Name"
            disabled={!isEditing}
          />
          {isEditing && (
            <Button type="button" onClick={() => onRemove(index)}>
              Xóa
            </Button>
          )}
        </div>
      ))}
      {isEditing && (
        <Button
          type="button"
          onClick={onAdd}
        >
          Thêm
        </Button>
      )}
    </div>
  );
};