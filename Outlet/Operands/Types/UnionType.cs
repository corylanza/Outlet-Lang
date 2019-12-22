using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands
{
    public class UnionType : Type
    {

        public Type First;
        public Type Second;

        public UnionType(Type first, Type second)
        {
            First = first;
            Second = second;
        }


        public override bool Equals(Operand b)
        {
            return b is UnionType u && ((First.Equals(u.First) && Second.Equals(u.Second)) || (Second.Equals(u.First) && First.Equals(u.Second))); 
        }

        public override bool Is(Type t, out int level)
        {
            level = -1;
            return false;// First.Is(t, out level) || Second.Is(t, out level);
        }

        public override string ToString()
        {
            return First.ToString() + "/" + Second.ToString();
        }
    }
}
