using System.Collections.Generic;
using Type = Outlet.Types.Type;

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
        // TODO public abstract T RuntimeType { get; }
        public T RuntimeType;

        public override Type GetOutletType() => RuntimeType;

    }

	public delegate Operand Getter(IBindable s);
	public delegate void Setter(IBindable s, Operand val);
    public delegate IEnumerable<(string, Operand)> Lister();

	public interface ICallable {
		Operand Call(Operand? caller, params Operand[] args);
	}

	public interface IDereferenceable {
		Operand GetMember(IBindable field);
        void SetMember(IBindable field, Operand value);
        IEnumerable<(string id, Operand val)> GetMembers();
	}
}
