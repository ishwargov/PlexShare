using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareDashboard.Dashboard
{
    
    //     Class to store the summary, it also
    //     implemets the IReceivedFromServer interface so that it
    //     can be sent to the client side (from the server side)
    
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
