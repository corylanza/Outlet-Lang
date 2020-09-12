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
        private string? Path;

        [ForeignFunction(Name = "open")]
        public static OFile Open(string path)
        {
            return new OFile()
            {
                Path = path
            };
        }

        [ForeignFunction(Name = "read")]
        public string[] Read()
        {
            return File.ReadAllLines(Path);
        }
    }
}
