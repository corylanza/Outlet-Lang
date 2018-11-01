using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class State<Tin, Tout> {

        public bool Accepting;
        public bool Keep;
        public Tout Output;
        private State<Tin, Tout> DefaultState;
        Dictionary<Tin, State<Tin, Tout>> Transitions = new Dictionary<Tin, State<Tin, Tout>>();

        public State(bool accepting, bool keep, Tout output = default(Tout)) {
            Accepting = accepting;
            Keep = keep;
            Output = output;
        }

        public bool CanTransition(Tin c) => Transitions.ContainsKey(c) | !(DefaultState is null);

        public State<Tin, Tout> Transition(Tin c) {
			if (Transitions.ContainsKey(c)) return Transitions[c];
			else if (DefaultState != null) return DefaultState;
            throw new Exception("illegal state change");
        }

        public void SetDefualt(State<Tin, Tout> s) {
            DefaultState = s;
        }

        public void SetTransition(Tin c, State<Tin, Tout> s) {
            Transitions.Add(c, s);
        }

    }
}
