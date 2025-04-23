import React, { useEffect, useRef, useState } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { 
  Upload, 
  FolderOpen,  
  Edit,  
  Download, 
  Trash2,
} from "lucide-react";
import { Input } from "@/components/ui/input";
import { ContextMenu } from "@/components/ui/context-menu";
import fileApi from "@/redux/api/fileApi";
import type { FileItem, FileFilterRequest } from "@/types/file";
import FileManagerModal from "@/components/FileManagerModal";

// Types
interface ContextMenuState {
  show: boolean;
  x: number;
  y: number;
  fileId: string | null;
}

interface FileManagerProps {
  onSelectImage?: (file: FileItem) => void;
}

const FileManager : React.FC<FileManagerProps> = ({ onSelectImage }) => {
  // Refs
  const fileInputRef = useRef<HTMLInputElement>(null);

  // States
  const [files, setFiles] = useState<FileItem[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [selectedFolder, setSelectedFolder] = useState("admin");
  const [loading, setLoading] = useState(false);
  const [totalPages, setTotalPages] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);
  
  const [contextMenu, setContextMenu] = useState<ContextMenuState>({
    show: false,
    x: 0,
    y: 0,
    fileId: null,
  });
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [isFileManagerOpen, setIsFileManagerOpen] = useState(false);

  // Effects
  useEffect(() => {
    handleFetchFiles();
  }, [currentPage, pageSize, searchTerm]);

  // API Handlers
  const handleFetchFiles = async () => {
    try {
      setLoading(true);
      
      const params: FileFilterRequest = {
        currentPage: currentPage,
        pageSize: pageSize,
        sort: 'createdOnDate desc',
        searchAllApp: true
      };

      if (searchTerm) {
        params.fullTextSearch = searchTerm;
        params.fileName = searchTerm;
        params.listTextSearch = [searchTerm];
      }

      const response = await fileApi.getFiles(params);
      setFiles(response.data.content);
      setTotalPages(response.data.totalPages);
      setTotalRecords(response.data.totalRecords);
    } catch (error) {
      console.error("Failed to fetch files:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (!files?.length) return;

    try {
      setLoading(true);
      await fileApi.uploadFile(files[0]);
      await handleFetchFiles(); // Refresh list after upload
    } catch (error) {
      console.error("Failed to upload file:", error);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteFile = async (fileId: string) => {
    try {
      setLoading(true);
      await fileApi.deleteFile(fileId);
      await handleFetchFiles(); // Refresh list after delete
    } catch (error) {
      console.error("Failed to delete file:", error);
    } finally {
      setLoading(false);
    }
  };

  // Context Menu Handlers
  const handleContextMenu = (e: React.MouseEvent, fileId: string) => {
    e.preventDefault();
    setContextMenu({
      show: true,
      x: e.clientX,
      y: e.clientY,
      fileId,
    });
  };

  const closeContextMenu = () => {
    setContextMenu({
      show: false,
      x: 0,
      y: 0,
      fileId: null,
    });
  };
  

  // Context Menu Items Configuration
  const getContextMenuItems = (fileId: string) => [
    {
      icon: <Edit className="w-4 h-4" />,
      label: "Đổi tên",
      onClick: () => {
        // TODO: Implement rename dialog
      },
    },
    {
      divider: true,
    },
    {
      icon: <Download className="w-4 h-4" />,
      label: "Tải xuống",
      //TOdo
      // onClick: () => handleDownloadFile(fileId),
    },
    {
      icon: <Trash2 className="w-4 h-4" />,
      label: "Xóa",
      className: "text-destructive hover:text-destructive",
      onClick: () => handleDeleteFile(fileId),
    },
  ];

  // Filters
  const filteredFiles = files.filter(file => 
    file.fileName.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Render Methods
  const renderSidebar = () => (
    <div className="w-64 bg-background border-r p-4">
      <Input 
        placeholder="Tìm..." 
        className="mb-4"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      
      <div className="space-y-2">
        <div className="font-medium text-sm">Thư mục</div>
        <div 
          className={`flex items-center space-x-2 p-2 rounded cursor-pointer ${
            selectedFolder === "admin" ? "bg-accent" : "hover:bg-accent/50"
          }`}
          onClick={() => setSelectedFolder("admin")}
        >
          <FolderOpen size={20} />
          <span>Admin</span>
        </div>
      </div>
    </div>
  );

  const renderUploadSection = () => (
    <div className="mb-6">
      <Button
        onClick={() => fileInputRef.current?.click()}
        className="w-full h-32 border-2 border-dashed"
        disabled={loading}
      >
        <div className="flex flex-col items-center">
          <Upload size={24} className="mb-2" />
          <span>{loading ? "Đang tải lên..." : "Kéo thả file hoặc click để tải lên"}</span>
        </div>
      </Button>
      <Input
        type="file"
        ref={fileInputRef}
        onChange={handleFileUpload}
        className="hidden"
        accept="image/*"
      />
    </div>
  );

  const renderFileGrid = () => (
    <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
      {loading ? (
        <div>Loading...</div>
      ) : filteredFiles.length === 0 ? (
        <div>Không có file nào</div>
      ) : (
        filteredFiles.map((file) => (
          <div
            key={file.id}
            className="border rounded-lg overflow-hidden group"
            onContextMenu={(e) => handleContextMenu(e, file.id)}
            onClick={()=>{
              if (onSelectImage) {
                onSelectImage(file);
              }
            }
          }
          >
            <div className="aspect-square relative">
              <img
                src={file.completeFilePath}
                alt={file.fileName}
                className="w-full h-full object-cover"
              />
            </div>
            <div className="p-2 bg-background/95">
              <p className="text-sm truncate" title={file.fileName}>
                {file.fileName}
              </p>
              <p className="text-xs text-muted-foreground">
                {new Date(file.createdOnDate).toLocaleDateString()}
              </p>
            </div>
          </div>
        ))
      )}
    </div>
  );

  return (
    <div className="flex h-screen">
      {renderSidebar()}
      
      <div className="flex-1 p-6">
        <Card>
          <CardContent className="p-6 h-[calc(100vh-100px)] overflow-hidden">
            <div className="flex flex-col h-full">
              {renderUploadSection()}
              
              <div className="flex-1 overflow-y-auto pr-2">
                {renderFileGrid()}
              </div>

              {contextMenu.show && contextMenu.fileId && (
                <ContextMenu
                  items={getContextMenuItems(contextMenu.fileId).map(item => ({
                    ...item,
                    label: item.label || ''
                  }))}
                  x={contextMenu.x}
                  y={contextMenu.y}
                  onClose={closeContextMenu}
                />
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default FileManager; 