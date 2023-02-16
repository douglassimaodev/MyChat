const InitChat = (currentUser) => {
    let myUser = currentUser;

    var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    //Disable send button until connection is established
    $("#sendMessage").prop('disabled', true);

    connection.on("updateConnectedClients", function (connectedClients) {
        // Update the UI to show the list of connected clients
    });

    connection.on("ReceiveErrorMessage", function (message) {
    
        $("#message").val("");

        var containerMessage = `<div class="flex-row justify-content-start mb-4">
              <p class="small mb-0 fw-bold text-danger">${message}</p>
             </div>`;

        $("#messagesList").append(containerMessage);

        const myDiv = $("#messagesList");
        myDiv.animate({ scrollTop: myDiv.prop("scrollHeight") }, 'slow');
    })    

    connection.on("ReceiveMessage", function (user, message) {
       
        $("#message").val("");

        var containerMessage = `<div class="flex-row justify-content-start mb-4">
              ${user} says:
              <p class="small mb-0 fw-bold">${message}</p>
             </div>`;

        $("#messagesList").append(containerMessage);

        const myDiv = $("#messagesList");
        myDiv.animate({ scrollTop: myDiv.prop("scrollHeight") }, 'slow');

        console.log("aquiiii");
       
    });

    connection.on("joinGroup", function (message) {
        
        $("#message").val("");

        var containerMessage = `<div class="flex-row justify-content-start mb-4">              
              <p class="small mb-0 fw-bold">${message}</p>
             </div>`;

        $("#messagesList").append(containerMessage);

        const myDiv = $("#messagesList");
        myDiv.animate({ scrollTop: myDiv.prop("scrollHeight") }, 'slow');

    });

    connection.on("ReceiveLastMessages", function (lastMessages) {       
        var listItems = lastMessages.map(function (chatMessage) {
            return `<div class="flex-row justify-content-start mb-4">
              ${chatMessage.user} says:
              <p class="small mb-0 fw-bold">${chatMessage.message}</p>
             </div>`});
        $("#messagesList").append(listItems);

        const myDiv = $("#messagesList");
        myDiv.animate({ scrollTop: myDiv.prop("scrollHeight") }, 'slow');
    });

    connection.start().then(function () {
        $("#sendMessage").prop('disabled', false);
        var chatRoom = $("#chatRoom").val();

        connection.invoke("JoinRoom", chatRoom).catch(function (err) {
            console.error(err.toString());
        });

        connection.invoke("GetLastMessages", chatRoom).catch(function (err) {
            console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

    $("#sendMessage").click(function () {       
        var sender = $("#sender").val();
        var message = $("#message").val();
        var chatRoom = $("#chatRoom").val();

        //send to all
        connection.invoke("SendMessage", sender, message, chatRoom).catch(function (err) {
            return console.error(err.toString());
        });

        event.preventDefault();

    });
}