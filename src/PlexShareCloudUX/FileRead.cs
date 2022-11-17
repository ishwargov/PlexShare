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
        public static string[] GetPaths(int offline=0)
        {
            // Store the path of the textfile in your system
            DirectoryInfo di = new DirectoryInfo("..\\..\\..\\");
            string currentPath = di.FullName;
            //Console.WriteLine(di.FullName);
            currentPath += "OfflineSetup_Path.txt";
            string file = @currentPath;
            string[] lines = { "", "" ,currentPath}; //remove the current path from here. 
            // To read a text file line by line
            if (File.Exists(file))
            {
                // Store each line in array of strings
                lines = File.ReadAllLines(file);

                foreach (string ln in lines)
                    Console.WriteLine(ln);
            }
            return lines;
        }
    }
}
