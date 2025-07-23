// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/CommentSignalRChanel")
    .build();

//setup lắng nghe message “load” thì thực hiện
connection.on("load", function () {
    //location.reload(); // Tải lại trang để cập nhật dữ liệu
    location.href = '/CommentPage'
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});