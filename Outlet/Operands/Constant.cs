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
        public static Constant<object> Null() =>
            new Constant<object>(Primitive.Object);

        public abstract object GetValue();
        public abstract override bool Equals(Operand b);
        public abstract override string ToString();
    }


    public class Constant<E> : Constant
    {
        public E Value;

        public Constant(Primitive type, E value)
        {
            RuntimeType = type;
            Value = value;
        }

        public Constant(Primitive type)
        {
            RuntimeType = type;
        }

        public override object GetValue() => Value;
        
        public override bool Equals(Operand b)
        {
            return b is Constant<E> other && Value.Equals(other.Value);
        }

        public override string ToString()
        {
            return Value is null ? "null" : Value.ToString();
        }
    }
}
