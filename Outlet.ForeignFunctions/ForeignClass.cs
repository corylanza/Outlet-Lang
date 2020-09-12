using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.ForeignFunctions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ForeignClass : Attribute
    {
        public string? Name { get; set; }
    }
}
