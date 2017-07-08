using System;
namespace Cardamom.Interface.Items
{
	public class TextInput : StandardItem<string>
	{
		public TextInput(string ClassName)
			: base(ClassName, Series.Standard)
		{
			OnChange += (sender, e) => DisplayedString = Value;
		}

		public override void Update(MouseController MouseController, KeyController KeyController, int DeltaT, SFML.Graphics.Transform Transform)
		{
			base.Update(MouseController, KeyController, DeltaT, Transform);

			if (KeyController.Character != KeyController.NONE)
			{
				if (KeyController.Character == KeyController.BACKSPACE && Value.Length > 0)
					Value = Value.Substring(0, Value.Length - 1);
				else
					Value += KeyController.Character;
				DisplayedString = Value;
			}
		}
	}
}
