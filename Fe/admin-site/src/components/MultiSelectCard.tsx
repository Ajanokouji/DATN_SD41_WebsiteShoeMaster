import React, { useState } from "react";

interface Item {
  id: number;
  label: string;
}

interface MultiSelectCardProps {
  selectedItems: number[];
  onSelectionChange: (selected: number[]) => void;
}
const MultiSelectCard: React.FC<MultiSelectCardProps> = ({
  selectedItems,
  onSelectionChange,
}) => {
  const availableOptions: Item[] = [
    { id: 3, label: "Liệu pháp spa" },
    { id: 4, label: "Chăm sóc da mặt" },
    { id: 5, label: "Massage chân" },
  ];

  const [items, setItems] = useState<Item[]>([]);
  const [selectedOption, setSelectedOption] = useState<number | null>(null);

  const handleAddItem = () => {
    if (selectedOption !== null) {
      const selected = availableOptions.find(
        (opt) => opt.id === selectedOption
      );
      if (selected && !items.find((item) => item.id === selected.id)) {
        setItems((prev) => [...prev, selected]);
        onSelectionChange([...selectedItems, selected.id]);
      }
      setSelectedOption(null);
    }
  };

  const toggleSelectItem = (id: number) => {
    const newSelected = selectedItems.includes(id)
      ? selectedItems.filter((itemId) => itemId !== id)
      : [...selectedItems, id];
    onSelectionChange(newSelected);
  };

  return (
    <div className="p-4 border rounded-lg bg-green-50">
      <label className="block mb-2 text-sm font-medium text-gray-700">
        Sản phẩm quan tâm <span className="text-red-500">*</span>
      </label>
      <div className="flex flex-wrap gap-2">
        {items.map((item) => (
          <button
            type="button"
            key={item.id}
            className={`px-3 py-2 text-sm font-medium rounded-lg ${
              selectedItems.includes(item.id)
                ? "bg-yellow-600 text-white"
                : "bg-yellow-300 text-yellow-900"
            }`}
            onClick={() => toggleSelectItem(item.id)}
          >
            {item.label}
          </button>
        ))}
      </div>
      <div className="mt-4 flex items-center gap-2">
        <select
          className="flex-1 p-2 border rounded-lg"
          value={selectedOption || ""}
          onChange={(e) => setSelectedOption(Number(e.target.value))}
        >
          <option value="" disabled>
            Chọn sản phẩm từ danh sách...
          </option>
          {availableOptions.map((option) => (
            <option key={option.id} value={option.id}>
              {option.label}
            </option>
          ))}
        </select>
        <button
          type="button"
          className="px-4 py-2 text-sm font-medium text-white bg-blue-500 rounded-lg hover:bg-blue-600"
          onClick={handleAddItem}
          disabled={selectedOption === null}
        >
          Thêm
        </button>
      </div>
    </div>
  );
};

export default MultiSelectCard;
