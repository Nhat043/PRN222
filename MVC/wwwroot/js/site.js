// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:44319/DataSignalRChanel") // Host Hub
    .build();




connection.on("load", function () {
    location.href = '/Product/Detail'; // hoặc location.reload();
});

connection.start()
    .then(() => console.log("SignalR connected from MVC"))
    .catch(err => console.error("Lỗi kết nối SignalR:", err.toString()));
