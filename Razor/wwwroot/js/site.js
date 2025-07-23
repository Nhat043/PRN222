
// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Connect to DataSignalRChanel
var dataConnection = new signalR.HubConnectionBuilder()
    .withUrl("/DataSignalRChanel")
    .build();

dataConnection.on("loadComment", function () {
    // handle event for DataSignalRChanel
    console.log("DataSignalRChanel load event");
    location.reload();
});

dataConnection.start()
    .then(() => console.log("Connected to DataSignalRChanel"))
    .catch(err => console.error("Error connecting to DataSignalRChanel:", err.toString()));

// Connect to AccountSignalRChanel
var accountConnection = new signalR.HubConnectionBuilder()
    .withUrl("/AccountSignalRChanel")
    .build();

accountConnection.on("load", function () {
    // handle event for AccountSignalRChanel
    location.href = '/mvc';
});

accountConnection.start()
    .then(() => console.log("Connected to AccountSignalRChanel"))
    .catch(err => console.error("Error connecting to AccountSignalRChanel:", err.toString()));
