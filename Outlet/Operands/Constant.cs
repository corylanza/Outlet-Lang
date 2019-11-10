using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands 
{
    public abstract class Constant : Operand<Primitive>
    {
        public static Constant<int> Int(int value) =>
            new Constant<int>(value) { Type = Primitive.Int };
        public static Constant<float> Float(float value) =>
            new Constant<float>(value) { Type = Primitive.Float };
        public static Constant<string> String(string value) =>
            new Constant<string>(value) { Type = Primitive.String };
        public static Constant<bool> Bool(bool value) =>
            new Constant<bool>(value) { Type = Primitive.Bool };
        public static Constant<object> Null() =>
            new Constant<object>() { Type = Primitive.Object };

        public abstract object GetValue();
        public abstract override bool Equals(Operand b);
        public abstract override string ToString();
    }


    public class Constant<E> : Constant
    {
        public E Value;

        public Constant(E value)
        {
            Value = value;
        }

        public Constant()
        {

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
