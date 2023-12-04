using AoCLibrary;
using Finder2020Win;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Windows;
using System.Windows.Threading;

namespace Leaders
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ElfResult? _last = null;
		private DateTime _next = DateTime.MinValue;

		public MainWindow()
		{
			ElfHelper.AppName = "LEAD";
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

		void Log(string str)
		{
			ElfHelper.Log(str);
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
			for (int day = ElfHelper.Day; day <= 25; day++)
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

			var elfResult = await ElfHelper.Read(force);
			Debug.Assert(elfResult != null);
			Log("Updating UI with data from: " + elfResult.Timestamp);

			var changes = elfResult.HasChanges(_last);
			if (changes.Any())
			{
				foreach (var change in changes)
					Log(change);
				Sms.SendMessage("4109608923", "ELF Alert!" + Environment.NewLine + string.Join(Environment.NewLine, changes));
			}

			staLeft.Text = "Left today: " + elfResult.PointsLeftToday();

			var ordered = elfResult.AllMembers(true).OrderByDescending(m => m.LocalScore);
			var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
			int i = 0;
			var vms = new List<MemberViewModel>();
			var prevScore = showables.First().LocalScore;
			foreach (var showable in showables)
			{
				++i;
				vms.Add(new MemberViewModel(showable, i, prevScore));
			}
			grd.ItemsSource = vms;

			_next = elfResult.Timestamp.Add(ElfHelper.MinApiRefresh);
			staNext.Text = "Next update " + _next;
			_last = elfResult;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//await ElfHelper.WriteStubFiles(4, false);
			await Tick(false);
		}

		private async void Now_Click(object sender, RoutedEventArgs e)
		{
			await Tick(true);
		}

		private async void AddNext_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure?", "Sure?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
			{
				var day = (int)btnAddNext.Tag;
				await ElfHelper.WriteStubFiles(day, true);
				Log("Created next day" + day);
				UpdateNextButton();
			}
		}

		private void Puzzle_Click(object sender, RoutedEventArgs e)
		{
			//https://adventofcode.com/2023/day/4
			ElfHelper.Open($"https://adventofcode.com/{ElfHelper.Year}/day/{ElfHelper.Day}");
		}

		private void Leaderboard_Click(object sender, RoutedEventArgs e)
		{
			ElfHelper.Open($"https://adventofcode.com/{ElfHelper.Year}/leaderboard/private/view/1403088");
		}
	}
}
