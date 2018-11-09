using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Overload<T> {

		private List<T> Overloads = new List<T>(); 

		public Overload(params T[] t) {
			Overloads = t.ToList();
		}

		public void Add(T t) => Overloads.Add(t);

		//https://en.cppreference.com/w/cpp/language/overload_resolution

		// all functions that use the same name and 
		public List<T> Cadidates() => Overloads;
		// checks valid arg num and that each arg has a valid conversion
		public List<T> Viable() => null;// Overloads.Where();
		// finds closest match
		public T Best() => Overloads[1];
	}
}
