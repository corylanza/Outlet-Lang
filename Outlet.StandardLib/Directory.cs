using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "Directory")]
    public static class ODirectory
    {
        [ForeignFunction(Name = "current")]
        public static string Current() => Directory.GetCurrentDirectory();

        [ForeignFunction(Name = "cd")]
        public static void CD(string path) => Directory.SetCurrentDirectory(path);

        [ForeignFunction(Name = "ls")]
        public static void LS()
        {
            var list = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory());
            foreach (var file in list)
            {
                Console.WriteLine(file);
            }
        }
    }
}
