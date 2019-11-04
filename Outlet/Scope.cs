﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Outlet.Checking;
using Type = Outlet.Operands.Type;

namespace Outlet {
	public class Scope {

		public static Scope Global = new Scope();

		private readonly Dictionary<string, Type> DefinedTypes = new Dictionary<string, Type>();
		private readonly Dictionary<string, (Type type, bool defined)> Defined = new Dictionary<string, (Type, bool)>();
		private readonly Dictionary<string, (Type Type, Operand Value)> Variables = new Dictionary<string, (Type, Operand)>();

		public readonly Scope Parent;

		private Scope() {
			Parent = null;
			foreach(string s in ForeignFunctions.NativeFunctions.Keys) {
				Function f = ForeignFunctions.NativeFunctions[s];
				Define(f.Type, s);
				Add(s, f.Type, f);
			}
			foreach(string s in ForeignFunctions.NativeTypes.Keys) {
				Type t = ForeignFunctions.NativeTypes[s];
				DefineType(t, s);
				Add(s, Primitive.MetaType, t);
			}
		}

		public Scope(Scope parent) {
			Parent = parent;
		}

		public void Declare(Type t, string s) {
			if(Defined.ContainsKey(s)) Checker.Error("variable " + s + " already declared in this scope");
			else Defined.Add(s, (t, false));
		}

		public void Define(Type t, string s) {
            if (Defined.ContainsKey(s) && Defined[s].defined)
            {
                if(Defined[s].type is FunctionType existingFunc && t is FunctionType newFunc)
                {
                    Defined[s] = (new MethodGroupType(existingFunc, newFunc), true);
                }
                else if(Defined[s].type is MethodGroupType existing && t is FunctionType added)
                {
                    existing.Methods.Add(added);
                }
                else Checker.Error("variable " + s + " already defined in this scope");
            }
            else Defined[s] = (t, true);
		}

		public void DefineType(Type t, string name) {
			if(Defined.ContainsKey(name) && Defined[name].defined) Checker.Error("type " + name + " already defined in this scope");
			else {
				DefinedTypes[name] = t;
				Defined[name] = (Primitive.MetaType, true);
			}
		}

		public (Type, int) Find(string s) {
			if(Defined.ContainsKey(s) && Defined[s].defined) {
				return (Defined[s].type, 0);
			} else if(Parent != null) {
				(Type t, int r) = Parent.Find(s);
				if(r == -1) return (t, r);
				else return (t, 1 + r);
			} else return (null, -1);
		}

		public Type FindType(int level, string s) {
			if(level == 0) return DefinedTypes[s];
            if (Parent is null) return Checker.Error("Something went wrong finding type " + s);
            else return Parent.FindType(level - 1, s);
		}

		public Operand Get(int level, string s) {
			if(level == 0 && !Variables.ContainsKey(s)) throw new RuntimeException("failed to get " + s + " THIS SHOULD NOT PRINT");
			if(level == 0) return Variables[s].Value;
			else return Parent.Get(level - 1, s);
		}

        public IEnumerable<(string, Operand)> List() {
            foreach(string variableName in Variables.Keys) {
                yield return (variableName, Variables[variableName].Value);
            }
        }

		public void Add(string id, Type t, Operand v) {
            if(Variables.ContainsKey(id))
            {
                if (Variables[id].Value is Function existingFunc && v is Function newFunc)
                {
                    var newGroup = new MethodGroup(existingFunc, newFunc);
                    Variables[id] = (newGroup.Type, newGroup);
                }
                else if (Variables[id].Value is MethodGroup existing && v is Function toAdd)
                {
                    existing.AddMethod(toAdd);
                }
                else throw new OutletException("Variable already exists and cannot be added again, THIS SHOULD NOT PRINT");
            } else Variables[id] = (t, v);
		}

		public void Assign(int level, string id, Operand v) {
			if(level == 0) {
				Type t = Variables[id].Type;
				//if(v.Type.Is(t)) 
				Variables[id] = (t, v);
				//else throw new RuntimeException("cannot convert type " + v.Type.ToString() + " to type " + t.ToString());
			} else Parent.Assign(level - 1, id, v);
		}
	}
}
