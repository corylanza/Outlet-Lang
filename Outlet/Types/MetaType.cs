using System;
using System.Collections.Generic;
using System.Text;

namespace Outlet.Types
{
    public class MetaType : Type
    {
        public Type Stored { get; private set; }

        public MetaType(Type type) => Stored = type;

        public override bool Is(Type t, out uint level)
        {
            level = 0;
            if(t == Primitive.MetaType)
            {
                level = 1;
                return true;
            }
            return false;
        }

        public override string ToString() => $"type({Stored})";
    }
}
