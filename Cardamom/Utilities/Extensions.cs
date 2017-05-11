using System;
using System.Collections.Generic;

namespace Cardamom.Utilities
{
	public static class Extensions
	{
		public static T ArgMax<T>(this IEnumerable<T> Source, Func<T, double> Selector)
		{
			T maxValue = default(T);
			double max = 0;
			bool assigned = false;

			foreach (T item in Source)
			{
				double v = Selector(item);
				if (v > max || !assigned)
				{
					assigned = true;
					max = v;
					maxValue = item;
				}
			}
			return maxValue;
		}
	}
}
