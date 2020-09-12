using Outlet.ForeignFunctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.StandardLib
{
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
}
