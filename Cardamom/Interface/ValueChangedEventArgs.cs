using System;
namespace Cardamom.Interface
{
	public class ValueChangedEventArgs<T> : EventArgs
	{
		public readonly T Value;

		public ValueChangedEventArgs(T Value)
		{
			this.Value = Value;
		}
	}
}
