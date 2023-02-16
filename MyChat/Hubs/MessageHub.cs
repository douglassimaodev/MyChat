using Microsoft.AspNetCore.SignalR;
using MyChat.Models;
using MyChat.Services;

namespace MyChat.Hubs
{
    public class MessageHub : Hub
    {
       
        private static List<ChatMessage> messages = new List<ChatMessage>();

        private readonly IStockService _stockService;

        public MessageHub(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task SendMessage(string user, string message, string roomName)
        {
            if (message.StartsWith("/stock="))
            {
                string stockCode = message.Substring(7);

                if(string.IsNullOrEmpty(stockCode))
                {
                    await SendErrorMessage($"Command value is missing");
                }
                else
                {
                    var response = await _stockService.GetStock(stockCode);
                    if (response.Item1)
                    {
                        var chatMessage = new ChatMessage(user, response.Item2, roomName);
                        messages.Add(chatMessage);
                        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, chatMessage.Message);
                    }
                    else
                    {
                        await SendErrorMessage(response.Item2);
                    }
                }                
            }
            else
            {
                messages.Add(new ChatMessage(user, message, roomName));
                await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);                
            }
        }

        public async Task SendErrorMessage(string message)
        {
            var connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync("ReceiveErrorMessage", message);
        }

        public async Task GetLastMessages(string roomName)
        {
            var lastMessages = messages.Where(x=>x.RoomName == roomName)
            .OrderByDescending(m => m.Timestamp)
            .Take(50)
            .OrderBy(m => m.Timestamp);

            await Clients.Caller.SendAsync("ReceiveLastMessages", lastMessages);
            //await Clients.Client(roomName).SendAsync("ReceiveLastMessages", lastMessages);
        }

        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
