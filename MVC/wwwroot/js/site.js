var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/DataSignalRChanel") // Host Hub
    .build();

connection.on("load", function () {
    location.reload(); // hoặc location.reload();
});

connection.start()
    .then(() => console.log("SignalR connected from MVC"))
    .catch(err => console.error("Lỗi kết nối SignalR:", err.toString()));
