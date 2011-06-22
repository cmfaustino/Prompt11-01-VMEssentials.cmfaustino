using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sessao2_3
{
    class Utils
    {
        public static void ProcessFiles(DirectoryInfo rootFolder, Func<FileInfo, bool> pred, Action<FileInfo> action)
        {
            Console.WriteLine("\nConteúdo processado de " + rootFolder.FullName + "\n");

            foreach (FileInfo fi in rootFolder.GetFiles())
            {
                if (pred(fi)) action(fi);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in rootFolder.GetDirectories())
            {
                ProcessFiles(diSourceSubDir, pred, action);
            }
                
        }
    }

    class Program
    {
        //static bool recent_changed_size_bigger(FileInfo fi)
        //{
        //    return true;
        //}

        static void cmd_dir_output(FileInfo fi)
        {
            Console.WriteLine("{0}\t{1}\t{2} bytes", fi.FullName, fi.LastWriteTime, fi.Length);
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(args[0]);
            //Console.ReadLine();

            //Utils.ProcessFiles(new DirectoryInfo(@"c:\program files"), recent_changed_size_bigger, cmd_dir_output);
            Utils.ProcessFiles(new DirectoryInfo(@"c:\z_prompt"), delegate (FileInfo fi) {
                return (fi.Length > 300) && (fi.LastWriteTime > new DateTime(2011,01,01));
            }, cmd_dir_output);
            Console.ReadLine();
        }
    }
}
