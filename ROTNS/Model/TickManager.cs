using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ROTNS.Model
{
	public class TickManager
	{
		public EventHandler<EventArgs> OnRemove;

		List<Ticked> _Items = new List<Ticked>();

		public TickManager()
		{
		}

		public void Add(Ticked Item) { _Items.Add(Item); }
		public void Remove(Ticked Item) { _Items.Remove(Item); }

		private void Tick()
		{
			lock(_Items)
			{
				_Items.ForEach(i => i.Tick());
				_Items = _Items.Where(
					i => {
						if (i.Remove && OnRemove != null) OnRemove(i, EventArgs.Empty); return !i.Remove;
				}).ToList();
			}
		}

		public void Start()
		{
			Timer T = new Timer(1000);
			T.AutoReset = true;
			T.Elapsed += (sender, e) => Tick();
			T.Start();
		}
	}
}
