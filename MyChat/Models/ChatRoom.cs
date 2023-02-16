namespace MyChat.Models
{
    public class ChatRoom
    {
        public ChatRoom(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
