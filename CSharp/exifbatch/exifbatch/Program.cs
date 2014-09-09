using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace exifbatch
{
    class Program
    {
        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.jpg");
            }
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                //log.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    Console.WriteLine(fi.FullName);
                }

                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        static void Main(string[] args)
        {
            string process_path = null;
            if (args.Length == 0)
            {
                //no paramter use correct.
                process_path = Directory.GetCurrentDirectory();
            } else
            {
                process_path = Path.GetFullPath(args[0]);
            }
            DirectoryInfo walking_directory = new DirectoryInfo(process_path);
            WalkDirectoryTree(walking_directory);


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            string value = "許重功";
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "&#" + ((int)c).ToString("d") + ";";
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }


            Debug.WriteLine(sb.ToString());
            startInfo.Arguments = string.Format("/C exiftool.exe -title=\"{0}\" -E test1.jpg", sb.ToString());
            Debug.WriteLine(startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
