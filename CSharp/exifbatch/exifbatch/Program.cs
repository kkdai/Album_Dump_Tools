using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic.FileIO;

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
                Console.WriteLine(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // Rename Files
                    Console.WriteLine(fi.FullName);
                    string exif_name = ReadNameFile(fi.FullName);
                    Console.WriteLine("name=" + exif_name);
                    string exif_desc = ReadDescFile(fi.FullName);
                    Console.WriteLine("desc=" + exif_name);
                    Update_Title(fi.FullName, exif_name);
                    Update_Desc(fi.FullName, exif_desc);
                    Console.WriteLine(fi.FullName + " update exif done..");
                    Thread.Sleep(50);
                }

                subDirs = root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }

                // Rename folder
                Thread.Sleep(500);
                try
                {
                    files = root.GetFiles("FOLDER_NAME.TXT");
                     foreach (System.IO.FileInfo fi in files)
                     {       
                         string folder_name = ReadFolderName(fi.FullName);

                         //Remove all text file after modification name.
                         System.IO.FileInfo[] files_rm = null;
                         files_rm = root.GetFiles("*.TXT");
                         foreach (System.IO.FileInfo fi_rm in files_rm)
                         {
                             File.Delete(fi_rm.FullName);
                         }
                         Thread.Sleep(300);

                         //Remove all original image file.
                         files_rm = root.GetFiles("*.jpg_original");
                         foreach (System.IO.FileInfo fi_rm in files_rm)
                         {
                             File.Delete(fi_rm.FullName);
                         }
                         Thread.Sleep(300);
                         
                         if (folder_name.Length > 0)
                         {
                             folder_name = folder_name.Trim(new[] { '\\', '*', '/', '?', ':', '>', '<', '|', '"' });
                             FileSystem.RenameDirectory(root.FullName, folder_name);
                             Thread.Sleep(1);
                         }
                     }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static string ReadFolderName(string file_path)
        {
            return ReadFileContent(file_path);
        }

        static string ReadNameFile(string jpg_file_name)
        {
            string txt_name = jpg_file_name + ".name.txt";
            return ReadFileContent(txt_name);
        }
        static string ReadDescFile(string jpg_file_name)
        {
            string txt_name = jpg_file_name + ".desc.txt";
            return ReadFileContent(txt_name);
        }

        static string ReadFileContent(string soruce_file)
        {
            string file_content = "";
            if (File.Exists(soruce_file))
            {
                StreamReader sr = new StreamReader(soruce_file, System.Text.Encoding.Default);
                while (!sr.EndOfStream)
                {                                       
                    file_content += sr.ReadLine();      
                }
                sr.Close();
            }
            return file_content;

        }
        static string Generate_EscStringe(string soruce_string) 
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in soruce_string)
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
            return sb.ToString();
        }
        static void Update_Title(string file_name, string title)
        {
            string esc_title = Generate_EscStringe(title);
            Update_Exif(file_name, string.Format("-title=\"{0}\"", esc_title));
        }

        static void Update_Desc(string file_name, string desc)
        {
            string esc_desc = Generate_EscStringe(desc);
            Update_Exif(file_name, string.Format("-description=\"{0}\"", esc_desc));
        }

        static void Update_Exif(string file_name, string update_command)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe"; 
            startInfo.Arguments = string.Format("/C d:\\exiftool.exe {0} -E {1}", update_command, file_name);
            Console.WriteLine("cmd =>" + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();
        }

        static void Main(string[] args)
        {
            string process_path = null;
            if (args.Length == 0)
            {
                //no paramter use correct.
                process_path = Directory.GetCurrentDirectory();
            }
            else
            {
                process_path = Path.GetFullPath(args[0]);
            }
            DirectoryInfo walking_directory = new DirectoryInfo(process_path);
            WalkDirectoryTree(walking_directory);


            //System.Diagnostics.Process process = new System.Diagnostics.Process();
            //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            //startInfo.FileName = "cmd.exe";
            //string value = "許重功";
            //StringBuilder sb = new StringBuilder();
            //foreach (char c in value)
            //{
            //    if (c > 127)
            //    {
            //        // This character is too big for ASCII
            //        string encodedValue = "&#" + ((int)c).ToString("d") + ";";
            //        sb.Append(encodedValue);
            //    }
            //    else
            //    {
            //        sb.Append(c);
            //    }
            //}


            //Console.WriteLine(sb.ToString());
            //startInfo.Arguments = string.Format("/C exiftool.exe -title=\"{0}\" -E test1.jpg", sb.ToString());
            //Console.WriteLine(startInfo.Arguments);
            //process.StartInfo = startInfo;
            //process.Start();
        }
    }
}
