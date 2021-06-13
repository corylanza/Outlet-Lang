using Outlet.AST;
using Outlet.Compiling.ByteCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Compiling
{
    public class ByteCodeGenerator : IASTVisitor<CodeBlock>
    {
        public CodeBlock Visit(ClassDeclaration c)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(ConstructorDeclaration c)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(FunctionDeclaration f)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(OperatorOverloadDeclaration o)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(VariableDeclaration v)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Access a)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(As a)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Assign a)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Binary b)
        {
            IEnumerable<byte> left = b.Left.Accept(this);
            IEnumerable<byte> right = b.Right.Accept(this);
            var bytes = b.Oper!.GenerateByteCode(left, right);
            return new CodeBlock(bytes);
        }

        public CodeBlock Visit(Call c)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit<E>(Literal<E> c) where E : struct
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(StringLiteral s)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(NullExpr n)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Declarator d)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(TupleAccess d)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(MemberAccess d)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Is i)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Lambda l)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(ListLiteral l)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(ShortCircuit s)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Ternary t)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(TupleLiteral t)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Unary u)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Variable v)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(Block b)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(ForLoop f)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(IfStatement i)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(ReturnStatement r)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(WhileLoop w)
        {
            throw new NotImplementedException();
        }

        public CodeBlock Visit(UsingStatement u)
        {
            throw new NotImplementedException();
        }
    }
}
