﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class StateMachine<Tin, Tout> {

        public State<Tin, Tout> Cur;

        public StateMachine() { }

        public bool Peek(Tin c) => Cur.CanTransition(c);

        public State<Tin, Tout> NextState(Tin c) {
            Cur = Cur.Transition(c);
            return Cur;
        }

        public State<Tin, Tout> AddStartState(bool accepting=false, bool keep=false, Tout action=default(Tout)) {
            var s = new State<Tin, Tout>(accepting, keep, action);
            Cur = s;
            return s;
        }

        public State<Tin, Tout> AddState(bool accepting=false, bool keep = false, Tout action=default(Tout)) 
            => new State<Tin, Tout>(accepting, keep, action);
    }
}