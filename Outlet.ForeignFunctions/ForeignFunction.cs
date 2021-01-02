using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.ForeignFunctions
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ForeignFunction : Attribute
    {
        public string? Name { get; set; }
        public bool IsStatic { get; set; }
    }
}
