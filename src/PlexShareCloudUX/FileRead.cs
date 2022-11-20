using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PlexShareCloudUX
{
    public class FileRead
    {
        public static string[] GetPaths(string filename)
        {
            // Store the path of the textfile in your system
            DirectoryInfo di = new DirectoryInfo("..\\..\\..\\");//For removing directory dependency until bin directory. 
            string currentPath = di.FullName;
            //Console.WriteLine(di.FullName);
            currentPath += filename;
            string file = @currentPath;
            string[] lines = { "", "" }; 
            // To read a text file line by line
            if (File.Exists(file))
            {
                // Store each line in array of strings
                lines = File.ReadAllLines(file);

                foreach (string ln in lines)
                    Console.WriteLine(ln);
            }
            else
            {
                //logger for to create file with the urls required. 
                lines[0] = "File Not Found";
            }
            return lines;
        }
    }
}
