using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlet.Operands;
using Type = Outlet.Operands.Type;

namespace Outlet {
	public class Overload<T> where T : IOverloadable {

		private readonly List<T> Overloads = new List<T>(); 

		public Overload(params T[] t) {
			Overloads = t.ToList();
		}

		public void Add(T t) => Overloads.Add(t);

		// finds closest match
		public T FindBestMatch(params Type[] inputs) {
            (T best, int bestLevel) = (default, -1);
            foreach(T overload in Overloads)
            {
                bool valid = overload.Valid(out int level, inputs);
                if (!valid) continue;
                if (bestLevel == -1 || level < bestLevel)
                {
                    (best, bestLevel) = (overload, level);
                }
            }

            return best;
		}
	}

	public interface IOverloadable {
		bool Valid(out int level, params Type[] inputs);
	}
}
