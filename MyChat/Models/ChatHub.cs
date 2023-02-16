//using Microsoft.AspNetCore.SignalR;
//using MyChat.Models;

//namespace ChatApp.Models
//{
//    public class ChatHubb : Hub
//    {
//        private static List<ChatMessage> messages = new List<ChatMessage>();

//        public async Task SendMessage(string user, string message)
//        {
//            messages.Add(new ChatMessage(user, message));
//            await Clients.All.SendAsync("ReceiveMessage", user, message);
//        }


//        //public async Task SendMessageQueeu(string user, string message)
//        //{
//        //    if (message.StartsWith("/stock="))
//        //    {
//        //        string stockCode = message.Substring(7);

//        //        StockQuote stockQuote = await _mediator.Send(new GetStockQuoteQuery(stockCode));

//        //        var chatMessage = new ChatMessage(user, $"{stockQuote.Symbol} quote is {stockQuote.Price:C2} per share");
//        //        messages.Add(chatMessage);
//        //        await Clients.All.SendAsync("ReceiveMessage", user, message);
//        //    }
//        //    else
//        //    {
//        //        messages.Add(new ChatMessage(user, message));
//        //        await Clients.All.SendAsync("ReceiveMessage", user, message);

//        //        //await _mediator.Send(new AddChatMessageCommand(request.ChatMessage));
//        //    }
//        //}

//        public async Task GetLastMessages()
//        {
//            var lastMessages = messages
//            .OrderByDescending(m => m.Timestamp)
//            .Take(50)
//            .OrderBy(m => m.Timestamp);

//            await Clients.Caller.SendAsync("ReceiveLastMessages", lastMessages);
//        }
//    }



//    //public class ChatHub : Hub
//    //{
//    //    private readonly IMediator _mediator;

//    //    public ChatHub(IMediator mediator)
//    //    {
//    //        _mediator = mediator;
//    //    }

//    //    public async Task SendMessage(ChatMessage chatMessage)
//    //    {

//    //        if (chatMessage.Text.StartsWith("/stock="))
//    //        {
//    //            string stockCode = chatMessage.Text.Substring(7);

//    //            StockQuote stockQuote = await _mediator.Send(new GetStockQuoteQuery(stockCode));

//    //            var botMessage = new ChatMessage
//    //            {
//    //                UserName = "Bot",
//    //                Text = $"{stockQuote.Symbol} quote is {stockQuote.Price:C2} per share",
//    //                Timestamp = DateTime.UtcNow
//    //            };

//    //            await _mediator.Send(new SendMessageCommand(botMessage));
//    //        }
//    //        else
//    //        {
//    //            chatMessage.Timestamp = DateTime.UtcNow;
//    //            await _mediator.Send(new AddChatMessageCommand(request.ChatMessage));
//    //        }

//    //        //await _mediator.Send(new SendMessageCommand(chatMessage));
//    //    }

//    //    public async Task<IEnumerable<ChatMessage>> GetLastMessages()
//    //    {
//    //        return await _mediator.Send(new GetLastMessagesQuery());
//    //    }
//    //}

//    //public class GetStockQuoteQueryHandler : IRequestHandler<GetStockQuoteQuery, StockQuote>
//    //{
//    //    private readonly IHttpClientFactory _httpClientFactory;

//    //    public GetStockQuoteQueryHandler(IHttpClientFactory httpClientFactory)
//    //    {
//    //        _httpClientFactory = httpClientFactory;
//    //    }

//    //    public async Task<StockQuote> Handle(GetStockQuoteQuery request, CancellationToken cancellationToken)
//    //    {
//    //        var url = $"https://stooq.com/q/l/?s={request.StockCode}&f=sd2t2ohlcv&h&e=csv";
//    //        var client = _httpClientFactory.CreateClient();

//    //        var response = await client.GetAsync(url);

//    //        if (response.IsSuccessStatusCode)
//    //        {
//    //            var content = await response.Content.ReadAsStringAsync();
//    //            var lines = content.Split('\n');

//    //            if (lines.Length > 1)
//    //            {
//    //                var values = lines[1].Split(',');

