using Outlet.AST;
using Outlet.Operands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Int = Outlet.Operands.Value<int>;
using Bln = Outlet.Operands.Value<bool>;
using Flt = Outlet.Operands.Value<float>;
using Str = Outlet.Operands.String;
using Obj = Outlet.Operands.Operand;
using Typ = Outlet.Operands.TypeObject;
using BinOp = Outlet.Operators.BinaryOperation;
using UnOp = Outlet.Operators.UnaryOperation;
using Outlet.Types;

namespace Outlet.Operators
{
    public enum Side { Left, Right }

    public abstract class Operator : IOperatorPrecedenceParsable
    {
        public string Name { get; private set; }
        public readonly int Precedence;
        public readonly Side Assoc;

        protected Operator(string name, int precedence, Side associativity) =>
            (Name, Precedence, Assoc) = (name, precedence, associativity);

        public override string ToString() => Name;

        private static Operands.Array Range(int start, int end, bool inc)
        {
            int i = inc ? 1 : 0;
            return new Operands.Array(Enumerable.Range(start, end - start + i).Select((x) => Value.Int(x)).ToArray());
        }

        public static readonly UnaryOperator PostInc, PostDec, PreInc, PreDec, UnaryPlus, Complement, UnaryAnd, Negative, Not;
        public static readonly BinaryOperator Dot, Times, Divide, Modulus, Plus, Minus, LShift, RShift,
                                              LT, LTE, GT, GTE, Is, As, Isnt, BoolEquals, NotEqual, BitAnd,
                                              BitXor, BitOr, LogicalAnd, LogicalOr, Question, Ternary,
                                              Lambda, Equal, PlusEqual, MinusEqual, DivEqual, MultEqual,
                                              /*IncRange, ExcRange,*/ ModEqual;

