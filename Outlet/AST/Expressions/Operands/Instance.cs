using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.AST {
	public class Instance : Operand, IDereferenceable {
		
		private Dictionary<string, Operand> Fields = new Dictionary<string, Operand>();

		public Instance(Class type, List<(Identifier, Operand)> fields) {
			Type = type;
			foreach((Identifier id, Operand o) in fields) {
				Fields.Add(id.Name, o);
			}
		}

		public Operand Dereference(Identifier field) {
			if (Fields.ContainsKey(field.Name)) return Fields[field.Name];
			throw new OutletException("field " + field.Name + " not found");
		}

		public override bool Equals(Operand b) {
			throw new NotImplementedException();
		}

		public override Operand Eval(Scope block) => this;

		public override string ToString() {
			string s = Type.Name+"("; 
			foreach(string k in Fields.Keys) {
				s += k + ": " + Fields[k].ToString()+", ";
			}
			return s+")";
		}
	}
}
