using System;
using System.Linq;
using Int = Outlet.Operands.Value<int>;
using Bln = Outlet.Operands.Value<bool>;
using Flt = Outlet.Operands.Value<float>;
using Str = Outlet.Operands.String;
using Obj = Outlet.Operands.Operand;
using Typ = Outlet.Operands.TypeObject;
using BinOp = Outlet.Operators.BinaryOperation;
using UnOp = Outlet.Operators.UnaryOperation;
using Outlet.Operands;
using Outlet.AST;
using Outlet.Compiling.Instructions;
using Outlet.Types;

namespace Outlet.Operators
{
    public record PostInc() : UnaryOperator("++", 1, Side.Left);
    public record PostDec() : UnaryOperator("--", 1, Side.Left);

    public record DotOp() : BinaryOperator(".", 1, Side.Left)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => right switch
        {
            Literal<int> idx => new TupleAccess(left, idx.Value),
            Variable member => new MemberAccess(left, member),
            _ => throw new UnexpectedException("Only variable or tuple access allowed here")
        };
    }

    //public record ExcRange() : BinaryOperator("..",   1,  Side.Right,
    //new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, false)));
    //public record IncRange() : BinaryOperator("...",  1,  Side.Right,   new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, true)));

    public record PreInc() : UnaryOperator("++", 1, Side.Left);
    public record PreDec() : UnaryOperator("--", 1, Side.Left);
    public record UnaryPlus() : UnaryOperator("+", 2, Side.Right);

    public record Complement() : UnaryOperator("~", 2, Side.Right,
        new UnOp<Int, Int>((l) => Value.Int(~l.Underlying)));

    public record UnaryAnd() : UnaryOperator("&", 2, Side.Right,
        new UnOp<Typ, Typ>((l) => new Typ(l.GetOutletType())),
        new UnOp<Obj, Typ>((l) => new Typ(l.GetOutletType())));

    public record DefineLookup() : UnaryOperator("#", 2, Side.Right,
        new UnOp<Typ, Str>((l) => new Str(l.Encapsulated.ToString())));

    public record Negative() : UnaryOperator("-", 2, Side.Right,
        new UnOp<Int, Int>((l) => Value.Int(-l.Underlying), bytecode: () => new NegateInt()),
        new UnOp<Flt, Flt>((l) => Value.Float(-l.Underlying)),
        new UnOp<Str, Str>((l) => new Str(string.Concat(l.Underlying.Reverse()))));

    public record Not() : UnaryOperator("!", 2, Side.Right,
        new UnOp<Bln, Bln>((l) => Value.Bool(!l.Underlying)));

    public record Times() : BinaryOperator("*", 3, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying * r.Underlying)),
        new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying * r.Underlying)));

    public record Divide() : BinaryOperator("/", 3, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0 ? throw new RuntimeException("Divide By 0") : Value.Int(l.Underlying / r.Underlying)),
        new BinOp<Flt, Flt, Flt>((l, r) => r.Underlying == 0 ? throw new RuntimeException("Divide By 0") : Value.Float(l.Underlying / r.Underlying)),
        new BinOp<Typ, Typ, Typ>((l, r) => new Typ(new UnionType(l.Encapsulated, r.Encapsulated))));

    public record Modulus() : BinaryOperator("%", 3, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0 ? throw new RuntimeException("Divide By 0") : Value.Int(l.Underlying % r.Underlying)));

    public record Plus() : BinaryOperator("+", 4, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying + r.Underlying), bytecode: () => new BinaryAdd()),
        new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying + r.Underlying)),
        new BinOp<Str, Obj, Str>((l, r) => new Str(l.Underlying + r.ToString())),
        new BinOp<Obj, Str, Str>((l, r) => new Str(l.ToString() + r.Underlying)));

    public record Minus() : BinaryOperator("-", 4, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying - r.Underlying), bytecode: () => new BinarySub()),
        new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying - r.Underlying)));

    public record LShift() : BinaryOperator("<<", 5, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying << r.Underlying)));

    public record RShift() : BinaryOperator(">>", 5, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying >> r.Underlying)));

    public record LT() : BinaryOperator("<", 6, Side.Left,
        new BinOp<Int, Int, Bln>((l, r) => Value.Bool(l.Underlying < r.Underlying)),
        new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying < r.Underlying)));

    public record LTE() : BinaryOperator("<=", 6, Side.Left,
        new BinOp<Int, Int, Bln>((l, r) => Value.Bool(l.Underlying <= r.Underlying)),
        new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying <= r.Underlying)));

    public record GT() : BinaryOperator(">", 6, Side.Left,
        new BinOp<Int, Int, Bln>((l, r) => Value.Bool(l.Underlying > r.Underlying)),
        new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying > r.Underlying)));

    public record GTE() : BinaryOperator(">=", 6, Side.Left,
        new BinOp<Int, Int, Bln>((l, r) => Value.Bool(l.Underlying >= r.Underlying)),
        new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying >= r.Underlying)));

    public record AsOp() : BinaryOperator("as", 6, Side.Left)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => new As(left, right);
    }

    public record IsOp() : BinaryOperator("is", 6, Side.Left,
        new BinOp<Obj, Typ, Bln>((l, r) => Value.Bool(l.GetOutletType().Is(r.Encapsulated))),
        new BinOp<Typ, Typ, Bln>((l, r) => Value.Bool(l.GetOutletType().Is(r.Encapsulated))));

    public record IsntOp() : BinaryOperator("isnt", 6, Side.Left,
        new BinOp<Obj, Typ, Bln>((l, r) => Value.Bool(!l.GetOutletType().Is(r.Encapsulated))),
        new BinOp<Typ, Typ, Bln>((l, r) => Value.Bool(!l.GetOutletType().Is(r.Encapsulated))));

    public record BoolEquals() : BinaryOperator("==", 7, Side.Left,
        new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(l.Equals(r))));

    public record NotEqual() : BinaryOperator("!=", 7, Side.Left,
        new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(!l.Equals(r))));

    public record BitAnd() : BinaryOperator("&", 8, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying & r.Underlying)));

    public record BitXor() : BinaryOperator("^", 9, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying ^ r.Underlying)));

    public record BitOr() : BinaryOperator("|", 10, Side.Left,
        new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying | r.Underlying)));

    public record LogicalAndOp() : BinaryOperator("&&", 11, Side.Left)
    {
        // Needs separate AST Node due to short circuiting behavior
        public override Expression GenerateAstNode(Expression left, Expression right) => new LogicalAnd(left, right);
    }

    public record LogicalOrOp() : BinaryOperator("||", 12, Side.Left)
    {
        // Needs separate AST Node due to short circuiting behavior
        public override Expression GenerateAstNode(Expression left, Expression right) => new LogicalOr(left, right);
    }

    public record TernaryQuestion() : BinaryOperator("?", 13, Side.Right);
    public record TernaryElse() : BinaryOperator(":", 13, Side.Right);

    public record Equal() : BinaryOperator("=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, right);
    }

    public record PlusEqual() : BinaryOperator("+=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, new Plus().GenerateAstNode(left, right));
    }

    public record MinusEqual() : BinaryOperator("-=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, new Minus().GenerateAstNode(left, right));
    }

    public record DivEqual() : BinaryOperator("/=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, new Divide().GenerateAstNode(left, right));
    }

    public record MultEqual() : BinaryOperator("*=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, new Times().GenerateAstNode(left, right));
    }

    public record ModEqual() : BinaryOperator("%=", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => Assign(left, new Modulus().GenerateAstNode(left, right));
    }

    public record LambdaOp() : BinaryOperator("=>", 14, Side.Right)
    {
        public override Expression GenerateAstNode(Expression left, Expression right) => left switch {
        ///    ParamListWrapper wrapper => throw new NotImplementedException(),//wrapper.Wrapped ,
            _ => new Lambda(left, right)
        };
    }
}
