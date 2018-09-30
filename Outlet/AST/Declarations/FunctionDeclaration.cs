using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public class FunctionDeclaration : Declaration {

        private Scope Scope;
        private Identifier ID;
        private List<Identifier> ArgNames;
        private Statement Body;

        public FunctionDeclaration(Scope s, Identifier id, List<Identifier> argnames, Statement body) {
            Scope = s;
            ID = id;
            ArgNames = argnames;
            Body = body;
        }


        public override void Execute() {
            Function f = new Function(Scope, ID, ArgNames, Body);
            Scope.AddFunc(ID, f);
        }

        public override string ToString() {
            throw new NotImplementedException();
        }
    }
}
