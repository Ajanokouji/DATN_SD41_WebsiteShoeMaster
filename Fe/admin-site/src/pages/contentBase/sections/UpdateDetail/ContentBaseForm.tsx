import React from "react";
import { FormProvider, UseFormReturn } from "react-hook-form";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { ContentBaseFormSchema } from "./FormSchema";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";

import "@/pages/contentBase/modules/CustomImageBlot";
import "@/pages/contentBase/modules/CustomVideoBlot";

import ReactQuill, { Quill } from "react-quill";
import "react-quill/dist/quill.snow.css";

import QuillResizeImage from 'quill-resize-image';
// Đăng ký module resize
Quill.register('modules/resize', QuillResizeImage);

interface ContentBaseFormProps {
  methods: UseFormReturn<ContentBaseFormSchema>;
  isEditing: boolean;
  onSubmit: (values: ContentBaseFormSchema) => void;
  children?: React.ReactNode;
}

export const ContentBaseForm: React.FC<ContentBaseFormProps> = ({
  methods,
  isEditing,
  onSubmit,
  children,
}) => {
  const { control } = methods;

  // Cấu hình Toolbar và tính năng cho React Quill
  const modules = {
    toolbar: [
      [{ 'font': ['sans-serif', 'serif', 'monospace'] }],
      [{ 'size': ['small', false, 'large', 'huge'] }],// cỡ chữ
      [{ 'header': [1, 2, 3, 4, 5, 6, false] }], // tiêu đề

      ['bold', 'italic', 'underline', 'strike'], // định dạng
      [{ 'color': [] }, { 'background': [] }], // màu

      [{ 'script': 'sub'}, { 'script': 'super' }],

      [{ 'align': [] }], // căn lề
      [{ 'list': 'ordered'}, { 'list': 'bullet' }],
      [{ 'indent': '-1'}, { 'indent': '+1' }],

      ['link', 'image', 'video'],

      ['clean'] // nút xóa định dạng
    ],
    resize: {},
  };

  const formats = [
    'header', 'font', 'size',
    'bold', 'italic', 'underline', 'strike',
    'color', 'background',
    'script',
    'list', 'bullet', 'indent',
    'align',
    'link', 'image', 'video'
  ];

  return (
    <FormProvider {...methods}>
      <Form {...methods}>
        <form
          onSubmit={methods.handleSubmit(onSubmit)}
          className="space-y-4 mt-6"
        >
          <div className="grid grid-cols-2 gap-4">
            <FormField
              control={control}
              name="title"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tiêu đề</FormLabel>
                  <FormControl>
                    <Input
                      {...field}
                      placeholder="Nhập tiêu đề"
                      disabled={!isEditing}
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
                      {...field}
                      placeholder="Nhập đường dẫn SEO"
                      disabled={!isEditing}
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
                    {...field}
                    placeholder="Nhập mô tả SEO"
                    disabled={!isEditing}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

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
          onChange={(e) => field.onChange(e.target.checked)}
          disabled={!isEditing}
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
                <div className="relative">
                  <ReactQuill
                    theme="snow"
                    {...field}
                    placeholder="Viết nội dung ở đây..."
                    modules={modules}
                    formats={formats}
                  />
                  {!isEditing && (
                    <div
                      className="absolute inset-0 z-10"
                      style={{ pointerEvents: "all", backgroundColor: "transparent" }}
                    />
                  )}
                </div>
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        
          {children}
        </form>
      </Form>
    </FormProvider>
  );
};