using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Instance : Operand, IDereferenceable {
		
		private readonly Dictionary<string, Operand> Fields = new Dictionary<string, Operand>();
		private readonly Scope Scope;

		public Instance(Class type, Scope closure, List<(Identifier, Operand)> fields) {
			Type = type;
			Scope = closure;
			foreach((Identifier id, Operand o) in fields) {
				Scope.Add(id.Name, o);
				//Fields.Add(id.Name, o);
			}
			Fields.Add("toString", new Native((args) => new Literal(ToString())));
		}

		public Operand Dereference(Identifier field) {
			if (Fields.ContainsKey(field.Name)) return Fields[field.Name];
			return Scope.Get(0, field.Name);
			//return (Type as Class).Get(field);
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override Operand Eval(Scope scope) => this;

		public override string ToString() {
			string s = Type.Name+"("; 
			foreach(string k in Fields.Keys) {
				if(!(Fields[k] is Function)) s += k + ": " + Fields[k].ToString()+", ";
			}
			return s+")";
		}
	}
}
