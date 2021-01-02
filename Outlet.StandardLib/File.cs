using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Outlet.StandardLib
{
    [ForeignClass(Name = "File")]
    public class OFile
    {
        [ForeignField]
        public string Path;

        public OFile(string path)
        {
            Path = path;
        }

        [ForeignFunction(Name = "open")]
        public static OFile Open(string path) => new OFile(path);

        [ForeignFunction(Name = "read")]
        public string[] Read()
        {
            return File.ReadAllLines(Path);
        }
    }
}
