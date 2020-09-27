using System.Collections.Generic;
using System.Linq;
using Outlet.Types;

namespace Outlet {
	public class Overload<T> where T : class, IOverloadable
	{

		private readonly List<T> Overloads = new List<T>(); 

		public Overload(params T[] t) {
			Overloads = t.ToList();
		}

		public void Add(T t) => Overloads.Add(t);

        // finds closest match
        public T? FindBestMatch(params Type[] inputs)
        {
            (T? best, uint? bestLevel) = (default, null);
            foreach (T overload in Overloads)
            {
                if (overload.Valid(out uint level, inputs) && (bestLevel is null || level < bestLevel))
                {
                    (best, bestLevel) = (overload, level);
                }
            }

            return best;
        }
    }

	public interface IOverloadable {
		bool Valid(out uint level, params Type[] inputs);
	}
}
