using Outlet.Compiling.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Interpreting.ByteCode
{
    public partial class VirtualMachine : IInstructionVisitor<object>
    {
        public object Visit(ConstInt c)
        {
            Accumulator = c.Value;
            return c.Value;
        }

        public object Visit(ConstFloat c)
        {
            throw new NotImplementedException();
        }

        public object Visit(ConstBool c)
        {
            throw new NotImplementedException();
        }

        public object Visit(ConstString c)
        {
            throw new NotImplementedException();
        }

        public object Visit(NegateInt n)
        {
            Accumulator = -1 * Math.Abs(Accumulator);
            return Accumulator;
        }
    }
}
