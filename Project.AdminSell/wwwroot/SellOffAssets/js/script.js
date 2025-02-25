let cart = [];
let pendingOrders = [];

function openProductModal() {
    document.getElementById("productModal").style.display = "flex";
}

function closeProductModal() {
    document.getElementById("productModal").style.display = "none";
}

function openOrderModal() {
    if (pendingOrders.length >= 10) {
        alert("Không thể thêm quá 10 hóa đơn chờ!");
        return;
    }
    document.getElementById("orderModal").style.display = "flex";
}

function closeOrderModal() {
    document.getElementById("orderModal").style.display = "none";
}

function confirmAddProduct() {
    let name = document.getElementById("productName").value;
    let price = parseFloat(document.getElementById("productPrice").value);
    let quantity = parseInt(document.getElementById("productQuantity").value);

    if (!name || price <= 0 || quantity <= 0) {
        alert("Vui lòng nhập thông tin hợp lệ!");
        return;
    }

    cart.push({ name, price, quantity });
    updateCart();
    closeProductModal();
}

function updateCart() {
    let cartTable = document.querySelector("#cart tbody");
    cartTable.innerHTML = "";
    let total = 0;

    cart.forEach(item => {
        let row = cartTable.insertRow();
        row.insertCell(0).innerText = item.name;
        row.insertCell(1).innerText = item.price.toLocaleString() + "đ";
        row.insertCell(2).innerText = item.quantity;
        row.insertCell(3).innerText = (item.price * item.quantity).toLocaleString() + "đ";
        total += item.price * item.quantity;
    });

    document.getElementById("total").innerText = total.toLocaleString();
}

function confirmAddPendingOrder() {
    if (cart.length === 0) {
        alert("Giỏ hàng trống! Không thể thêm hóa đơn chờ.");
        return;
    }

    // Lưu giỏ hàng vào hóa đơn chờ
    let order = { items: [...cart] };
    pendingOrders.push(order);

    // Reset giỏ hàng
    cart = [];
    updateCart();
    updatePendingOrders();
    closeOrderModal();
}

function updatePendingOrders() {
    let ordersDropdown = document.getElementById("pendingOrderSelect");
    ordersDropdown.innerHTML = '<option value="">Chọn hóa đơn để thanh toán</option>';

    pendingOrders.forEach((order, index) => {
        let option = document.createElement("option");
        option.value = index;
        option.innerText = `Hóa đơn ${index + 1} - ${order.items.length} sản phẩm`;
        ordersDropdown.appendChild(option);
    });
}

function processPayment() {
    let selectedIndex = document.getElementById("pendingOrderSelect").value;

    if (selectedIndex === "") {
        alert("Vui lòng chọn hóa đơn để thanh toán!");
        return;
    }

    let name = document.getElementById("customerName").value.trim();
    let phone = document.getElementById("customerPhone").value.trim();
    let address = document.getElementById("customerAddress").value.trim();
    let paymentMethod = document.getElementById("paymentMethod").value;

    if (!name || !phone || !address) {
        alert("Vui lòng nhập đầy đủ thông tin khách hàng!");
        return;
    }

    let selectedOrder = pendingOrders[selectedIndex];

    if (!selectedOrder || selectedOrder.items.length === 0) {
        alert("Hóa đơn trống! Không thể thanh toán.");
        return;
    }

    alert(`Thanh toán thành công!\nKhách hàng: ${name}\nSố điện thoại: ${phone}\nĐịa chỉ: ${address}\nPhương thức: ${paymentMethod}\nSố sản phẩm: ${selectedOrder.items.length}`);

    // Xóa hóa đơn đã thanh toán
    pendingOrders.splice(selectedIndex, 1);
    updatePendingOrders();

    // Xóa thông tin khách hàng sau khi thanh toán
    document.getElementById("customerName").value = "";
    document.getElementById("customerPhone").value = "";
    document.getElementById("customerAddress").value = "";
    document.getElementById("voucher").value = "";
}