
<script src="~/js/signalr.js"/>
let connection = new signalR.HubConnectionBuilder().withUrl("/notifications").build();

connection.on('sendToUser', data => {
    alert(data)
});

connection.start().then(() => {
    console.log("connection started");
    connection.invoke('GetConnectionId').then(function (connectionId) {
        alert(connectionId);
    });
});

        /*function sendMessage() {
        var msg = document.getElementById("txtMessage").value;
        connection.invoke('send', msg);
    }*/
   