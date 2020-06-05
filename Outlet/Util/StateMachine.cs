using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class StateMachine<Tin, Tout> where Tout : class where Tin : struct
    {

        public State<Tin, Tout> Cur;

        public StateMachine() {
            // temporary start state which is overwritten when AddStartState is called
            Cur = new State<Tin, Tout>(false, false);
        }

        public bool Peek(Tin c) => Cur.CanTransition(c);

        public State<Tin, Tout> NextState(Tin c) 
        {
            Cur = Cur.Transition(c);
            return Cur;
        }

        public State<Tin, Tout> AddStartState(bool accepting=false, bool keep=false, Tout? action = default) 
        {
            var s = new State<Tin, Tout>(accepting, keep, action);
            Cur = s;
            return s;
        }

        public State<Tin, Tout> AddState(bool accepting=false, bool keep = false, Tout? action = default) 
            => new State<Tin, Tout>(accepting, keep, action);
    }
}
