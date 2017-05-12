using System;
namespace Cence
{
	public static class Constant
	{
		public static Func<double, double, T> Create<T>(T Value)
		{
			return (x, y) => Value;
		}
	}
}
