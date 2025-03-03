import { useState } from "react";
import { Modal, Button, Select, Card, Tabs } from "flowbite-react";
import { CirclePlus } from "lucide-react";

import { useRef } from "react";
import { HiAdjustments, HiClipboardList, HiUserCircle } from "react-icons/hi";
import { MdDashboard } from "react-icons/md";

const BaseAddProductModal = () => {
  const [openModal, setOpenModal] = useState(false);
  const tabsRef = useRef<TabsRef>(null);
  const [activeTab, setActiveTab] = useState(0);
  return (
    <div>
      <CirclePlus onClick={() => setOpenModal(true)} size={25} />

      <Modal show={openModal} size="2xl" onClose={() => setOpenModal(false)}>
        <Modal.Header>Thêm Sản Phẩm</Modal.Header>
        <Modal.Body>
          <div className="flex gap-4">
            <Card className="w-1/3 p-4 bg-gray-800 rounded-lg shadow-md border border-gray-700">
              <div className="flex flex-col items-center">
                <img
                  src="/placeholder-book.png"
                  alt="Book Cover"
                  className="w-20 h-28 mb-2 rounded-lg"
                />
                <img
                  src="/placeholder-qr.png"
                  alt="QR Code"
                  className="w-14 h-14 mt-2"
                />
                <Button className="mt-2 bg-blue-600 hover:bg-blue-500">
                  Lưu ảnh QR
                </Button>
              </div>

              <div className="mt-4 space-y-2">
                <input
                  className="bg-gray-700 p-2 rounded-lg w-full"
                  placeholder="MFN"
                  disabled
                />
                <input
                  className="bg-gray-700 p-2 rounded-lg w-full"
                  type="datetime-local"
                  placeholder="Ngày biên mục"
                />
                <Select>
                  <option>Sách</option>
                  <option>Báo</option>
                  <option>Tạp chí</option>
                </Select>
                <Select>
                  <option>Thư viện A</option>
                  <option>Thư viện B</option>
                </Select>
              </div>
              <div className="mt-4 space-y-2">
                <Card className="w-2/3 p-4 bg-gray-800 rounded-lg shadow-md border border-gray-700">
                  <div className="flex flex-col gap-3">
                    <Tabs
                      aria-label="Default tabs"
                      variant="default"
                      ref={tabsRef}
                      onActiveTabChange={(tab) => setActiveTab(tab)}
                    >
                      <Tabs.Item active title="Profile" icon={HiUserCircle}>
                        This is{" "}
                        <span className="font-medium text-gray-800 dark:text-white">
                          Profile tab's associated content
                        </span>
                        . Clicking another tab will toggle the visibility of
                        this one for the next. The tab JavaScript swaps classes
                        to control the content visibility and styling.
                      </Tabs.Item>
                      <Tabs.Item title="Dashboard" icon={MdDashboard}>
                        This is{" "}
                        <span className="font-medium text-gray-800 dark:text-white">
                          Dashboard tab's associated content
                        </span>
                        . Clicking another tab will toggle the visibility of
                        this one for the next. The tab JavaScript swaps classes
                        to control the content visibility and styling.
                      </Tabs.Item>
                      <Tabs.Item title="Settings" icon={HiAdjustments}>
                        This is{" "}
                        <span className="font-medium text-gray-800 dark:text-white">
                          Settings tab's associated content
                        </span>
                        . Clicking another tab will toggle the visibility of
                        this one for the next. The tab JavaScript swaps classes
                        to control the content visibility and styling.
                      </Tabs.Item>
                      <Tabs.Item title="Contacts" icon={HiClipboardList}>
                        This is{" "}
                        <span className="font-medium text-gray-800 dark:text-white">
                          Contacts tab's associated content
                        </span>
                        . Clicking another tab will toggle the visibility of
                        this one for the next. The tab JavaScript swaps classes
                        to control the content visibility and styling.
                      </Tabs.Item>
                      <Tabs.Item disabled title="Disabled">
                        Disabled content
                      </Tabs.Item>
                    </Tabs>
                    <div className="text-sm text-gray-500 dark:text-gray-400">
                      Active tab: {activeTab}
                    </div>
                    <Button.Group>
                      <Button
                        color="gray"
                        onClick={() => tabsRef.current?.setActiveTab(0)}
                      >
                        Profile
                      </Button>
                      <Button
                        color="gray"
                        onClick={() => tabsRef.current?.setActiveTab(1)}
                      >
                        Dashboard
                      </Button>
                      <Button
                        color="gray"
                        onClick={() => tabsRef.current?.setActiveTab(2)}
                      >
                        Settings
                      </Button>
                      <Button
                        color="gray"
                        onClick={() => tabsRef.current?.setActiveTab(3)}
                      >
                        Contacts
                      </Button>
                    </Button.Group>
                  </div>
                </Card>
              </div>
            </Card>
          </div>
        </Modal.Body>
        <Modal.Footer>
          <Button onClick={() => setOpenModal(false)}>I accept</Button>
          <Button color="gray" onClick={() => setOpenModal(false)}>
            Decline
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default BaseAddProductModal;
