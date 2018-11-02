using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public static class ForeignFunctions {
		public static Dictionary<string, Function> NativeFunctions = new Dictionary<string, Function>() {
			{"print", new Native((Operand[] o) => {
									foreach(Operand op in o){
										Console.WriteLine(op.ToString());
									} return null; }) },
			{"readline", new Native((Operand[] o) => new Constant(Console.ReadLine())) },
			{"max", new Native((Operand[] o) => new Constant(o.Max(x => x.Value))) },
			{"gettype", new Native((Operand[] o) => new Constant(o[0].Type.ToString())) },
		};

		public static Dictionary<string, AST.Type> NativeTypes = new Dictionary<string, AST.Type>() {
			{"int", Primitive.Int },
			{"float", Primitive.Float },
			{"bool", Primitive.Bool },
			{"string", Primitive.String },
			{"object", Primitive.Object },
			{"list", Primitive.List },
			{"type", Primitive.MetaType },
			{"void", Primitive.Void },
			{ "func", Primitive.FuncType },
		};
	}
}
