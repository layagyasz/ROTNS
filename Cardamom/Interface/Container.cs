using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cardamom.Planar;

using SFML.Graphics;

namespace Cardamom.Interface
{
    public abstract class Container : Pod
    {
        protected List<Pod> _Items = new List<Pod>();

        public void Add(Pod Item) { _Items.Add(Item); }

        public virtual void Update(MouseController MouseController, KeyController KeyController, int DeltaT, PlanarTransformMatrix Transform)
        {
            foreach (Pod Item in _Items)
            {
                Item.Update(MouseController, KeyController, DeltaT, Transform);
            }
        }

        public virtual void Draw(RenderTarget Target, PlanarTransformMatrix Transform)
        {
            foreach (Pod Item in _Items)
            {
                Item.Draw(Target, Transform);
            }
        }
    }
}
