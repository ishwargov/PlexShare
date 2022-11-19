using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexShareCloudUX
{
    internal class TestFileread
    {
        static void Main(string[] args)
        {
            // Store the path of the textfile in your system
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            currentPath = currentPath + "OnlineSetup_Path.txt";
            string file = @currentPath;
            //string file = @"C:\Users\polisetty vamsi\source\repos\PVamsi5\PlexShare\src\PlexShareCloudUX\OnlineSetup_Path.txt";

            // To read a text file line by line
            if (File.Exists(file))
            {
                // Store each line in array of strings
                string[] lines = File.ReadAllLines(file);

                foreach (string ln in lines)
                    Console.WriteLine(ln);
            }
            Console.WriteLine();
        }
    }
}