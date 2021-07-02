using Outlet.Compiling.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Interpreting.ByteCode
{
    public partial class VirtualMachine : IInstructionVisitor<object?>
    {
        public object? Visit(ConstInt c)
        {
            ValueStack.Push(c.Value);
            return null;
        }

        public object? Visit(ConstFloat c)
        {
            throw new NotImplementedException();
        }

        public object? Visit(ConstBool c)
        {
            ValueStack.Push(c.Value ? 1 : 0);
            return null;
        }

        public object? Visit(ConstString c)
        {
            throw new NotImplementedException();
        }

        public object? Visit(NegateInt n)
        {
            ValueStack.Push(-ValueStack.Pop());
            return null;
        }

        public object? Visit(BinaryAdd b)
        {
            ValueStack.Push(ValueStack.Pop() + ValueStack.Pop());
            return null;
        }

        public object? Visit(BinarySub b)
        {
            var right = ValueStack.Pop();
            var left = ValueStack.Pop();
            ValueStack.Push(left - right);
            return null;
        }

        public object? Visit(LocalStore l)
        {
            SetLocal(l.LocalId, ValueStack.Pop());
            return null;
        }

        public object? Visit(LocalGet l)
        {
            ValueStack.Push(GetLocal(l.LocalId));
            return null;
        }

        public object? Visit(JumpRelative j)
        {
            idx += j.JumpInterval;
            return null;
        }

        public object? Visit(JumpFalseRelative j)
        {
            if(ValueStack.Pop() == 0)
            {
                idx += j.JumpInterval;
            }
            return null;
        }

        public object? Visit(CallFunc c)
        {
            IEnumerable<int> Args(int argCount)
            {
                foreach(int i in Enumerable.Range(0, argCount))
                {
                    yield return ValueStack.Pop();
                }
            }

            var args = Args(c.ArgCount);
            var caller = ValueStack.Pop();

            //TODO fix
            StackFrames.Push(new CallFrame(100, idx));
            return null;
        }

        public object? Visit(Return r)
        {
            throw new NotImplementedException();
        }
    }
}
