
// Connect to DataSignalRChanel
var dataConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/DataSignalRChanel")
    .build();

dataConnection.on("load", function () {
    // handle event for DataSignalRChanel
    console.log("DataSignalRChanel load event");
});

dataConnection.start()
    .then(() => console.log("Connected to DataSignalRChanel"))
    .catch(err => console.error("Error connecting to DataSignalRChanel:", err.toString()));

// Connect to AccountSignalRChanel
var accountConnection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/AccountSignalRChanel")
    .build();

accountConnection.on("load", function () {
    // handle event for AccountSignalRChanel
    location.href = '/mvc';
});

accountConnection.start()
    .then(() => console.log("Connected to AccountSignalRChanel"))
    .catch(err => console.error("Error connecting to AccountSignalRChanel:", err.toString()));

