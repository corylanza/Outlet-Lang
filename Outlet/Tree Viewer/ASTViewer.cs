using Outlet.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outlet.TreeViewer
{
    public class ASTViewer : IVisitor<Node>
    {
        public Node BuildTree(params IASTNode[] nodes)
        {
            return nodes.First().Accept(this);
        }

        public Node Visit(ClassDeclaration c)
        {
            throw new NotImplementedException();
        }

        public Node Visit(ConstructorDeclaration c)
        {
            throw new NotImplementedException();
        }

        public Node Visit(FunctionDeclaration f)
        {
            throw new NotImplementedException();
        }

        public Node Visit(OperatorOverloadDeclaration o)
        {
            throw new NotImplementedException();
        }

        public Node Visit(VariableDeclaration v)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Access a)
        {
            throw new NotImplementedException();
        }

        public Node Visit(As a)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Assign a) => new OperationNode("=", a.Left.Accept(this), a.Right.Accept(this));

        public Node Visit(Binary b) => new OperationNode(b.Op, b.Left.Accept(this), b.Right.Accept(this));

        public Node Visit(Call c)
        {
            throw new NotImplementedException();
        }

        public Node Visit<E>(Literal<E> c) where E : struct => new PrimitiveNode(c.Value.ToString() ?? "");

        public Node Visit(StringLiteral s)
        {
            throw new NotImplementedException();
        }

        public Node Visit(NullExpr n) => new PrimitiveNode("null");

        public Node Visit(Declarator d)
        {
            throw new NotImplementedException();
        }

        public Node Visit(TupleAccess d)
        {
            throw new NotImplementedException();
        }

        public Node Visit(MemberAccess d)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Is i)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Lambda l)
        {
            throw new NotImplementedException();
        }

        public Node Visit(ListLiteral l)
        {
            throw new NotImplementedException();
        }

        public Node Visit(ShortCircuit s)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Ternary t)
        {
            throw new NotImplementedException();
        }

        public Node Visit(TupleLiteral t)
        {
            throw new NotImplementedException();
        }

        public Node Visit(Unary u) => new OperationNode(u.Op, u.Expr.Accept(this));

        public Node Visit(Variable v) => new VariableNode(v.Identifier);

        public Node Visit(Block b)
        {
            throw new NotImplementedException();
        }

        public Node Visit(ForLoop f)
        {
            throw new NotImplementedException();
        }

        public Node Visit(IfStatement i)
        {
            throw new NotImplementedException();
        }

        public Node Visit(ReturnStatement r)
        {
            throw new NotImplementedException();
        }

        public Node Visit(WhileLoop w)
        {
            throw new NotImplementedException();
        }

        public Node Visit(UsingStatement u)
        {
            throw new NotImplementedException();
        }
    }
}