        static Operator()
        {
            PostInc = new UnaryOperator("++", 1, Side.Left);
            PostDec = new UnaryOperator("--", 1, Side.Left);

            Dot = new BinaryOperator(".", 1, Side.Left, 
                astGenerator: (l, r) => r switch {
                    Literal<int> idx => new TupleAccess(l, idx.Value),
                    Variable member => new MemberAccess(l, member),
                    _ => throw new UnexpectedException("Only variable or tuple access allowed here")
                }
            );

            //ExcRange =		new BinaryOperator("..",   1,  Side.Right,
            //new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, false)));
            //IncRange =		new BinaryOperator("...",  1,  Side.Right,   new BinOp<Int, Int, Operands.Array>((l, r) => Range(l.Val, r.Val, true)));
            PreInc = new UnaryOperator("++", 1, Side.Left);
            PreDec = new UnaryOperator("--", 1, Side.Left);
            UnaryPlus = new UnaryOperator("+", 2, Side.Right);

            Complement = new UnaryOperator("~", 2, Side.Right,
                new UnOp<Int, Int>((l) => Value.Int(~l.Underlying)));

            UnaryAnd = new UnaryOperator("&", 2, Side.Right,
                new UnOp<Obj, Typ>((l) => new Typ(l.GetOutletType())));

            Negative = new UnaryOperator("-", 2, Side.Right,
                new UnOp<Int, Int>((l) => Value.Int(-l.Underlying)),
                new UnOp<Flt, Flt>((l) => Value.Float(-l.Underlying)),
                new UnOp<Str, Str>((l) => new Str(string.Concat(l.Underlying.Reverse()))));

            Not = new UnaryOperator("!", 2, Side.Right,
                new UnOp<Bln, Bln>((l) => Value.Bool(!l.Underlying)));

            Times = new BinaryOperator("*", 3, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying * r.Underlying)),
                new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying * r.Underlying)));

            Divide = new BinaryOperator("/", 3, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0
                    ? throw new RuntimeException("Divide By 0")
                    : Value.Int(l.Underlying / r.Underlying)),
                new BinOp<Flt, Flt, Flt>((l, r) => r.Underlying == 0
                    ? throw new RuntimeException("Divide By 0")
                    : Value.Float(l.Underlying / r.Underlying)),
                new BinOp<Typ, Typ, Typ>((l, r) => new Typ(new UnionType(l.Encapsulated, r.Encapsulated))));

            Modulus = new BinaryOperator("%", 3, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => r.Underlying == 0
                    ? throw new RuntimeException("Divide By 0")
                    : Value.Int(l.Underlying % r.Underlying)));

            Plus = new BinaryOperator("+", 4, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying + r.Underlying)),
                new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying + r.Underlying)),
                new BinOp<Str, Obj, Str>((l, r) => new Str(l.Underlying + r.ToString())),
                new BinOp<Obj, Str, Str>((l, r) => new Str(l.ToString() + r.Underlying)));

            Minus = new BinaryOperator("-", 4, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying - r.Underlying)),
                new BinOp<Flt, Flt, Flt>((l, r) => Value.Float(l.Underlying - r.Underlying)));

            LShift = new BinaryOperator("<<", 5, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying << r.Underlying)));
            
            RShift = new BinaryOperator(">>", 5, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying >> r.Underlying)));
            
            LT = new BinaryOperator("<", 6, Side.Left,
                new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying < r.Underlying)));
            
            LTE = new BinaryOperator("<=", 6, Side.Left,
                new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying <= r.Underlying)));
            
            GT = new BinaryOperator(">", 6, Side.Left,
                new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying > r.Underlying)));
            
            GTE = new BinaryOperator(">=", 6, Side.Left,
                new BinOp<Flt, Flt, Bln>((l, r) => Value.Bool(l.Underlying >= r.Underlying)));
            
            As = new BinaryOperator("as", 6, Side.Left, astGenerator: (l, r) => new As(l, r));
            Is = new BinaryOperator("is", 6, Side.Left, astGenerator: (l, r) => new Is(l, r, yes: true));
            Isnt = new BinaryOperator("isnt", 6, Side.Left, astGenerator: (l, r) => new Is(l, r, yes: false));

            BoolEquals = new BinaryOperator("==", 7, Side.Left,
                new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(l.Equals(r))));

            NotEqual = new BinaryOperator("!=", 7, Side.Left,
                new BinOp<Obj, Obj, Bln>((l, r) => Value.Bool(!l.Equals(r))));

            BitAnd = new BinaryOperator("&", 8, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying & r.Underlying)));

            BitXor = new BinaryOperator("^", 9, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying ^ r.Underlying)));

            BitOr = new BinaryOperator("|", 10, Side.Left,
                new BinOp<Int, Int, Int>((l, r) => Value.Int(l.Underlying | r.Underlying)));

            LogicalAnd = new BinaryOperator("&&", 11, Side.Left, astGenerator: (l, r) => new LogicalAnd(l, r));
            LogicalOr = new BinaryOperator("||", 12, Side.Left, astGenerator: (l, r) => new LogicalOr(l, r));
            Question = new BinaryOperator("?", 13, Side.Right);
            Ternary = new BinaryOperator(":", 13, Side.Right);
            Equal = new BinaryOperator("=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, r));
            PlusEqual = new BinaryOperator("+=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, Plus.GenerateASTNode(l, r)));
            MinusEqual = new BinaryOperator("-=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, Minus.GenerateASTNode(l, r)));
            DivEqual = new BinaryOperator("/=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, Divide.GenerateASTNode(l, r)));
            MultEqual = new BinaryOperator("*=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, Times.GenerateASTNode(l, r)));
            ModEqual = new BinaryOperator("%=", 14, Side.Right, astGenerator: (l, r) => new Assign(l, Modulus.GenerateASTNode(l, r)));
            Lambda = new BinaryOperator("=>", 14, Side.Right, astGenerator: (l, r) => new Lambda(l, r));
        }
    }

    public delegate Expression BinaryOpASTGenerator(Expression left, Expression right);

    public class BinaryOperator : Operator
    {
        public readonly Overload<BinOp> Overloads;
        private readonly BinaryOpASTGenerator ASTGenerator;

        public BinaryOperator(string name, int p, Side a, BinaryOpASTGenerator astGenerator, params BinOp[] defaultoverloads) : base(name, p, a)
        {
            ASTGenerator = astGenerator;
            Overloads = new Overload<BinOp>(defaultoverloads);
        }

        public BinaryOperator(string name, int p, Side a, params BinOp[] defaultoverloads) : base(name, p, a)
        {
            ASTGenerator = DefaultAstGenerator;
            Overloads = new Overload<BinOp>(defaultoverloads);
        }

        private Expression DefaultAstGenerator(Expression left, Expression right) => new Binary(Name, left, right, Overloads);


        // Right param is first due to shunting yard popping the right operand first
        public Expression GenerateASTNode(Expression right, Expression left) => ASTGenerator(left, right);
    }

    public class UnaryOperator : Operator
    {
        public readonly Overload<UnOp> Overloads;

        public UnaryOperator(string name, int p, Side a, params UnOp[] overloads) : base(name, p, a)
        {
            Overloads = new Overload<UnOp>(overloads);
        }

        public Expression GenerateASTNode(Expression input) => new Unary(Name, input, Overloads);
    }

    public class Delimeter : IOperatorPrecedenceParsable
    {
        public static readonly Delimeter FuncParen = new Delimeter("(");
        public static readonly Delimeter LeftParen = new Delimeter("(");
        public static readonly Delimeter IndexBrace = new Delimeter("[");
        public static readonly Delimeter LeftBrace = new Delimeter("[");

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
