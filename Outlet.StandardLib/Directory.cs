using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "dir")]
    public class ODirectory
    {
        [ForeignField]
        public string Path;

        [ForeignConstructor]
        public ODirectory(string path)
        {
            Path = path;
        }

        [ForeignFunction(Name = "current")]
        public static ODirectory Current() => new ODirectory(Directory.GetCurrentDirectory());

        [ForeignFunction(Name = "parent")]
        public ODirectory Parent() => new ODirectory(Directory.GetParent(Path).FullName);

        [ForeignFunction(Name = "list")]
        public OFile[] ListFiles() => Directory.GetFiles(Path).Select(filepath => OFile.Open(filepath)).ToArray();

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
