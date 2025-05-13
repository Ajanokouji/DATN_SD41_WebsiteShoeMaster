import React, { useState, useRef, useEffect } from "react";
import ReactQuill, { Quill } from "react-quill";
import "react-quill/dist/quill.snow.css";

import QuillResizeImage from 'quill-resize-image';
// Đăng ký module resize
Quill.register('modules/resize', QuillResizeImage);

const Dashboard: React.FC = () => {
  const [editorContent, setEditorContent] = useState("");
  const [savedContent, setSavedContent] = useState("");
  const shadowRef = useRef<HTMLDivElement>(null);

  const handleSave = () => {
    setSavedContent(editorContent);
    console.log("Nội dung đã lưu (HTML):", editorContent);
  };

  useEffect(() => {
    if (shadowRef.current) {
      // Tạo shadow root nếu chưa có
      if (!shadowRef.current.shadowRoot) {
        const shadow = shadowRef.current.attachShadow({ mode: "open" });
        shadow.innerHTML = `<div id="shadow-content"></div>`;
      }

      // Gán nội dung HTML vào trong Shadow DOM
      const shadowContent = shadowRef.current.shadowRoot?.getElementById("shadow-content");
      if (shadowContent) {
        shadowContent.innerHTML = savedContent;
      }
    }
  }, [savedContent]);

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
    resize:{}
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
    <div style={{ padding: "30px", margin: "auto" }}>
      <h2>Trình soạn thảo văn bản (React Quill)</h2>
      <ReactQuill
        theme="snow"
        value={editorContent}
        onChange={setEditorContent}
        placeholder="Viết nội dung ở đây..."
        modules={modules}
        formats={formats}
      />

      <button
        style={{
          marginTop: "20px",
          padding: "10px 20px",
          backgroundColor: "#007bff",
          color: "#fff",
          border: "none",
          cursor: "pointer",
        }}
        onClick={handleSave}
      >
        Lưu nội dung
      </button>

      <hr style={{ margin: "40px 0" }} />

      <h1>Value lưu vào DB:</h1>
      <div className="border rounded" style={{maxWidth:'1000px', overflow:"auto"}}>{savedContent}</div>

      <br />
      <h1>Value hiển thị:</h1>
      <div className="border rounded" ref={shadowRef}></div>
    </div>
  );
};

export default Dashboard;