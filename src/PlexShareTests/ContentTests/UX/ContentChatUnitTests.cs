/// <author>Sughandhan S</author>
/// <created>11/11/2022</created>
/// <summary>
///     All Unit Tests for Content Chat 
/// </summary>



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static PlexShareTests.ContentTests.UX.ChatUtility;
using PlexShareApp.ViewModel;
using PlexShareContent;
using Dashboard;
using Xunit;
using PlexShareContent.Enums;
using PlexShareContent.DataModels;
using Assert = Xunit.Assert;

namespace PlexShareTests.ContentTests.UX
{
    public class ContentChatUnitTests
    {
        private ChatPageViewModel? _viewModel;


        [Fact]
        public void OnClientSessionChanged_ShouldAddUsers()
        {
            // Arrange
            _viewModel = new ChatPageViewModel(true);
            var testingSession = new SessionData();
            var testingUser1 = new UserData("Sughandhan", 111901049);
            testingSession.AddUser(testingUser1);
            var testingUser2 = new UserData("Narvik", 111901035);
            testingSession.AddUser(testingUser2);
            var testingUser3 = new UserData("Jha", 111901010);
            testingSession.AddUser(testingUser3);
            

            // Act
            _viewModel.OnClientSessionChanged(testingSession);

            // Assert
            // MUST CALL DispatcherUtil.DoEvents()
            DispatcherUtil.DoEvents();
            Assert.Equal(3, _viewModel.Users.Count);
            Assert.Equal("Sughandhan", _viewModel.Users[111901049]);
            Assert.Equal("Narvik", _viewModel.Users[111901035]);
            Assert.Equal("Jha", _viewModel.Users[111901010]);
        }

        [Fact]
        public void OnMessage_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            _viewModel = new ChatPageViewModel(true);
            var testingChatData = new ReceiveContentData();
            testingChatData.Event = MessageEvent.New;
            testingChatData.Data = "Good Morning Narvik!";
            testingChatData.MessageID = 1;
            testingChatData.ReceiverIDs = new int[0];
            testingChatData.ReplyMessageID = -1;
            testingChatData.SenderID = 111901049;
            testingChatData.SentTime = DateTime.Now;
            testingChatData.Starred = false;
            testingChatData.Type = MessageType.Chat;
            var testingSession = new SessionData();
            var testingUser1 = new UserData("Sughandhan", 111901049);
            var testingUser2 = new UserData("Narvik", 111901035);
            testingSession.AddUser(testingUser1);
            testingSession.AddUser(testingUser2);
            _viewModel.OnClientSessionChanged(testingSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnMessageReceived(testingChatData);

            //Assert
            // MUST CALL DispatcherUtil.DoEvents()
            DispatcherUtil.DoEvents();
            Assert.Equal("Sughandhan", _viewModel.ReceivedMsg.Sender);
            Assert.Equal(1, _viewModel.ReceivedMsg.MessageID);
            Assert.Equal("Good Morning Narvik!", _viewModel.ReceivedMsg.IncomingMessage);
            Assert.Equal("", _viewModel.ReceivedMsg.ReplyMessage);
            Assert.True(_viewModel.ReceivedMsg.Type);
        }

