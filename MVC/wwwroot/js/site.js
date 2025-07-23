// Connect to DataSignalRChanel
var dataConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/DataSignalRChanel")
    .build();

dataConnection.on("load", function () {
    // handle event for DataSignalRChanel
    var productId = document.querySelector("input[name='productId']")?.value;
    if (productId) {
        location.href = '/Product/Detail/' + productId;
    } else {
        location.reload(); // fallback nếu không tìm thấy id
    }
});

dataConnection.start()
    .then(() => console.log("Connected to DataSignalRChanel"))
    .catch(err => console.error("Error connecting to DataSignalRChanel:", err.toString()));

// Connect to AccountSignalRChanel
var accountConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/AccountSignalRChanel")
    .build();

accountConnection.on("loadBanAccount", function () {
    // handle event for AccountSignalRChanel
    location.href = '/mvc';
});

accountConnection.start()
    .then(() => console.log("Connected to AccountSignalRChanel"))
    .catch(err => console.error("Error connecting to AccountSignalRChanel:", err.toString()));

// Connect to VarianSignalR
var varianConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/VarianSignalR")
    .build();

varianConnection.on("load", function () {
    location.href = '/mvc/Product/Index'; // hoặc location.reload();
});

varianConnection.start()
    .then(() => console.log("SignalR connected from MVC (VarianSignalR)"))
    .catch(err => console.error("Lỗi kết nối SignalR (VarianSignalR):", err.toString()));
