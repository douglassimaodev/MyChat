namespace MyChat.Models
{
    public class ChatMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
        public string RoomName { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage(string user, string message, string roomName)
        {
            User = user;
            Message = message;
            RoomName = roomName;
            Timestamp = DateTime.UtcNow;
        }
    }
}
