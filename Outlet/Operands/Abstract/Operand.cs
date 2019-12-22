using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public abstract class Operand {
        public abstract Type GetOutletType();

        public abstract bool Equals(Operand b);
		public abstract override string ToString();

        public override bool Equals(object obj) => obj is Operand o && Equals(o);
        public override int GetHashCode() => base.GetHashCode();

    }

    public abstract class Operand<T> : Operand where T : Type
    {
        public T RuntimeType;

        public override Type GetOutletType() => RuntimeType;

    }

	public delegate Operand Getter(string s);
	public delegate void Setter(string s, Operand val);
    public delegate IEnumerable<(string, Operand)> Lister();

	public interface ICallable {
		Operand Call(params Operand[] args);
	}/*
	public interface IDereferenceable {
		Operand Dereference(string field);
	}
	public interface ICollection {
		Operand[] Values();
	}*/
}
