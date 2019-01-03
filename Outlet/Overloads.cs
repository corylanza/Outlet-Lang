using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet {
	public class Overload<T> where T : IOverloadable {

		private List<T> Overloads = new List<T>(); 

		public Overload(params T[] t) {
			Overloads = t.ToList();
		}

		public void Add(T t) => Overloads.Add(t);

		//https://en.cppreference.com/w/cpp/language/overload_resolution

		// all functions that use the same name and 
		public List<T> Candidates() => Overloads;
		// checks valid arg num and that each arg has a valid conversion
		public List<T> Viable(params Type[] inputs) => Overloads.Where(x => x.Valid(inputs)).ToList();
		// finds closest match
		public T Best(params Type[] inputs) {
			var viable = Viable(inputs);
			if(viable.Count == 1) return viable[0];
			return viable.MinElement(x => x.Level(inputs));
		}
	}

	public interface IOverloadable {
		bool Valid(params Type[] inputs);
		int Level(params Type[] inputs);
	}
}
