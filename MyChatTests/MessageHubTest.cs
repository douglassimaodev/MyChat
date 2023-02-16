using Microsoft.AspNetCore.SignalR;
using Moq;
using MyChat.Hubs;
using MyChat.Services;

namespace MyChatTests
{
    public class MessageHubTest
    {
        [Fact]
        public async Task JoinRoom_Should_Join_To_A_ChatRoom()
        {    
            var mockGroupManager = new Mock<IGroupManager>();
            mockGroupManager.Setup(m=>m.AddToGroupAsync(It.IsAny<string>(), "testGroup", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockContext = new Mock<HubCallerContext>();
            mockContext.SetupGet(m => m.ConnectionId).Returns("UserGuid");
           
            var simpleHub = new MessageHub(It.IsAny<IStockService>())
            {
                Groups = mockGroupManager.Object,
                Context = mockContext.Object
            };

            await simpleHub.JoinRoom("testGroup");
            mockGroupManager.Verify(m => m.AddToGroupAsync("UserGuid", "testGroup", default), Times.Once);

        }

        [Fact]
        public async Task SendMessage_Should_Send_Message_To_All_In_ChatRoom()
        {
            // arrange
            var roomName = "room001";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.Group(roomName)).Returns(mockClientProxy.Object);

            var simpleHub = new MessageHub(It.IsAny<IStockService>())
            {
                Clients = mockClients.Object
            };

            var message = "Hello, world!";
            var user = "demo@site.com";


            // act
            await simpleHub.SendMessage(user, message, roomName);

            // assert
            mockClients.Verify(clients => clients.Group(roomName), Times.Once);

            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveMessage", new object[] { "demo@site.com", "Hello, world!" },
                    default),
                Times.Once);
        }

        [Fact]
        public async Task SendCommand_Should_Send_Error_Message_When_Command_Is_Not_Valid()
        {
            // arrange
            var roomName = "room001";

            var mockFactory = new Mock<IHttpClientFactory>();
            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            var mockContext = new Mock<HubCallerContext>();
            mockContext.SetupGet(m => m.ConnectionId).Returns("UserGuid");

            mockClients.Setup(clients => clients.Client("UserGuid")).Returns(mockClientProxy.Object);

            var simpleHub = new MessageHub(It.IsAny<IStockService>())
            {
                Clients = mockClients.Object,
                Context = mockContext.Object
            };

            var message = "/stock=";
            var user = "demo@site.com";

            // act
            await simpleHub.SendMessage(user, message, roomName);

            // assert
            mockClientProxy.Verify(
                clientProxy => clientProxy.SendCoreAsync(
                    "ReceiveErrorMessage", new object[] { "Command value is missing" },
                    default),
                Times.Once);
        }

        [Fact]
        public async Task SendCommand_Should_Send_Right_Message_When_Command_Is_Valid()
        {
            // arrange
            var roomName = "room001";

            var mockFactory = new Mock<IHttpClientFactory>();

            var mockStockService = new Mock<IStockService>();
            mockStockService.Setup(x => x.GetStock(It.IsAny<string>())).ReturnsAsync(new Tuple<bool,string>(true, "Apple Inc."));

            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.Group(roomName)).Returns(mockClientProxy.Object);

            var simpleHub = new MessageHub(mockStockService.Object)
            {
                Clients = mockClients.Object
            };

            var message = "/stock=aapl.us";
            var user = "demo@site.com";

            // act
            await simpleHub.SendMessage(user, message, roomName);

            // Assert
            mockStockService.Verify(m => m.GetStock("aapl.us"), Times.Once);
            mockClientProxy.Verify(
               clientProxy => clientProxy.SendCoreAsync(
                   "ReceiveMessage", new object[] { "demo@site.com", "Apple Inc." },
                   default),
               Times.Once);
            mockClients.VerifyAll();
        }

        [Fact]
        public async Task LeaveRoom_Should_Leave_A_ChatRoom()
        {
            // arrange
            var mockGroupManager = new Mock<IGroupManager>();
            mockGroupManager.Setup(m => m.RemoveFromGroupAsync(It.IsAny<string>(), "testGroup", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockContext = new Mock<HubCallerContext>();
            mockContext.SetupGet(m => m.ConnectionId).Returns("UserGuid");

            var simpleHub = new MessageHub(It.IsAny<IStockService>())
            {
                Groups = mockGroupManager.Object,
                Context = mockContext.Object
            };

            // act
            await simpleHub.LeaveRoom("testGroup");

            // Assert
            mockGroupManager.Verify(m => m.RemoveFromGroupAsync("UserGuid", "testGroup", default), Times.Once);

        }
    }
}