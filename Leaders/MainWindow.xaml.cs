using AoCLibrary;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Leaders
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, ILogger
	{
		private ElfResult? _last = null;
		private DateTime _next = DateTime.MinValue;

		public MainWindow()
		{
			InitializeComponent();
			DispatcherTimer timer = new ();
			timer.Tick += Timer_Tick;
			timer.Interval = TimeSpan.FromMinutes(1);
			timer.Start();
		}

		private async void Timer_Tick(object? sender, EventArgs e)
		{
			await Tick(false);
		}

		public void Log(string str)
		{
			ElfHelper.Log("LEAD " + str);
			lst.Items.Insert(0, $"{DateTime.Now} {str}");
		}

		async Task Tick(bool force)
		{
			Title = "Day" + ElfHelper.DayString();
			UpdateNextButton();

			try
			{
				await Read(force);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}

		private void UpdateNextButton()
		{
			btnAddNext.IsEnabled = false;
			for (int day = ElfHelper.Day(); day <= 25; day++)
			{
				var dayFile = $"Day{day:00}.cs";
				if (!File.Exists(Path.Combine(ElfHelper.CodeDir(), dayFile)))
				{
					btnAddNext.IsEnabled = true;
					btnAddNext.Content = $"Add Day{day:00}";
					btnAddNext.Tag = day;
					break;
				}
			}
		}

		async Task Read(bool force)
		{
			if (!force && DateTime.Now < _next)
				return;

			var aocResult = await ElfHelper.Read(force);
			Debug.Assert(aocResult != null);
			Log("Updating from: " + aocResult.Timestamp);
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

			_next = aocResult.Timestamp.AddMinutes(15);
			Log("Next update " + _next);
			_last = aocResult;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			await Tick(false);
		}

		private async void Now_Click(object sender, RoutedEventArgs e)
		{
			await Tick(true);
		}

		private void AddNext_Click(object sender, RoutedEventArgs e)
		{
			var day = (int) btnAddNext.Tag;
			ElfHelper.WriteStubFiles(day, true);
			Log("Created next day" + day);
			UpdateNextButton();
		}
	}
}
