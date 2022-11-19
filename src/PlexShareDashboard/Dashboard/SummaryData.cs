/*
 * Name : Saurabh Kumar
 * Roll : 111901046
 * File Name: SummaryData.cs
 * This file contains the class for SummaryData model
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard
{
    /// <summary>
    /// Class to store the summary
    /// </summary>
    
    public class SummaryData
    {
        public string summary;

        //     Default Constructor, necessary for serialization
        public SummaryData()
        {
        }

        //     Constructor to initialize the field summary with
        //     a given string.
       
        public SummaryData(string chatSummary)
        {
            summary = chatSummary;
        }
    }
    
}
