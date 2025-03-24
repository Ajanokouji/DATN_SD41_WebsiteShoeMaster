// Đóng popup
function closePopup() {
    document.getElementById("product-popup").style.display = "none";
}

// Mở popup
function openPopup() {
    document.getElementById("product-popup").style.display = "flex";
}

// Tìm kiếm sản phẩm
document.getElementById("search-product").addEventListener("input", (e) => {
    renderProductTable(e.target.value);
});

// Hiển thị popup thông báo
function showNotification(message) {
    document.getElementById("notification-message").textContent = message;
    document.getElementById("notification-popup").style.display = "flex";
}

// Đóng popup thông báo
function closeNotification() {
    document.getElementById("notification-popup").style.display = "none";
}

// Tải danh sách sản phẩm từ API
let productList = [];

async function fetchProducts() {
    console.log("fetchProducts() đã chạy");
    try {
        const response = await fetch("https://localhost:7095/api/SanPham/getAll");
        productList = await response.json();
        // console.log("Dữ liệu sản phẩm:", productList);
        renderProductTable();
    } catch (error) {
        console.error("Lỗi khi lấy sản phẩm:", error);
    }
}

// Hiển thị danh sách sản phẩm trong popup
function renderProductTable() {
    const tbody = document.querySelector("#product-table tbody");
    if (!tbody) {
        console.error("Không tìm thấy tbody trong bảng sản phẩm!");
        return;
    }

    // Xóa nội dung cũ
    tbody.innerHTML = "";

    // Kiểm tra nếu productList có dữ liệu
    if (productList.length === 0) {
        tbody.innerHTML = "<tr><td colspan='4'>Không có sản phẩm nào!</td></tr>";
        return;
    }

    // Duyệt qua từng sản phẩm và tạo hàng trong bảng
    productList.forEach(p => {
        const row = document.createElement("tr");

        // Cột ảnh sản phẩm
        const imgCell = document.createElement("td");
        const img = document.createElement("img");
        // img.src = `https://localhost:7095/images/${p.image}`;
        img.alt = "#Image";
        img.alt = "Sản phẩm";
        img.width = 50;
        img.onerror = () => { img.src = "default.jpg"; }; // Nếu ảnh lỗi, thay bằng ảnh mặc định
        imgCell.appendChild(img);

        // Cột tên sản phẩm
        const nameCell = document.createElement("td");
        nameCell.textContent = p.ten;

        // Cột số lượng
        const quantityCell = document.createElement("td");
        quantityCell.textContent = p.soLuong;

        // Cột nút thêm sản phẩm
        const addCell = document.createElement("td");
        const addButton = document.createElement("button");
        addButton.textContent = "+";
        addButton.onclick = () => addProductToInvoice(p.image, p.ten, p.soLuong);
        addCell.appendChild(addButton);

        // Thêm các ô vào hàng
        row.appendChild(imgCell);
        row.appendChild(nameCell);
        row.appendChild(quantityCell);
        row.appendChild(addCell);

        // Thêm hàng vào tbody
        tbody.appendChild(row);
    });
}

document.addEventListener("DOMContentLoaded", () => {
    fetchProducts();
    const invoiceTabs = document.getElementById("invoice-tabs");
    const invoiceContent = document.getElementById("invoice-content");
    const addInvoiceBtn = document.getElementById("add-invoice");
    let invoices = [];
    let selectedInvoice = null;

    // Thêm hóa đơn mới
    addInvoiceBtn.addEventListener("click", () => {
        if (invoices.length >= 5) {
            showNotification("Không thể tạo quá 5 hóa đơn!");
            return;
        }
    
        const invoiceId = Date.now();
        invoices.push({ id: invoiceId, products: [] });
        selectedInvoice = invoiceId;
        renderTabs();
        renderInvoice();
    });

    // Render danh sách hóa đơn dưới dạng tab
    function renderTabs() {
        invoiceTabs.innerHTML = "";
        invoices.forEach((invoice) => {
            const tab = document.createElement("button");
            tab.textContent = `Hóa đơn ${invoice.id}`;
            tab.classList.toggle("active", invoice.id === selectedInvoice);
            tab.onclick = () => {
                selectedInvoice = invoice.id;
                renderTabs();
                renderInvoice();
            };

            // Nút xóa hóa đơn
            const closeBtn = document.createElement("span");
            closeBtn.textContent = " x";
            closeBtn.classList.add("close-btn");
            closeBtn.onclick = (e) => {
                e.stopPropagation();
                invoices = invoices.filter((inv) => inv.id !== invoice.id);
                selectedInvoice = invoices.length ? invoices[0].id : null;
                renderTabs();
                renderInvoice();
            };

            tab.appendChild(closeBtn);
            invoiceTabs.appendChild(tab);
        });
    }

    // Hiển thị nội dung hóa đơn
    function renderInvoice() {
        invoiceContent.innerHTML = "";
        const invoice = invoices.find((inv) => inv.id === selectedInvoice);
        if (!invoice) {
            invoiceContent.classList.remove("active");
            return;
        }
        invoiceContent.classList.add("active");

        const title = document.createElement("h2");
        title.textContent = `Giỏ hàng - Hóa đơn ${invoice.id}`;
        invoiceContent.appendChild(title);

        // Nút thêm sản phẩm
        const addProductBtn = document.createElement("button");
        addProductBtn.textContent = "+ Thêm sản phẩm";
        addProductBtn.classList.add("add-product");
        addProductBtn.onclick = openPopup;
        invoiceContent.appendChild(addProductBtn);

        // Bảng sản phẩm
        const table = document.createElement("table");
        table.classList.add("product-table");
        table.innerHTML = `
        <thead>
            <tr>
                <th>Tên</th>
                <th>Giá</th>
                <th>Số lượng</th>
                <th>Xóa</th>
            </tr>
        </thead>
        <tbody>
            ${invoice.products.map((p, index) => `
                <tr>
                    <td>${p.name}</td>
                    <td>${p.price}</td>
                    <td>
                        <input type="number" value="${p.quantity}" min="1" onchange="updateQuantity(${invoice.id}, ${index}, this.value)">
                    </td>
                    <td><button onclick="removeProduct(${invoice.id}, ${index})">X</button></td>
                </tr>
            `).join("")}
        </tbody>
    `;
        invoiceContent.appendChild(table);
    }
});

// Cập nhật số lượng sản phẩm
function updateQuantity(invoiceId, productIndex, quantity) {
    const invoice = invoices.find(inv => inv.id === invoiceId);
    if (invoice) {
        invoice.products[productIndex].quantity = parseInt(quantity, 10);
        renderInvoice();
    }
}

// Xóa sản phẩm
function removeProduct(invoiceId, productIndex) {
    const invoice = invoices.find(inv => inv.id === invoiceId);
    if (invoice) {
        invoice.products.splice(productIndex, 1);
        renderInvoice();
    }
}