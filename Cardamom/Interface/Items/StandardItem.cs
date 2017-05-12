using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Interface.Items.Subcomponents;

namespace Cardamom.Interface.Items
{
    public class StandardItem<T> : GuiConstruct<T>
    {
        TextComponent _Text;

        public StandardItem(string ClassName, Series Series)
            : base(ClassName, Series)
        {
            RectComponent R = new RectComponent(_Class);
            _Text = new TextComponent(_Class);
            _Box = R.GetBoundingBox();

            _Components = new Component[]
            {
                R,
                _Text
            };
        }

        public string DisplayedString { get { return _Text.DisplayedString; } set { _Text.DisplayedString = value; } }
    }
}
