var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7082/DataSignalRChanel") // Host Hub
    .build();