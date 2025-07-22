var connection = new signalR.HubConnectionBuilder()
    .withUrl("/DataSignalRChanel")
    .build();

//setup lắng nghe message “load” thì thực hiện
connection.on("load", function () {
    location.href = '/VariationOptionPage';
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
