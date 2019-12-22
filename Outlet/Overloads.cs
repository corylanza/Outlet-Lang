using System.Collections.Generic;
using System.Linq;
using Outlet.Types;

namespace Outlet {
	public class Overload<T> where T : IOverloadable {

		private readonly List<T> Overloads = new List<T>(); 

		public Overload(params T[] t) {
			Overloads = t.ToList();
		}

		public void Add(T t) => Overloads.Add(t);

		// finds closest match
		public T FindBestMatch(params ITyped[] inputs) {
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
		bool Valid(out int level, params ITyped[] inputs);
	}
}
