using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.FFI
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ForeignFunction : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ForeignField : Attribute
    {
        public string Name { get; set; }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ForeignClass : Attribute
    {
        public string Name { get; set; }
        public bool IsStatic { get; set; }
    }
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class ForeignConstructor : Attribute
    {

    }
}
