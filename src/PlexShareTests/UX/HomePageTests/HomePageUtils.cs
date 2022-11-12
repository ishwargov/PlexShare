//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Dashboard;
//using Dashboard.Client.SessionManagement;

//namespace PlexShareTests.UX.HomePageTests
//{
//    internal class HomePageUtils
//    {
//        public class FakeClientSessionManager : IUXClientSessionManager
//        {
//            public bool AddClient(string ipAddress, int ports, string username)
//            {
//                if (string.IsNullOrWhiteSpace(ipAddress) || string.IsNullOrWhiteSpace(username) ||
//                    ipAddress.Contains(" "))
//                    return false;
//                return true;
//            }

//            /// <summary>
//            ///     Removes the user from the meeting by deleting their
//            ///     data from the session.
//            /// </summary>
//            public void RemoveClient()
//            {
//            }

//            /// <summary>
//            ///     End the meeting for all, creating and storing the summary and analytics.
//            /// </summary>
//            public void EndMeet()
//            {
//            }

//            /// <summary>
//            ///     Get the summary of the chats that were sent from the start of the
//            ///     meet till the function was called.
//            /// </summary>
//            /// <returns> Summary of the chats as a string. </returns>
//            public void GetSummary()
//            {
//            }

//            /// <summary>
//            ///     Used to subcribe for any changes in the
//            ///     Session object.
//            /// </summary>
//            /// <param name="listener"> The subscriber. </param>
//            public void SubscribeSession(IClientSessionNotifications listener)
//            {
//            }

//            /// <summary>
//            ///     Gather analytics of the users and messages.
//            /// </summary>
//            public void GetAnalytics()
//            {
//            }

//            public UserData GetUser()
//            {
//                throw new NotImplementedException();
//            }

//            // Event for notifying summary creation 
//            public event NotifySummaryCreated SummaryCreated;

//            // Event for notifying the end of the meeting on the client side
//            public event NotifyEndMeet MeetingEnded;

//            // Event for notifying the creation of anlalytics to the client UX.
//            public event NotifyAnalyticsCreated AnalyticsCreated;
//        }
//    }
//}