        [Fact]
        public void OnAllMessages_ReceivedMsgObj_ShouldMatchReceivedMsg()
        {
            //Arrange
            _viewModel = new ChatPageViewModel(true);
            var testingAllMessages = new List<ChatThread>();
            var testingChatThread = new ChatThread();
            var testingChatData1 = new ReceiveContentData();
            var testingChatData2 = new ReceiveContentData();

            testingChatData1.Event = MessageEvent.New;
            testingChatData1.Data = "What is our Content Module progress?";
            testingChatData1.MessageID = 1;
            testingChatData1.ReceiverIDs = new int[0];
            testingChatData1.ReplyMessageID = -1;
            testingChatData1.SenderID = 111901010;
            testingChatData1.SentTime = DateTime.Now;
            testingChatData1.Starred = false;
            testingChatData1.Type = MessageType.Chat;

            testingChatData2.Event = MessageEvent.New;
            testingChatData2.Data = "We have made it steady and reliable";
            testingChatData2.MessageID = 2;
            testingChatData2.ReceiverIDs = new int[0];
            testingChatData2.ReplyMessageID = 1;
            testingChatData2.SenderID = 111901035;
            testingChatData2.SentTime = DateTime.Now;
            testingChatData2.Starred = false;
            testingChatData2.Type = MessageType.Chat;

            var sampleMsgList = new List<ReceiveContentData>();
            sampleMsgList.Add(testingChatData1);
            sampleMsgList.Add(testingChatData2);
            testingChatThread.MessageList = sampleMsgList;
            testingAllMessages.Add(testingChatThread);

            var sampleSession = new SessionData();
            var sampleUser1 = new UserData("Jha", 111901010);
            var sampleUser2 = new UserData("Narvik", 111901035);
            sampleSession.AddUser(sampleUser1);
            sampleSession.AddUser(sampleUser2);
            _viewModel.OnClientSessionChanged(sampleSession);
            DispatcherUtil.DoEvents();

            //Act
            _viewModel.OnAllMessagesReceived(testingAllMessages);

            //Assert
            // MUST CALL DispatcherUtil.DoEvents()
            DispatcherUtil.DoEvents();
            Assert.Equal("Narvik", _viewModel.ReceivedMsg.Sender);
            Assert.Equal(2, _viewModel.ReceivedMsg.MessageID);
            Assert.Equal("We have made it steady and reliable", _viewModel.ReceivedMsg.IncomingMessage);
            Assert.Equal("What is our Content Module progress?", _viewModel.ReceivedMsg.ReplyMessage);
            Assert.True(_viewModel.ReceivedMsg.Type);
        }

        /// <summary>
        /// Checking if the Sent Chat Message matches with the MsgToSend object
        /// </summary>
        [Fact]
        public void SendChatMessage_SingleThread_ShouldMatchChatMsgSent()
        {
            // Arrange
            _viewModel = new ChatPageViewModel(true);
            var testMessage = "Welcome to the Chat Page";
            var testReplyMsgID = -1;
            var testMessageType = "Chat";

            // Act
            _viewModel.SendMessage(testMessage, testReplyMsgID, testMessageType);

            // Assert
            Assert.True(_viewModel.MsgToSend.Data == "Welcome to the Chat Page");
            Assert.True(_viewModel.MsgToSend.ReplyThreadID == -1);
        }

        /// <summary>
        /// Checking if the Sent File Message matches with the MsgToSend object
        /// </summary>
        [Fact]
        public void SendFileMessage_SingleThread_ShouldMatchFileMsgSent()
        {
            // Arrange
            _viewModel = new ChatPageViewModel(true);
            var currentDirectory = Directory.GetCurrentDirectory();
            var filepath = currentDirectory.Split(new[] { "\\PlexShareTests" }, StringSplitOptions.None);
            var testMesage = filepath[0] + "\\PlexShareTests\\ContentTests\\UX\\testing_file.pdf";
            var testReplyMsgID = -1;
            var testMessageType = "File";

            // Act
            _viewModel.SendMessage(testMesage, testReplyMsgID, testMessageType);

            // Assert
            Assert.True(_viewModel.MsgToSend.ReplyThreadID == -1);
        }

        /// <summary>
        /// Checking if a Property Changed Event is raised
        /// </summary>
        [Fact]
        public void OnPropertyChanged_SingleThread_EventMustBeRaised()
        {
            // Arrange
            _viewModel = new ChatPageViewModel(true);
            var testingPropertyName = string.Empty;

            _viewModel.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                testingPropertyName = e.PropertyName;
            };

            // Act
            _viewModel.OnPropertyChanged("testing");

            // Assert
            Assert.NotNull(testingPropertyName);
            Assert.Equal("testing", testingPropertyName);
        }


    }
}
