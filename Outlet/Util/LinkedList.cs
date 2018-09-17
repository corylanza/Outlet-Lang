using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet.Util {
    public class LinkedList<T> : IEnumerable<T> {

        private class LLNode<T> {
            T val;
            LLNode<T> next;
            internal LLNode(T val, LLNode<T> next) {
                this.val = val;
                this.next = next;
            }
        }

        public LinkedList() { }


        public IEnumerator<T> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }
}
