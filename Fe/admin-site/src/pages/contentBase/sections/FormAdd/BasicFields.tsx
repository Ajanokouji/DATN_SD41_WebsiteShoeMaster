import React from "react";
import { Control, useFormContext } from "react-hook-form";
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { ContentBaseFormSchema } from "./FormSchema";

import ReactQuill, { Quill } from "react-quill";
import "react-quill/dist/quill.snow.css";

import QuillResizeImage from 'quill-resize-image';
// Đăng ký module resize
Quill.register('modules/resize', QuillResizeImage);

interface BasicInfoFieldsProps {
  control: Control<ContentBaseFormSchema>;
}

export const BasicInfoFields: React.FC<BasicInfoFieldsProps> = ({ control }) => {
  const { trigger } = useFormContext();

  // Cấu hình Toolbar và tính năng cho React Quill
  const modules = {
    toolbar: [
      [{ font: ["sans-serif", "serif", "monospace"] }],
      [{ size: ["small", false, "large", "huge"] }], // cỡ chữ
      [{ header: [1, 2, 3, 4, 5, 6, false] }], // tiêu đề

      ["bold", "italic", "underline", "strike"], // định dạng
      [{ color: [] }, { background: [] }], // màu

      [{ script: "sub" }, { script: "super" }],

      [{ align: [] }], // căn lề
      [{ list: "ordered" }, { list: "bullet" }],
      [{ indent: "-1" }, { indent: "+1" }],

      ["link", "image", "video"],

      ["clean"], // nút xóa định dạng
    ],
    resize:{},
  };

  const formats = [
    "header",
    "font",
    "size",
    "bold",
    "italic",
    "underline",
    "strike",
    "color",
    "background",
    "script",
    "list",
    "bullet",
    "indent",
    "align",
    "link",
    "image",
    "video",
  ];

  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="title"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tiêu đề</FormLabel>
              <FormControl>
                <Input
                  placeholder="Nhập tiêu đề"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("title");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="seoUri"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Đường dẫn SEO</FormLabel>
              <FormControl>
                <Input
                  placeholder="Nhập đường dẫn SEO"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("seoUri");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="seoTitle"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tiêu đề SEO</FormLabel>
              <FormControl>
                <Input
                  placeholder="Nhập tiêu đề SEO"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("seoTitle");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="seoKeywords"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Từ khóa SEO</FormLabel>
              <FormControl>
                <Input
                  placeholder="Nhập từ khóa SEO"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("seoKeywords");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <FormField
        control={control}
        name="seoDescription"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Mô tả SEO</FormLabel>
            <FormControl>
              <Textarea
                placeholder="Nhập mô tả SEO"
                {...field}
                onChange={(e) => {
                  field.onChange(e);
                  trigger("seoDescription");
                }}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <div className="grid grid-cols-2 gap-4">
        <FormField
          control={control}
          name="publishStartDate"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Ngày bắt đầu xuất bản</FormLabel>
              <FormControl>
                <Input
                  type="datetime-local"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("publishStartDate");
                    trigger("publishEndDate");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={control}
          name="publishEndDate"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Ngày kết thúc xuất bản</FormLabel>
              <FormControl>
                <Input
                  type="datetime-local"
                  {...field}
                  onChange={(e) => {
                    field.onChange(e);
                    trigger("publishEndDate");
                    trigger("publishStartDate");
                  }}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />
      </div>

      <FormField
        control={control}
        name="isPublish"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Trạng thái xuất bản</FormLabel>
            <FormControl>
              <Input
                type="checkbox"
                checked={field.value}
                onChange={(e) => {
                  field.onChange(e.target.checked);
                  trigger("isPublish");
                }}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />

      <FormField
        control={control}
        name="content"
        render={({ field }) => (
          <FormItem>
            <FormLabel>Nội dung bài viết</FormLabel>
            <FormControl>
              <ReactQuill
                theme="snow"
                {...field}
                onChange={(e) => {
                  field.onChange(e);
                  trigger("content");
                }}
                placeholder="Viết nội dung ở đây..."
                modules={modules}
                formats={formats}
              />
            </FormControl>
            <FormMessage />
          </FormItem>
        )}
      />
    </div>
  );
};