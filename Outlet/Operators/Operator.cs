using Outlet.AST;
using BinOp = Outlet.Operators.BinaryOperation;
using UnOp = Outlet.Operators.UnaryOperation;

namespace Outlet.Operators
{
    public enum Side { Left, Right }

    public abstract record Operator(string Character, int Precedence, Side Association) : IOperatorPrecedenceParsable
    {
        public string Name => Character;

        public override string ToString() => Character;
    }

    public abstract record BinaryOperator(string Character, int Precedence, Side Association, params BinOp[] Operations)
        : Operator(Character, Precedence, Association)
    {
        public virtual Expression GenerateAstNode(Expression left, Expression right) =>
            new Binary(Character, left, right, new Overload<BinOp>(Operations));

        protected static Expression Assign(Expression l, Expression r) => l switch
        {
            Variable v => new LocalAssign(v, r),
            MemberAccess m => new MemberAssign(m, r),
            ArrayAccess a => new ArrayAssign(a, r),
            _ => throw new OutletException($"Can only assign to variables, fields, and array idxs not {l}")
        };
    }

    public abstract record UnaryOperator(string Character, int Precedence, Side Association, params UnOp[] Operations)
        : Operator(Character, Precedence, Association)
    {
        public virtual Expression GenerateAstNode(Expression input) =>
            new Unary(Character, input, new Overload<UnOp>(Operations));
    }

    public class Delimeter : IOperatorPrecedenceParsable
    {
        public static readonly Delimeter FuncParen = new("(");
        public static readonly Delimeter LeftParen = new("(");
        public static readonly Delimeter IndexBrace = new("[");
        public static readonly Delimeter LeftBrace = new("[");

        public string Name { get; private set; }

        private Delimeter(string name)
        {
            Name = name;
        }
    }

    public interface IOperatorPrecedenceParsable
    {
        string Name { get; }
    }
}
