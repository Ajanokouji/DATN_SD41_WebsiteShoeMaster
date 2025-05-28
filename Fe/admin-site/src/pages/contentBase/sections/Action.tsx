import { Button } from "@/components/ui/button";
import { FaPlus } from "react-icons/fa6";
import AddContentBaseSheet from "./FormAdd/AddContentBaseSheet";
import { useState, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";

const ActionHeader = () => {
  const [isOpenAdd, setIsOpenAdd] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();

  const params = new URLSearchParams(location.search);

  const [title, setTitle] = useState(params.get("title") || "");
  const [seoUri, setSeoUri] = useState(params.get("seoUri") || "");

  const handleSearch = () => {
    const newParams = new URLSearchParams();

    if (title) newParams.set("title", title);
    if (seoUri) newParams.set("seoUri", seoUri);

    navigate({ pathname: location.pathname, search: newParams.toString() });
  };

  useEffect(() => {
    setTitle(params.get("title") || "");
    setSeoUri(params.get("seoUri") || "");
  }, [location.search]);

  return (
    <section>
      <h2 className="text-2xl mb-6">Tạo Content Base</h2>

      <div className="flex flex-wrap gap-4 mb-4">
        <input
          type="text"
          placeholder="Tiêu đề"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="border p-2 rounded w-48"
        />
        <input
          type="text"
          placeholder="SEO URI"
          value={seoUri}
          onChange={(e) => setSeoUri(e.target.value)}
          className="border p-2 rounded w-48"
        />
        <Button onClick={handleSearch}>Tìm kiếm</Button>
      </div>

      <Button onClick={() => setIsOpenAdd(true)} className="mt-4 px-4 py-2">
        <FaPlus />
        Tạo
      </Button>

      {isOpenAdd && (
        <AddContentBaseSheet
          isOpen={isOpenAdd}
          onClose={() => setIsOpenAdd(false)}
        />
      )}
    </section>
  );
};

export default ActionHeader;