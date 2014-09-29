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
                    Console.WriteLine(fi.FullName);
                    string exif_name = ReadNameFile(fi.FullName);
                    Console.WriteLine("name=" + exif_name);
                    string exif_desc = ReadDescFile(fi.FullName);
                    Console.WriteLine("desc=" + exif_desc);
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

                        //Remove all original image file.
                        System.IO.FileInfo[] files_rm = null;
                        files_rm = root.GetFiles("*.jpg_original");
                        foreach (System.IO.FileInfo fi_rm in files_rm)
                        {
                            File.Delete(fi_rm.FullName);
                        }
                        Thread.Sleep(300);

                        if (folder_name.Length > 0)
                        {
                            folder_name = RemoveInvalidCharInPath(folder_name);
                            FileSystem.RenameDirectory(root.FullName, folder_name);
                            //Sleep to avoid CPU overload..
                            Thread.Sleep(10);
                        }
                    }
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        static void WalkTwoDirectoryTrees(System.IO.DirectoryInfo process_root, System.IO.DirectoryInfo target_root)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = target_root.GetFiles("*.jpg");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            // Walk all target JPG to find txt.
            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    string jpg_name = System.IO.Path.GetFileNameWithoutExtension(fi.Name);
                    Console.WriteLine("Currently jpg file:" + jpg_name);
                    jpg_name = TrimeSpecificChar(jpg_name, '_');
                    jpg_name = TrimeSpecificChar(jpg_name, '(');
                    Console.WriteLine("Trim to file:" + jpg_name);

                    string pic_name_file = FindFileFullPath(process_root, jpg_name, ".name.txt");
                    Console.WriteLine("Currently find name file" + pic_name_file + "\n");
                    string exif_name = ReadFileContent(pic_name_file);
                    Console.WriteLine(exif_name);

                    string pic_desc_file = FindFileFullPath(process_root, jpg_name, ".desc.txt");
                    Console.WriteLine("Currently find desc file" + pic_desc_file + "\n");
                    string exif_desc = ReadFileContent(pic_desc_file);
                    Console.WriteLine(exif_desc);
                    
                    Update_Title(fi.FullName, exif_name);
                    Thread.Sleep(100);
                    Update_Desc(fi.FullName, exif_desc);
                    Thread.Sleep(100);
                    Console.WriteLine(fi.FullName + " update exif done..");
                }

                subDirs = target_root.GetDirectories();

                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        static string  TrimeSpecificChar(string in_str, char target_char)
        {
            string [] str_array = in_str.Split(target_char);
            return str_array[0].Trim(' ');
        }

        static string RemoveInvalidCharInPath(string ori_path)
        {
            string ret_string;
            ret_string = ori_path.Replace("\\", "");
            ret_string = ret_string.Replace("*", "");
            ret_string = ret_string.Replace("/", "");
            ret_string = ret_string.Replace("?", "");
            ret_string = ret_string.Replace("<", "");
            ret_string = ret_string.Replace(":", "");
            ret_string = ret_string.Replace(">", "");
            ret_string = ret_string.Replace("\"", "");
            return ret_string;
        }

        static void RemoveAllTextFiles(System.IO.DirectoryInfo root)
        {
            /// Remove all text file after modification name.
            System.IO.FileInfo[] files_rm = null;
            files_rm = root.GetFiles("*.TXT");
            foreach (System.IO.FileInfo fi_rm in files_rm)
            {
                File.Delete(fi_rm.FullName);
            }
            Thread.Sleep(300);
        }

        static string ReadFolderName(string file_path)
        {
            return ReadFileContent(file_path);
        }

        static string FindFileFullPath(System.IO.DirectoryInfo process_root, string file_name, string ext_name)
        {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                string target_file_set = file_name + "*" + ext_name;
                files = process_root.GetFiles(target_file_set);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            // Walk all target JPG to find txt.
            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    return fi.FullName;
                }

                subDirs = process_root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    FindFileFullPath(dirInfo, file_name, ext_name);
                }
            }
            return ""; //not found
        }

        static string FindDescFile(string jpg_file_name)
        {
            string txt_name = jpg_file_name + ".desc.txt";
            return ReadFileContent(txt_name);
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
            /// Command line as follow: (1)exifbatch.exe path1 or (2)exifbatch.exe path1 path2
            /// (1) txt and jpg in the same path
            /// (2) txt in first path and jpg in second path.
            /// 
            string process_path = "";
            string target_path = "";
            if (args.Length == 0)
            {
                //no paramter use correct.
                process_path = Directory.GetCurrentDirectory();
            }
            else if (args.Length == 1)
            {
                process_path = Path.GetFullPath(args[0]);
                DirectoryInfo walking_directory = new DirectoryInfo(process_path);
                WalkDirectoryTree(walking_directory);
            }
            else
            {
                process_path = Path.GetFullPath(args[0]);
                DirectoryInfo process_directory = new DirectoryInfo(process_path);
                target_path = Path.GetFullPath(args[1]);
                DirectoryInfo target_directory = new DirectoryInfo(target_path);
                WalkTwoDirectoryTrees(process_directory, target_directory);
            }
        }
    }
}
