using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet
{
    public interface IStackFrame<T>
    {
        void Assign(IBindable variable, T value, uint level = 0);

        T Get(IBindable variable, uint level = 0);

        IEnumerable<(string Id, T Value)> List();
    }
}