//    //                if (values.Length == 7 && decimal.TryParse(values[6], out decimal price))
//    //                {
//    //                    var stockQuote = new StockQuote
//    //                    {
//    //                        Symbol = request.StockCode.ToUpper(),
//    //                        Price = price
//    //                    };

//    //                    return stockQuote;
//    //                }
//    //            }
//    //        }

//    //        throw new Exception($"Failed to get stock quote for {request.StockCode}");
//    //    }
//    //}


//    //public class GetStockQuoteQuery : IRequest<StockQuote>
//    //{
//    //    public string StockCode { get; }

//    //    public GetStockQuoteQuery(string stockCode)
//    //    {
//    //        StockCode = stockCode;
//    //    }
//    //}

//    //public class AddChatMessageCommand : IRequest
//    //{
//    //    public ChatMessage ChatMessage { get; }

//    //    public AddChatMessageCommand(ChatMessage chatMessage)
//    //    {
//    //        ChatMessage = chatMessage;
//    //    }
//    //}

//    //public class AddChatMessageCommandHandler : AsyncRequestHandler<AddChatMessageCommand>
//    //{
//    //    private readonly IList<ChatMessage> _chatMessages;

//    //    public AddChatMessageCommandHandler(IList<ChatMessage> chatMessages)
//    //    {
//    //        _chatMessages = chatMessages;
//    //    }

//    //    protected override Task Handle(AddChatMessageCommand request, CancellationToken cancellationToken)
//    //    {
//    //        _chatMessages.Add(request.ChatMessage);

//    //        return Task.CompletedTask;
//    //    }
//    //}


//    //public class GetLastMessagesQuery : IRequest<IEnumerable<ChatMessage>>
//    //{
//    //}

//    //public class GetLastMessagesQueryHandler : IRequestHandler<GetLastMessagesQuery, IEnumerable<ChatMessage>>
//    //{
//    //    private readonly IList<ChatMessage> _chatMessages;

//    //    public GetLastMessagesQueryHandler(IList<ChatMessage> chatMessages)
//    //    {
//    //        _chatMessages = chatMessages;
//    //    }

//    //    public Task<IEnumerable<ChatMessage>> Handle(GetLastMessagesQuery request, CancellationToken cancellationToken)
//    //    {
//    //        IEnumerable<ChatMessage> lastMessages = _chatMessages
//    //            .OrderByDescending(m => m.Timestamp)
//    //            .Take(50)
//    //            .OrderBy(m => m.Timestamp);

//    //        return Task.FromResult(lastMessages);
//    //    }
//    //}

//    // Define the SignalR hub for handling chatroom messages
//    /*public class ChatHub : Hub
//    {
//        private readonly IMessageService _messageService;
//        private readonly ICommandParser _commandParser;
//        private readonly IBotService _botService;

//        public ChatHub(IMessageService messageService, ICommandParser commandParser, IBotService botService)
//        {
//            _messageService = messageService;
//            _commandParser = commandParser;
//            _botService = botService;
//        }

//        public async Task SendMessagee(string message)
//        {
//            var user = Context.User.Identity.Name;
//            if (_commandParser.IsCommand(message))
//            {
//                // If the message is a command, handle it with the bot service
//                var result = await _botService.HandleCommand(user, message);
//                await Clients.Caller.SendAsync("ReceiveMessage", result);
//            }
//            else
//            {
//                // Otherwise, add the message to the chat history and broadcast it to all clients
//                var chatMessage = new ChatMessage(user, message);
//                _messageService.AddMessage(chatMessage);
//                await Clients.All.SendAsync("ReceiveMessage", chatMessage);
//            }
//        }

//        public async Task GetChatHistory()
//        {
//            // Retrieve the last 50 chat messages from the message service and send them to the caller
//            var chatHistory = _messageService.GetLast50Messages();
//            await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
//        }
//    }

//    // Define the command parser interface
//    public interface ICommandParser
//    {
//        bool IsCommand(string message);
//        string GetCommand(string message);
//        string GetArgument(string message);
//    }

//    // Define the bot service interface
//    public interface IBotService
//    {
//        Task<string> HandleCommand(string user, string message);
//    }

