﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
    public abstract class Declaration : IASTNode {

		public string Name => Decl.Identifier;
        public Declarator Decl { get; private init; }

        protected Declaration(Declarator decl)
        {
            Decl = decl;
        }

		public abstract T Accept<T>(IVisitor<T> visitor);

		public abstract override string ToString();
    }
}
