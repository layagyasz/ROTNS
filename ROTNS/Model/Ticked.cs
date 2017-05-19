using System;
namespace ROTNS.Model
{
	public interface Ticked
	{
		bool Remove { get; }
		void Tick(Random Random);
	}
}
