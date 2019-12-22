using System.Collections.Generic;
using Outlet.Operands;
using Outlet.Types;
using Type = Outlet.Types.Type;

namespace Outlet 
{
	public class Scope 
    {

		public static Scope Global = new Scope();

		private readonly Dictionary<string, (ITyped Type, Operand Value)> Variables = new Dictionary<string, (ITyped, Operand)>();

		public readonly Scope Parent;

		private Scope() 
        {
			Parent = null;
			foreach(string s in ForeignFunctions.NativeFunctions.Keys) 
            {
				Function f = ForeignFunctions.NativeFunctions[s];
				Add(s, f.GetOutletType(), f);
			}
			foreach(string s in ForeignFunctions.NativeTypes.Keys) 
            {
				Type t = ForeignFunctions.NativeTypes[s];
				Add(s, Primitive.MetaType, new TypeObject(t));
			}
		}

		public Scope(Scope parent) 
        {
			Parent = parent;
		}

		public Operand Get(int level, string s) 
        {
			if(level == 0 && !Variables.ContainsKey(s)) throw new RuntimeException("failed to get " + s + " THIS SHOULD NOT PRINT");
			if(level == 0) return Variables[s].Value;
			else return Parent.Get(level - 1, s);
		}

        public IEnumerable<(string, Operand)> List() 
        {
            foreach(string variableName in Variables.Keys) 
            {
                yield return (variableName, Variables[variableName].Value);
            }
        }

		public void Add(string id, ITyped t, Operand v) 
        {
            if(Variables.ContainsKey(id))
            {
                if (Variables[id].Value is Function existingFunc && v is Function newFunc)
                {
                    var newGroup = new MethodGroup(existingFunc, newFunc);
                    Variables[id] = (newGroup.GetOutletType(), newGroup);
                }
                else if (Variables[id].Value is MethodGroup existing && v is Function toAdd)
                {
                    existing.AddMethod(toAdd);
                }
                else throw new OutletException("Variable already exists and cannot be added again, THIS SHOULD NOT PRINT");
            } else Variables[id] = (t, v);
		}

		public void Assign(int level, string id, Operand v) 
        {
			if(level == 0) 
            {
				ITyped t = Variables[id].Type;
				//if(v.Type.Is(t)) 
				Variables[id] = (t, v);
				//else throw new RuntimeException("cannot convert type " + v.Type.ToString() + " to type " + t.ToString());
			} else Parent.Assign(level - 1, id, v);
		}
	}
}
