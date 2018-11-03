using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet {
    public class Operation {
        public Operation(Type l, Type r, Type res, Func<Operand, Operand, Operand> f) {

        }
    }
}
