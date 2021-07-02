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
            //yield return new LocalGet(f.Decl.LocalId!.Value);
            var body = Gen(f.Body).Append(new Return());
            
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
            return Seq(Gen(c.Caller), c.Args.SelectMany(arg => Gen(arg)).Append(new CallFunc(c.Args.Length)));
        }

        public IEnumerable<Instruction> Visit<E>(Literal<E> c) where E : struct
        {
            yield return c switch
            {
                Literal<int> l => new ConstInt(l.Value),
                Literal<bool> l => new ConstBool(l.Value),
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
            var ifFalse = Gen(t.IfFalse);

            var ifTrue = Gen(t.IfTrue)
                // Jump over true branch if condition is false
                .Append(new JumpRelative(ifFalse.Count()));

            // true branch must include a jump over the false branch if one exists
            var condition = Gen(t.Condition)
                .Append(new JumpFalseRelative(ifTrue.Count()));

            return Seq(condition, ifTrue, ifFalse);
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
            if(i.Iffalse is null)
            {
                var ifTrue = Gen(i.Iftrue);

                var condition = Gen(i.Condition)
                    // Jump over true branch if condition is false
                    .Append(new JumpFalseRelative(ifTrue.Count()));

                return Seq(condition, ifTrue);
            } else
            {
                var ifFalse = Gen(i.Iffalse);

                var ifTrue = Gen(i.Iftrue)
                    // true branch must include a jump over the false branch if one exists
                    .Append(new JumpRelative(ifFalse.Count()));

                var condition = Gen(i.Condition)
                    // Jump over true branch if condition is false
                    .Append(new JumpFalseRelative(ifTrue.Count()));

                return Seq(condition, ifTrue, ifFalse);
            }
        }

        public IEnumerable<Instruction> Visit(ReturnStatement r)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(WhileLoop w)
        {

            //var body = Gen(w.Body);

            //var finalBody = body
            //    // Jump back to 
            //    .Append(new JumpRelative(-body.Count()));

            //var condition = Gen(w.Condition)
            //    .Append(new JumpFalseRelative(finalBody.Count()));

            //return Seq(condition, body);
            throw new NotImplementedException();
        }

        public IEnumerable<Instruction> Visit(UsingStatement u)
        {
            throw new NotImplementedException();
        }
    }
}
