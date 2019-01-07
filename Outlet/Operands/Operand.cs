using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Operands {
	public abstract class Operand {

		public dynamic Value;
		public Type Type;
		
		public override bool Equals(object obj) => obj is Operand o && Equals(o);
		public override int GetHashCode() => base.GetHashCode();
		
		public abstract bool Equals(Operand b);
		public abstract override string ToString();
	}

	public delegate Operand CallFunc(params Operand[] args);
	public delegate Operand StaticFunc(string s);

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
