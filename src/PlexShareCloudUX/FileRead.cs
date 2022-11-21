/******************************************************************************
 * Filename    = FileRead.cs
 *
 * Author      = Polisetty Vamsi
 *
 * Product     = PlexShare
 * 
 * Project     = PlexShareCloudUx
 *
 * Description = Reads the content from the local file.  
 *****************************************************************************/

using System.Diagnostics;
using System.IO;

namespace PlexShareCloudUX
{
    public class FileRead
    {
        /// <summary>
        /// Function for reading the required urls from a local file. 
        /// </summary>
        /// <param name="filename">Name of the local file</param>
        /// <returns>Returns the content in the given file</returns>
        public static string[] GetPaths(string filename)
        {
            DirectoryInfo di = new DirectoryInfo("..\\..\\..\\");//For removing directory dependency until bin directory. 
            // Store the path of the textfile in your system
            string currentPath = di.FullName;
            //Console.WriteLine(di.FullName);
            currentPath += filename;
            string file = @currentPath;
            string[] lines = { "", "" };

            // To read a text file line by line
            if (File.Exists(file))
            {
                // Store each line in array of strings
                Trace.WriteLine("[cloud] Reading the content from the file " + filename);
                lines = File.ReadAllLines(file);
            }
            else
            {
                //logger for to create file with the urls required. 
                Trace.WriteLine("[cloud] Given file path is found");
                lines[0] = "File Not Found";
            }

            return lines;
        }
    }
}
