using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class State<Tin, Tout> {

        public bool Accepting;
        public Tout Output;
        private State<Tin, Tout> DefaultState;
        Dictionary<Tin, State<Tin, Tout>> Transitions = new Dictionary<Tin, State<Tin, Tout>>();

        public State(State<Tin, Tout> def, bool accepting, Tout output = default(Tout)) {
            SetDefualt(def);
            Accepting = accepting;
            Output = output;
        }

        public State<Tin, Tout> Transition(Tin c) {
            if(Transitions.ContainsKey(c)) return Transitions[c];
            return DefaultState;
        }

        public void SetDefualt(State<Tin, Tout> s) {
            DefaultState = s;
        }

        public void SetTransition(Tin c, State<Tin, Tout> s) {
            Transitions.Add(c, s);
        }

    }
}
