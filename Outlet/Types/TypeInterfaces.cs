using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Types
{
    public interface IGenericType
    {
        Type WithTypeArguments(IEnumerable<Type> typeArgs);
    }
}
