using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Scanning {
	public static partial class Scanner {

		static State start = new State(); // starting / default state
		static State id = new State();
		static State number = new State();
		static State comment = new State(comment);
		static State error = new State();

		static void InitStates() {
			start.SetTransition(CharType.Letter, id);
			start.SetTransition(CharType.Number, number);
			id.SetTransition(CharType.Number, id);
		}

		private class State {

			State DefaultState;
			Dictionary<CharType, State> Transitions = new Dictionary<CharType, State>();

			internal State() {
				SetDefualt(error);
			}

			internal State(State def) {
				SetDefualt(def);
			}

			internal State Transition(CharType c) {
				if (Transitions.ContainsKey(c)) return Transitions[c];
				return DefaultState;
			}

			internal void SetDefualt(State s) {
				DefaultState = s;
			}

			internal void SetTransition(CharType c, State s) {
				Transitions.Add(c, s);
			}

		}
	}
}
