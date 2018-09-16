using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class StateMachine<Tin, Tout> {

        public State<Tin, Tout> Cur;
        private State<Tin, Tout> Error;

        public StateMachine() {
            Error = new State<Tin, Tout>(Error, false, default(Tout));
        }

        public bool Peek(Tin c) => Cur.Transition(c).Accepting;

        public State<Tin, Tout> NextState(Tin c) {
            Cur = Cur.Transition(c);
            return Cur;
        }

        public State<Tin, Tout> AddStartState(bool accepting=false, Tout action=default(Tout), State<Tin, Tout> defaultState = null) {
            var s = new State<Tin, Tout>(defaultState ?? Error, accepting, action);
            Cur = s;
            return s;
        }

        public State<Tin, Tout> ErrorState() => Error;

        public State<Tin, Tout> AddState(bool accepting=false, Tout action=default(Tout), State<Tin, Tout> defaultState = null) 
            => new State<Tin, Tout>(defaultState ?? Error, accepting, action);
    }
}
