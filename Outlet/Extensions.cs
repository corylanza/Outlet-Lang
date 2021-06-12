using Outlet.AST;
using Outlet.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outlet
{
    public static class Extensions
    {
		public static bool SameLengthAndAll<T, U>(this IEnumerable<T> list, IEnumerable<U> other, Func<T, U, bool> predicate)
		{
			if (list.Count() != other.Count()) return false;
			for (int i = 0; i < list.Count(); i++)
			{
				if (!predicate(list.ElementAt(i), other.ElementAt(i))) return false;
			}
			return true;
		}

		public static IEnumerable<(F, S)> Zip<F, S>(this IEnumerable<F> first, IEnumerable<S> second)
		{
			if (first.Count() == second.Count())
			{
				return Enumerable.Range(0, first.Count()).Select(idx => (first.ElementAt(idx), second.ElementAt(idx)));
			}
			throw new Exception("can't zip collections of different lengths");
		}

		public static T Dequeue<T>(this LinkedList<T> ll)
		{
			T temp = ll.First();
			ll.RemoveFirst();
			return temp;
		}

		public static Literal ToLiteral(this TokenLiteral literal)
		{
			return literal switch
			{
				IntLiteral i => new Literal<int>(i.Value),
				FloatLiteral f => new Literal<float>(f.Value),
				BoolLiteral b => new Literal<bool>(b.Value),
				Tokens.StringLiteral s => new AST.StringLiteral(s.Value),
				NullLiteral _ => new NullExpr(),
				_ => throw new NotImplementedException()
			};
		}

		public static Variable ToVariable(this string s) => new Variable(s);
	}
}
