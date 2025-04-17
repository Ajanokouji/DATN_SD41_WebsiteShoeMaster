import React, { useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Plus, Trash2, Image } from "lucide-react";
import { Label } from "@/components/ui/label";

interface MediaListProps {
  mediaUrls: string[];
  isEditing: boolean;
  onAdd: (mediaUrl: string) => void;
  onRemove: (index: number) => void;
}

export const MediaList: React.FC<MediaListProps> = ({
  mediaUrls,
  isEditing,
  onAdd,
  onRemove,
}) => {
  const [newMediaUrl, setNewMediaUrl] = useState("");

  const handleAddMedia = () => {
    if (newMediaUrl.trim()) {
      onAdd(newMediaUrl.trim());
      setNewMediaUrl("");
    }
  };

  return (
    <div className="space-y-4 mt-6">
      <div className="flex items-center justify-between">
        <Label className="text-lg font-medium">Ảnh</Label>
      </div>

      {isEditing && (
        <div className="flex space-x-2 items-end">
          <div className="flex-1">
            <Input
              value={newMediaUrl}
              onChange={(e) => setNewMediaUrl(e.target.value)}
              placeholder="Enter media URL"
            />
          </div>
          <Button type="button" variant="outline" onClick={handleAddMedia}>
            <Plus className="h-4 w-4 mr-2" />
            Add
          </Button>
        </div>
      )}

      {mediaUrls.length > 0 ? (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
          {mediaUrls.map((url, index) => (
            <div
              key={index}
              className="relative border rounded-md overflow-hidden group"
            >
              <div className="aspect-square bg-gray-100 flex items-center justify-center">
                {url ? (
                  <img
                    src={url}
                    alt={`Media ${index + 1}`}
                    className="w-full h-full object-cover"
                    onError={(e) => {
                      e.currentTarget.src = ""; // Clear src on error
                      e.currentTarget.classList.add("error");
                    }}
                  />
                ) : (
                  <Image className="h-12 w-12 text-gray-400" />
                )}
              </div>
              {isEditing && (
                <Button
                  variant="destructive"
                  size="sm"
                  className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity"
                  onClick={() => onRemove(index)}
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              )}
            </div>
          ))}
        </div>
      ) : (
        <div className="text-center p-4 border rounded-md bg-gray-50">
          No media available
          {!isEditing && mediaUrls.length === 0 && (
            <div className="flex justify-center items-center h-24">
              <Image className="h-12 w-12 text-gray-400" />
            </div>
          )}
        </div>
      )}
    </div>
  );
};
