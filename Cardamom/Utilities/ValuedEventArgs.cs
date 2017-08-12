using System;

namespace Cardamom.Utilities
{
	public class ValuedEventArgs<T> : EventArgs
	{
		public readonly T Value;

		public ValuedEventArgs(T Value)
		{
			this.Value = Value;
		}
	}
}
