/// <author>Morem Jayanth Kumar</author>
/// <created>3/11/2022</created>
/// <summary>
///		This file represents the main functions of the summarizer
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlexShareContent.DataModels;

namespace PlexShareDashboard.Dashboard.Server.Summary
{
    public interface ISummarizer
    {

        /// <summary>
        ///     Function to get the Summary of the chat and
        ///     discussion to present in the Dashboard
        /// </summary>
        /// <param name="chats">
        ///     Array of ChatContext each of
        ///     which would contain an array of chat messages
        ///     which would be used for the summarizer to
        ///     generate the summary
        /// </param>
        /// <returns>
        ///     String which is the summary of the
        ///     chat in the particular discusiion
        /// </returns>
        string GetSummary(ChatThread[] chats);


        /// <summary>
        ///     Function to save the summary of the dashboard
        ///     discussion that took place into the database
        ///     using the persistence module.
        /// </summary>
        /// <param name="chats">
        ///     Array of ChatContext each of
        ///     which would contain an array of chat messages
        ///     which would be used for the summarizer to
        ///     save the summary in the database
        /// </param>    
        /// <returns>
        ///     Returns true if summary was succesfully stored
        ///     and false otherwise
        /// </returns>
        bool SaveSummary(ChatThread[] chats);

    }
}
