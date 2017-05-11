using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Window;
using Cardamom.Interface.Items;
using Cardamom.Interface;
using ROTNS.Model;

using Venetia;

namespace ROTNS.View
{
	class RegionViewWindow : Pane
	{
		RegionView _Region;
		WorldView _World;

		TextBox _Readout = new TextBox("text-box");
		Button _ExitButton = new Button("button-exit");

		Container<Pod> _SummaryPage = new Container<Pod>();
		Container<Pod> _EconomyPage = new Container<Pod>();
		Container<Pod> _TradePage = new Container<Pod>();
		Container<Pod> _DiplomacyPage = new Container<Pod>();

		Table _EconomyTable = new Table("table") { Position = new Vector2f(0, 36)};

		Container<Pod>[] _Pages;

		public RegionViewWindow(string ClassName)
			: base(ClassName)
		{
			_Pages = new Container<Pod>[] { _SummaryPage, _EconomyPage, _TradePage, _DiplomacyPage };
			Array.ForEach(_Pages, Add);

			_Readout.Position = new Vector2f(2, 98);
			_SummaryPage.Add(_Readout);
			_EconomyPage.Add(_EconomyTable);

			string[] Classes = new string[]
			{
				"button-summary",
				"button-wealth",
				"button-trade",
				"button-diplomacy",
			};
			Action<object, MouseEventArgs>[] Actions = new Action<object, MouseEventArgs>[]
			{
			};

			for (int i = 0; i < Classes.Length; ++i)
			{
				Button Button = new Button(Classes[i]);
				Button.Position = new SFML.Window.Vector2f(34 * i, 0);
				Button.Value = _Pages[i];
				Button.OnClick += TogglePage;
				Add(Button);
				if (i == 0) TogglePage(Button, new MouseEventArgs(new Vector2f(0,0)));
			}

			_ExitButton.Position = new Vector2f(756, 0);

			_ExitButton.OnClick += (S, E) => { this.Visible = false; _World.HighlightRegion(_Region, false); };

			Add(_ExitButton);
		}

		private void TogglePage(object Sender, MouseEventArgs E)
		{
			Array.ForEach(_Pages, i => i.Visible = i == ((Button)Sender).Value);
		}

		public void SetRegion(WorldView World, RegionView Region)
		{
			if (_Region != null) _World.HighlightRegion(_Region, false);
			_Region = Region;
			_World = World;
			World.HighlightRegion(Region, true);

			_Readout.DisplayedString = String.Format(
				"{0}\nPopulation: ~{1:N0}\nProsperity: {2:0.00}\nGovernment:~{3}",
				Region.Region.Name,
				RoundToFigures((int)Region.Region.Population, 2),
				Region.Region.FlowPerCapita(),
				Region.Region.Administration.GovernmentForm);
			if (_SummaryPage.Items.Count == 1) _SummaryPage.Add(Region.Region.Administration.Flag);
			else _SummaryPage.Items[_SummaryPage.Items.Count - 1] = Region.Region.Administration.Flag;
			Region.Region.Administration.Flag.Position = new Vector2f(2, 36);

			_EconomyTable.Clear();
			foreach (Tangible Tangible in Region.Region.Economy.Goods)
			{
				_EconomyTable.Add(
					new TableRow("table-row") { new Button("table-cell") { DisplayedString = Tangible.Name } });
			}
		}

		private int RoundToFigures(int Number, int Digits)
		{
			int Floor = (int)Math.Pow(10, (int)Math.Log10(Number) + 1 - Digits);
			double D = (double)Number / Floor;
			return (int)(Math.Round(D) * Floor);
		}

		private string PercentileDescriptor(float Percentile)
		{
			if (Percentile < .2f) return "Poor";
			else if (Percentile < .4f) return "Below Average";
			else if (Percentile < .6f) return "Average";
			else if (Percentile < .8f) return "Above Average";
			else return "Wealthy";
		}
	}
}
