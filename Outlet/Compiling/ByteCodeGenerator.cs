using Outlet.AST;
using Outlet.Compiling.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Compiling
{
    public class ByteCodeGenerator : IASTVisitor<IEnumerable<Instruction>>
    {
        public IEnumerable<Instruction> GenerateByteCode(IASTNode program)
        {
            return program.Accept(this);
        }

        private static IEnumerable<Instruction> Seq(params IEnumerable<Instruction>[] instructions)
        {
            return instructions.SelectMany(a => a);
        }

        public IEnumerable<Instruction> Visit(ClassDeclaration c)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ConstructorDeclaration c)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(FunctionDeclaration f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(OperatorOverloadDeclaration o)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(VariableDeclaration v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Access a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(As a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Assign a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Binary b)
        {
            //IEnumerable<byte> left = b.Left.Accept(this);
            //IEnumerable<byte> right = b.Right.Accept(this);
            //var bytes = b.Oper!.GenerateByteCode(left, right);
            //return new CodeBlock(bytes);
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Call c)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit<E>(Literal<E> c) where E : struct
        {
            yield return c switch
            {
                Literal<int> l => new ConstInt(l.Value),
                _ => throw new NotImplementedException()
            };
        }

        public IEnumerable<Instruction> Visit(StringLiteral s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(NullExpr n)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Declarator d)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(TupleAccess d)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(MemberAccess d)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Is i)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Lambda l)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ListLiteral l)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ShortCircuit s)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Ternary t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(TupleLiteral t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Unary u)
        {
            if(u.Oper is not null)
            {
                return Seq(u.Expr.Accept(this), u.Oper.GenerateByteCode());
            } else
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<Instruction> Visit(Variable v)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Block b)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ForLoop f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(IfStatement i)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ReturnStatement r)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(WhileLoop w)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(UsingStatement u)
        {
            throw new NotImplementedException();
        }
    }
}
