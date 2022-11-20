using PlexShareContent.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PlexShareContent;

namespace PlexShareTests.DashboardTests.Summary
{
    internal class Utils
    {
        /// <summary>
        ///     Based on the query the appropriate chat context is returned
        /// </summary>
        /// <param name="query">
        ///     The query for the chat context
        /// </param>
        /// <returns>
        ///     The appropriate chat context array
        /// </returns>
        public static ChatThread[] GetChatThread(string query)
        {
            if (query == "Null Context") return null;

            if (query == "Empty chat context") return Array.Empty<ChatThread>();

            if (query == "Empty chats")
            {
                List<ChatThread> chats = new();
                for (var i = 0; i < 50; i++)
                {
                    ReceiveContentData data = new();
                    // All null messages but users are initialized.
                    data.Data = "";
                    data.Type = MessageType.Chat;
                    data.Starred = false;
                    ChatThread c = new();
                    List<ReceiveContentData> ReceiveContentDatas = new();
                    ReceiveContentDatas.Add(data);
                    c.MessageList = ReceiveContentDatas;
                    chats.Add(c);
                }

                return chats.ToArray();
            }

            if (query == "Fixed chat")
            {
                List<ChatThread> chats = new();
                for (var i = 0; i < 50; i++)
                {
                    ChatThread c = new();
                    List<ReceiveContentData> ReceiveContentDatas = new();
                    for (var j = 0; j < 5; j++)
                    {
                        ReceiveContentData data = new();
                        // A constant message CONST is sent by all the users.
                        data.Data = "CONST";
                        data.Type = MessageType.Chat;
                        data.Starred = false;
                        ReceiveContentDatas.Add(data);
                    }

                    c.MessageList   = ReceiveContentDatas;
                    chats.Add(c);
                }

                return chats.ToArray();
            }

            if (query == "Variable chat")
            {
                List<ChatThread> chats = new();
                ChatThread c = new();
                // Just to check the working of the Porter Stemmer to obtain the lemmas.
                List<ReceiveContentData> ReceiveContentDatas = new();
                ReceiveContentData step1 = new();
                step1.Data = "caresses. plastered. troubled. happy";
                step1.Type = MessageType.Chat;
                step1.Starred = false;
                ReceiveContentDatas.Add(step1);
                ReceiveContentData step2 = new();
                step2.Data = "relational. hesitanci. vietnamization";
                step2.Type = MessageType.Chat;
                step2.Starred = false;
                ReceiveContentDatas.Add(step2);
                ReceiveContentData step3 = new();
                step3.Data = "triplicate. formalize. electrical";
                step3.Type = MessageType.Chat;
                step3.Starred = true;
                ReceiveContentDatas.Add(step3);
                ReceiveContentData step4 = new();
                step4.Data = "allowance. defensible. homologous";
                step4.Type = MessageType.Chat;
                step4.Starred = false;
                ReceiveContentDatas.Add(step4);
                ReceiveContentData step5 = new();
                step5.Data = "probate. controll. roll";
                step5.Type = MessageType.Chat;
                step5.Starred = false;
                ReceiveContentDatas.Add(step5);
                c.MessageList   = ReceiveContentDatas;
                chats.Add(c);
                return chats.ToArray();
            }
            else
            {
                List<ChatThread> chats = new();
                for (var i = 0; i < 50; i++)
                {
                    // General chat context resembling real application cases.
                    ChatThread c = new();
                    List<ReceiveContentData> ReceiveContentDatas = new();
                    for (var j = 0; j < 5; j++)
                    {
                        ReceiveContentData data = new();
                        data.Data = "Hi from " + (i + j);
                        if (i % 5 == 0)
                            data.Data += ".This is special";
                        data.Type = MessageType.Chat;
                        data.Starred = i % 5 == 0;
                        ReceiveContentDatas.Add(data);
                    }

                    c.MessageList   = ReceiveContentDatas;
                    chats.Add(c);
                }

                return chats.ToArray();
            }
        }
    }

}
