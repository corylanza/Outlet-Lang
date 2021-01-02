using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.ForeignFunctions
{
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public sealed class ForeignConstructor : Attribute
    {
    }
}
