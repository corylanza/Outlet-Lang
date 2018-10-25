using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Instance : Operand, IDereferenceable {
		
		private readonly Dictionary<string, Operand> Fields = new Dictionary<string, Operand>();
		private readonly Scope Scope;

		public Instance(Class type, Scope closure) {
			Type = type;
			Scope = closure;
			/*
			foreach((Identifier id, Operand o) in fields) {
				Scope.Add(id.Name, o);
				//Fields.Add(id.Name, o);
			}
			Fields.Add("toString", new Native((args) => new Literal(ToString())));
			*/
		}

		public Operand Dereference(Identifier field) {
			return Fields[field.Name];
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override string ToString() {
			string s = Type.Name+" instance";
			return s;
		}
	}
}
