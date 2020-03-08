using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;

namespace Outlet.FFI
{
    public class ForeignInterface : Attribute
    {
        public string? Name { get; set; }
        public bool IsStatic { get; set; }
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ForeignFunction : ForeignInterface
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ForeignField : ForeignInterface
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ForeignClass : ForeignInterface
    {
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class ForeignConstructor : ForeignInterface
    {
    }
}
