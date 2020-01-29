using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet
{
    public interface IDeclarable
    {
        int LocalId { get; set; }
        string Identifier { get; }
    }

    public interface IVariable
    {
        int LocalId { get; set; }
        string Identifier { get; }
    }
}
