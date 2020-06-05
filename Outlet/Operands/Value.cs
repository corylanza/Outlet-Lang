using Outlet.Types;

namespace Outlet.Operands 
{
    public abstract class Value : Operand<Primitive>
    {

        public static Value<int> Int(int value) =>
            new Value<int>(Primitive.Int, value);
        public static Value<float> Float(float value) =>
            new Value<float>(Primitive.Float, value);
        //public static Constant<string> String(string value) =>
        //    new Constant<string>(Primitive.String, value);
        public static Value<bool> Bool(bool value) =>
            new Value<bool>(Primitive.Bool, value);
        public static NullClass Null => NullClass.Self;
        //public static Constant<object> Null() =>
        //    new Constant<object>(Primitive.Object);
        public abstract override bool Equals(Operand b);
        public abstract override string ToString();

        public class NullClass : Value
        {
            public override Primitive RuntimeType { get; }
            public static NullClass Self = new NullClass();

            private NullClass()
            {
                RuntimeType = Primitive.Object;
            }

            public override bool Equals(Operand b) => b is NullClass;

            public override string ToString() => "null";
        }
    }


    public class Value<E> : Value where E : struct
    {
        public readonly E Underlying;

        public override Primitive RuntimeType { get; }

        public Value(Primitive type, E value)
        {
            RuntimeType = type;
            Underlying = value;
        }
        
        public override bool Equals(Operand b)
        {
            return b is Value<E> other && (Underlying.Equals(other.Underlying));
        }

        public override string ToString() => Underlying.ToString() ?? "";
    }
}
