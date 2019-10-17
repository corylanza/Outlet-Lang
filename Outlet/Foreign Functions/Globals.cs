using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.FFI
{
    [ForeignClass(Name = "console", IsStatic = true)]
    public class Global
    {
        [ForeignFunction(Name = "print")]
        public static void PrintThis(object text) => Console.WriteLine(text);

        [ForeignFunction(Name = "read")]
        public static string ReadThis() => Console.ReadLine();
    }

    [ForeignClass(Name = "math", IsStatic = true)]
    public class MathO
    {
        [ForeignField(Name = "pi")]
        public const float PI = (float) Math.PI;

        [ForeignField(Name = "number")]
        public static int Number = 5;

        [ForeignFunction(Name = "change")]
        public static void Change() => Number++;

        [ForeignFunction(Name = "sin")]
        public static float MathSin(float input) => (float) Math.Sin(input);
    }
}