//    // Define the message service interface for storing and retrieving chat messages
//    public interface IMessageService
//    {
//        void AddMessage(ChatMessage message);
//        List<ChatMessage> GetLast50Messages();
//    }

//    // Define the chat message model
//    public class ChatMessage
//    {
//        public string User { get; set; }
//        public string Message { get; set; }
//        public DateTime Timestamp { get; set; }

//        public ChatMessage(string user, string message)
//        {
//            User = user;
//            Message = message;
//            Timestamp = DateTime.UtcNow;
//        }
//    }

//    // Implement the command parser to identify and handle commands
//    public class CommandParser : ICommandParser
//    {
//        public bool IsCommand(string message)
//        {
//            return message.StartsWith("/stock=");
//        }

//        public string GetCommand(string message)
//        {
//            return "/stock";
//        }

//        public string GetArgument(string message)
//        {
//            return message.Substring("/stock=".Length);
//        }
//    }

//    // Implement the bot service to handle stock commands
//    public class BotService : IBotService
//    {
//        private readonly IStockService _stockService;
//        private readonly IMessagePublisher _messagePublisher;

//        public BotService(IStockService stockService, IMessagePublisher messagePublisher)
//        {
//            _stockService = stockService;
//            _messagePublisher = messagePublisher;
//        }

//        public async Task<string> HandleCommand(string user, string message)
//        {
//            var stockCode = new CommandParser().GetArgument(message);
//            var stockQuote = await _stockService.GetStockQuote(stockCode);
//            var result = $"{stockCode} quote is ${stockQuote} per share";
//            var botMessage = new ChatMessage("Bot", result);
//            _messagePublisher.Publish(botMessage);
//            return result;
//        }
//    }

//    // Implement the stock service to call the stock API and return the stock quote
//    public class StockService : IStockService
//    {
//        private readonly HttpClient _httpClient;

//        public StockService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<decimal> GetStockQuote(string stockCode)
//        {
//            var response = await _httpClient.GetAsync($"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv");
//            if (response.IsSuccessStatusCode)
//            {
//                var content = await response.Content.ReadAsStringAsync();
//                var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
//                if (lines.Length > 1)
//                {
//                    var fields = lines[1].Split(',');
//                    if (fields.Length > 6 && decimal.TryParse(fields[6], out var stockQuote))
//                    {
//                        return stockQuote;
//                    }
//                }
//            }

//            throw new Exception($"Failed to retrieve stock quote for {stockCode}");
//        }
//    }

//    // Implement the message service using an in-memory list for simplicity
//    public class InMemoryMessageService : IMessageService
//    {
//        private readonly List<ChatMessage> _chatHistory = new List<ChatMessage>();

//        public void AddMessage(ChatMessage message)
//        {
//            _chatHistory.Add(message);
//        }

//        public List<ChatMessage> GetLast50Messages()
//        {
//            return _chatHistory.OrderByDescending(m => m.Timestamp).Take(50).ToList();
//        }
//    }

//    // Implement the message publisher using RabbitMQ
//    public class RabbitMQMessagePublisher : IMessagePublisher
//    {
//        private readonly ConnectionFactory _factory;
//        private readonly IConnection _connection;
//        private readonly IModel _channel;

//        public RabbitMQMessagePublisher(string hostname)
//        {
//            _factory = new ConnectionFactory() { HostName = hostname };
//            _connection = _factory.CreateConnection();
//            _channel = _connection.CreateModel();
//            _channel.QueueDeclare(queue: "stock-quotes", durable: false, exclusive: false, autoDelete: false, arguments: null);
//        }

//        public void Publish(ChatMessage message)
//        {
//            var body = Encoding.UTF8.GetBytes(message.Message);
//            _channel.BasicPublish(exchange: "", routingKey: "stock-quotes", basicProperties: null, body: body);
//        }
//    }

//    // Define the stock service interface
//    public interface IStockService
//    {
//        Task<decimal> GetStockQuote(string stockCode);
//    }

//    // Define the message publisher interface for sending bot messages to the chatroom
//    public interface IMessagePublisher
//    {
//        void Publish(ChatMessage message);
//    }
//    */
//}