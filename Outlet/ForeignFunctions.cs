using System;
using System.Collections.Generic;
using System.Linq;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet {
	public static class ForeignFunctions {

	public static FunctionType MakeType(params Type[] t) {
			if (t.Length == 0) throw new Exception("Foreign Function type invalid");
			return new FunctionType(t.Take(t.Length - 1).Select(x => (x, "")).ToArray(), t.Last());
		}

        public static Dictionary<string, Function> NativeFunctions = new Dictionary<string, Function>(); /*{
			{"print",       new Native(MakeType(Object, Void), (Operand[] o) => {
									foreach(Operand op in o){
										Console.WriteLine(op.ToString());
									} return null; }) },
			{"readline",    new Native(MakeType(String), (Operand[] o) => new Constant(Console.ReadLine())) },
			{"max",         new Native(MakeType(Int, Int, Int), (Operand[] o) => new Constant(o.Max(x => x.Value))) },
			{"gettype",     new Native(MakeType(Object, String), (Operand[] o) => new Constant(o[0].Type.ToString())) },
			{"parseint",    new Native(MakeType(String, Int), (Operand[] o) => new Constant(int.Parse(o[0].Value)))},
			{"map",			new Native(MakeType(new ArrayType(Int), MakeType(Int, Int), new ArrayType(Int)),
										(Operand[] o) => {
											Native ft = (Native) o[1];
											Operands.Array arr = (Operands.Array) o[0];
											return new Operands.Array(arr.Values().Select(x => ft.Call(x)).ToArray());
										})
			}
		};*/

	public static Dictionary<string, Type> NativeTypes = new Dictionary<string, Type>() {
			{"int", Primitive.Int },
			{"float", Primitive.Float },
			{"bool", Primitive.Bool },
			{"string", Primitive.String },
			{"object", Primitive.Object },
			{"type", Primitive.MetaType },
			{"void", Primitive.Void },
			/*{"math", new NativeClass("math",
				("pi", new Constant((float) Math.PI)),
				("sin", new Native(MakeType(Float, Float),
					(Operand[] o) => new Constant((float) Math.Sin(o[0].Value)))),
				("pow", new Native(MakeType(Int, Int, Int),
					(Operand[] o) => new Constant((int) Math.Pow(o[0].Value, o[1].Value))))
			)},*/
			/*{"file", new NativeClass("file"
				//("open", new Native(MakeType(String)))
			)}*/
		};

		
	}
}
