using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Compiling.Instructions
{
    public abstract class Instruction
    {
        public abstract T Accept<T>(IInstructionVisitor<T> visitor);

        public abstract override string ToString();
    }
}
