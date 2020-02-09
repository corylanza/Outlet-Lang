using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet
{
    public interface IBindable
    {
        int LocalId { get; }
        int ResolveLevel { get; }
        string Identifier { get; }

        void Bind(int id, int level);
    }
}
