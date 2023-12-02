using AoCLibrary;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace Leaders
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, ILogger
	{
		private AoCResult? _last = null;
		private DateTime _next = DateTime.MinValue;

		public MainWindow()
		{
			InitializeComponent();
			DispatcherTimer timer = new DispatcherTimer();
			timer.Tick += Timer_Tick;
			timer.Interval = TimeSpan.FromMinutes(1);
			timer.Start();
		}

		private async void Timer_Tick(object? sender, EventArgs e)
		{
			if (DateTime.Now > _next)
				await Read(false);
		}

		public void Log(string str)
		{
			lst.Items.Insert(0, $"{DateTime.Now} {str}");
		}

		async Task Read(bool force)
		{
			var res = await Communicator.Read($"https://adventofcode.com/{DateTime.Today.Year}/leaderboard/private/view/1403088.json", overrideThrottle: force);
			Log("Updating real: " + res.RealRead);
			var aocResult = AoCHelper.Deserialize(res.Json);
			Debug.Assert(aocResult != null);
			File.WriteAllText(@"c:\temp\data\aoc.json", AoCHelper.Serialize(aocResult));
			if (aocResult.HasChanges(_last, this) || force)
			{
				lstResults.Items.Clear();
				var ordered = aocResult.AllMembers().OrderByDescending(m => m.LocalScore);
				var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
				int i = 0;
				foreach (var showable in showables)
					lstResults.Items.Add($"{++i}. {showable}");
			}
			else
				Log("Data unchanged.");

			_last = aocResult;
			_next = DateTime.Now.AddMinutes(15);
			Log("Next update " + _next);
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await Read(false);
		}

		private async void Now_Click(object sender, RoutedEventArgs e)
		{
			await Read(true);
		}
	}
}
