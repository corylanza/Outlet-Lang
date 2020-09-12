using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.FFI
{
    [ForeignClass(Name = "console")]
    public static class ConsoleWrapper
    {
        [ForeignFunction(Name = "print")]
        public static void PrintThis(object text) => Console.WriteLine(text);

        [ForeignFunction(Name = "read")]
        public static string ReadThis() => Console.ReadLine();
    }

    [ForeignClass(Name = "math")]
    public static class MathO
    {
        [ForeignField(Name = "pi")]
        public const float PI = (float)Math.PI;

        [ForeignField(Name = "number")]
        public static int Number = 5;

        [ForeignFunction(Name = "change")]
        public static void Change() => Number++;

        [ForeignFunction(Name = "sin")]
        public static float MathSin(float input) => (float)Math.Sin(input);

        [ForeignFunction(Name = "max")]
        public static int Max(int a, int b) => a > b ? a : b;

        [ForeignFunction(Name = "max")]
        public static int Max(int a, int b, int c) => a > b ? a : b > c ? b : c;
    }

    [ForeignClass(Name = "outlet")]
    public static class Outlet
    {
        [ForeignFunction(Name = "run")]
        public static void Run(string name)
        {
            Program.RunFile(Directory.GetCurrentDirectory() + @"\Outlet\Test\" + name + ".txt");
        }
    }

    [ForeignClass(Name = "dict")]
    public class ODictionary 
    {
        private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

        [ForeignConstructor]
        public ODictionary()
        {

        }

        [ForeignFunction(Name = "get")]
        public object Get(string s) => dictionary[s];

        [ForeignFunction(Name = "set")]
        public void Set(string s, object value) => dictionary[s] = value;

        [ForeignFunction(Name = "keys")]
        public IEnumerable<string> Keys() => dictionary.Keys;

        [ForeignFunction(Name = "values")]
        public IEnumerable<object> Values() => dictionary.Values;
    }

    [ForeignClass(Name = "list")]
    public class OList
    {
        private readonly List<object> list = new List<object>();

        [ForeignConstructor]
        public OList()
        {

        }

        [ForeignFunction(Name = "get")]
        public object Get(int i) => list[i];

        [ForeignFunction(Name = "push")]
        public void Push(object o) => list.Add(o);

        [ForeignFunction(Name = "peek")]
        public object Peek() => list.Last();

        [ForeignFunction(Name = "pop")]
        public object Pop()
        {
            var temp = list.Last();
            list.RemoveAt(list.Count - 1);
            return temp;
        }

        [ForeignFunction(Name = "toArray")]
        public IEnumerable<object> ToArray() => list;
    }

    //[ForeignClass(Name = "File", IsStatic = false)]
    //public class OFile
    //{
    //    private string Path;

    //    [ForeignFunction(Name = "open")]
    //    public static OFile Open(string path)
    //    {
    //        return new OFile()
    //        {
    //            Path = path
    //        };
    //    }

    //    [ForeignFunction(Name = "read")]
    //    public string[] Read()
    //    {
    //        return File.ReadAllLines(Path);
    //    }
    //}

    //[ForeignClass(Name = "Directory")]
    //public class ODirectory
    //{
    //    [ForeignFunction(Name = "current")]
    //    public static string Current() => Directory.GetCurrentDirectory();

    //    [ForeignFunction(Name = "cd")]
    //    public static void CD(string path) => Directory.SetCurrentDirectory(path);

    //    [ForeignFunction(Name = "ls")]
    //    public static void LS()
    //    {
    //        var list = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory());
    //        foreach (var file in list)
    //        {
    //            Console.WriteLine(file);
    //        }
    //    }
    //}
}
