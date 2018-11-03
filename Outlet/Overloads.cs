using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.AST;

namespace Outlet {
	public class Overloads {

		public Overloads(string name) {

		}

		//https://en.cppreference.com/w/cpp/language/overload_resolution

		// all functions that use the same name and 
		public List<Function> Cadidates() => null;
		// checks valid arg num and that each arg has a valid conversion
		public List<Function> Viable() => null;
		// finds closest match
		public List<Function> Best() => null;
	}
}
