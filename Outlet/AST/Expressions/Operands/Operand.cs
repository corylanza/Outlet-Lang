using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public abstract class Operand : Expression {

		public dynamic Value;
		public Type Type;

		//public Operand(int line = 0, int pos = 0) : base(line, pos) { }
		
		public override bool Equals(object obj) => obj is Operand o && Equals(o);
		public override int GetHashCode() => base.GetHashCode();

		public override T Accept<T>(IVisitor<T> visitor) {
			throw new Exception("operands are not visitable");
		}
		
		public abstract bool Equals(Operand b);
		public abstract override string ToString();
	}

	public interface ICallable {
		Operand Call(params Operand[] args);
	}
	public interface IDereferenceable {
		Operand Dereference(string field);
	}
	public interface ICollection {
		Operand[] Values();
	}
	/*
	public interface IAssignable {
		void Assign(Scope s, Operand value);
	}*/
}
