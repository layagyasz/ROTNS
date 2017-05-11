using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cardamom.Interface.Items.Subcomponents
{
    public interface Component : Pod
    {
        void PerformTransitions(Dictionary<string, float> Transitions, SubClass From, SubClass To);
    }
}
