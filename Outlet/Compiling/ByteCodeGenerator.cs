﻿using Outlet.AST;
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
            return Gen(program);
        }

        private IEnumerable<Instruction> Gen(IASTNode node) => node.Accept(this);

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
            // TODO handle no initializer case
            return Seq(Gen(v.Initializer), Gen(v.Decl));
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ArrayAccess a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(ArrayAssign a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(As a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(LocalAssign a)
        {
            static IEnumerable<Instruction> LocalStore(uint localId)
            {
                yield return new LocalStore(localId);
            }

            return Seq(Gen(a.Right), LocalStore(a.Variable.LocalId!.Value));
        }

        public IEnumerable<Instruction> Visit(MemberAssign a)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(Binary b)
        {
            if (b.Oper is not null)
            {
                return Seq(Gen(b.Left), Gen(b.Right), b.Oper.GenerateByteCode());
            } else
            {
                throw new NotImplementedException();
            }
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
            yield return new LocalStore(d.LocalId!.Value);
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
                return Seq(Gen(u.Expr), u.Oper.GenerateByteCode());
            } else
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<Instruction> Visit(Variable v)
        {
            yield return new LocalGet(v.LocalId!.Value);
        }

        public IEnumerable<Instruction> Visit(Block b)
        {
            return b.Lines.SelectMany(line => Gen(line));
        }

        public IEnumerable<Instruction> Visit(ForLoop f)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(IfStatement i)
        {
            //TODO Jump By Relative
            //if (i.Iffalse is not null) return Seq(Gen(i.Condition).Append(), Gen(i.Iftrue), Gen(i.Iffalse));
            //else return Seq(Gen(i.Condition), Gen(i.Iftrue));
            //throw new NotImplementedException();
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
