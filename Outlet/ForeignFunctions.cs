using System;
using System.Collections.Generic;
using System.Linq;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet {
	public static class ForeignFunctions {

	    public static Dictionary<string, Type> NativeTypes = new Dictionary<string, Type>() {
			{"int", Primitive.Int },
			{"float", Primitive.Float },
			{"bool", Primitive.Bool },
			{"string", Primitive.String },
			{"object", Primitive.Object },
			{"type", Primitive.MetaType },
			{"void", Primitive.Void },
        };
	}
}
