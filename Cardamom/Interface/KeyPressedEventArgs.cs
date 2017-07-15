using System;

using SFML.Window;

namespace Cardamom.Interface
{
	public class KeyPressedEventArgs : EventArgs
	{
		public readonly Keyboard.Key Key;

		public KeyPressedEventArgs(Keyboard.Key Key)
		{
			this.Key = Key;
		}
	}
}
