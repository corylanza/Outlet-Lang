using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.ForeignFunctions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ForeignField : Attribute
    {
        public string? Name { get; set; }
        public bool IsStatic { get; set; }
    }
}
