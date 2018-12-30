using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Tokens;
using Outlet.AST;
using Type = Outlet.AST.Type;

namespace Outlet {
	public static class ForeignFunctions {

		private static readonly Type Int = Primitive.Int;
		private static readonly Type Float = Primitive.Float;
		private static readonly Type Void = Primitive.Void;
		private static readonly Type String = Primitive.String;
		private static readonly Type Bool = Primitive.Bool;
		private static readonly Type Object = Primitive.Object;

		public static FunctionType MakeType(params Type[] t) {
			if (t.Length == 0) throw new Exception("Foreign Function type invalid");
			return new FunctionType(t.Take(t.Length - 1).Select(x => (x, "")).ToArray(), t.Last());
		}

		public static Dictionary<string, Function> NativeFunctions = new Dictionary<string, Function>() {
			{"print",		new Native(MakeType(Object, Void), (Operand[] o) => {
									foreach(Operand op in o){
										Console.WriteLine(op.ToString());
									} return null; }) },
			{"readline",	new Native(MakeType(String), (Operand[] o) => new Const(Console.ReadLine())) },
			{"max",			new Native(MakeType(Int, Int, Int), (Operand[] o) => new Const(o.Max(x => x.Value))) },
			{"gettype",		new Native(MakeType(Object, String), (Operand[] o) => new Const(o[0].Type.ToString())) },
			{"parseint",	new Native(MakeType(String, Int), (Operand[] o) => new Const(int.Parse(o[0].Value)))}
		};

		public static Dictionary<string, Type> NativeTypes = new Dictionary<string, Type>() {
			{"int", Primitive.Int },
			{"float", Primitive.Float },
			{"bool", Primitive.Bool },
			{"string", Primitive.String },
			{"object", Primitive.Object },
			//{"list", Primitive.List },
			{"type", Primitive.MetaType },
			{"void", Primitive.Void },
			//{"func", Primitive.FuncType },
		};

		
	}
}
