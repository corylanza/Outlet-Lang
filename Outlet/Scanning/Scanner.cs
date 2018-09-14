using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Scanning {
	public static partial class Scanner {

		//private static Stack<State> states = new Stack<State>();
		//State cur { get { return states.Peek(); } }
		private static State cur;

		static Scanner() {
			cur = start;
			//states.Push(SBeg);
		}
		
		
		public static void Scan(string text) {
			foreach(char c in  text) {
				cur = cur.Transition(c);
			}
		}

	}
}
