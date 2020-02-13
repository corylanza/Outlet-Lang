using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet
{
    public interface IStackFrame<T>
    {
        void Assign(IBindable variable, T value);

        T Get(IBindable variable);

        IEnumerable<(string Id, T Value)> List();
    }
}
