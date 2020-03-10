using Outlet.Types;

namespace Outlet.Operands 
{
    public abstract class Constant : Operand<Primitive>
    {

        public static Constant<int> Int(int value) =>
            new Constant<int>(Primitive.Int, value);
        public static Constant<float> Float(float value) =>
            new Constant<float>(Primitive.Float, value);
        public static Constant<string> String(string value) =>
            new Constant<string>(Primitive.String, value);
        public static Constant<bool> Bool(bool value) =>
            new Constant<bool>(Primitive.Bool, value);
        public static NullClass Null => NullClass.Self;
        //public static Constant<object> Null() =>
        //    new Constant<object>(Primitive.Object);
        public abstract override bool Equals(Operand b);
        public abstract override string ToString();

        public class NullClass : Constant
        {
            public override Primitive RuntimeType { get; set; }
            public static NullClass Self = new NullClass();

            private NullClass()
            {
                RuntimeType = Primitive.Object;
            }

            public override bool Equals(Operand b) => b is NullClass;

            public override string ToString() => "null";
        }
    }


    public class Constant<E> : Constant
    {
        public E Value;

        public override Primitive RuntimeType { get; set; }

        public Constant(Primitive type, E value)
        {
            RuntimeType = type;
            Value = value;
        }
        
        public override bool Equals(Operand b)
        {
            return b is Constant<E> other && (Value?.Equals(other.Value) ?? other.Value is null);
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }
    }
}
