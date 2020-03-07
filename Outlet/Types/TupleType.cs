namespace Outlet.Types {
	public class TupleType : Type {

		public readonly Type[] Types;

		public TupleType(params Type[] types) {
			Types = types;
		}

		public override bool Is(Type t, out int level) {
            level = 0;
            if (t is TupleType tt && tt.Types.Length == Types.Length)
            {
                for (int i = 0; i < Types.Length; i++)
                {
                    if (Types[i].Is(tt.Types[i], out int elementLevel))
                    {
                        level += elementLevel;
                    }
                    else return false;
                }
                return true;
            }
            if (t == Primitive.Object) 
            {
                level = int.MaxValue;
                return true;
            }
            level = -1;
            return false;
        }

		//public override bool Equals(Operand b) {
		//	if(b is TupleType t && t.Types.Length == Types.Length) {
		//		for(int i = 0; i < Types.Length; i++) {
		//			if (!Types[i].Equals(t.Types[i])) return false;
		//		} return true;
		//	} return false;
		//}

		public override string ToString() {
			string s = "(";
			foreach (Type t in Types) {
				s += t.ToString() + ", ";
			}
			if(s == "(") return "()";
			return s.Substring(0, s.Length - 2) + ")";
		}
	}
}
