// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/CommentSignalRChanel") // Host Hub
    .build();




connection.on("load", function () {
    var productId = document.querySelector("input[name='productId']")?.value;
    if (productId) {
        location.href = '/mvc/Product/Detail/' + productId;
    } else {
        location.reload(); // fallback nếu không tìm thấy id
    }
});

connection.start()
    .then(() => console.log("SignalR connected from MVC"))
    .catch(err => console.error("Lỗi kết nối SignalR:", err.toString()));
