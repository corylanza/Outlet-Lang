using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet
{
    public interface IBindable
    {
        uint? LocalId { get; }
        uint? ResolveLevel { get; }
        string Identifier { get; }

        void Bind(uint id, uint level);
    }
}
