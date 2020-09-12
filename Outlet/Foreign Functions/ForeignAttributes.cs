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
    }


    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ForeignFunction : ForeignInterface
    {
        public bool IsStatic { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ForeignField : ForeignInterface
    {
        public bool IsStatic { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ForeignClass : ForeignInterface
    {
    }

    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class ForeignConstructor : ForeignInterface
    {
    }
}
