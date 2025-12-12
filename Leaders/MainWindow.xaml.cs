using AoCLibrary;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Leaders
{
	public partial class MainWindow : Window
	{
		private ElfResult? _last = null;
		private DateTime _next = DateTime.MinValue;

		public MainWindow()
		{
			Utils.AppName = "LEAD";
			InitializeComponent();
			DispatcherTimer timer = new ();
			timer.Tick += Timer_Tick;
			timer.Interval = TimeSpan.FromMinutes(1);
			timer.Start();
			var projects = ElfHelper.GetProjectStrings();
			var currentYear = Utils.ReadConfig("year", ElfHelper.Year);
			var currentYear2 = (currentYear - 2000).ToString();
            foreach (var project in projects)
			{
				var mi = new MenuItem();
				mi.Header = project.ToString();
                mi.Click += Year_Click;
				mi.Tag = 2000 + int.Parse(project.Substring(6));
				if (project.Contains(currentYear2))
					mi.IsChecked = true;
                mnuYear.Items.Add(mi);
			}
			if (currentYear != ElfHelper.Year)
				ElfHelper.OverrideYear(currentYear);
        }
        private async void Year_Click(object sender, RoutedEventArgs e)
        {
			if (sender is not MenuItem mi)
				return;
			foreach (MenuItem other in mnuYear.Items)
				other.IsChecked = false;
			mi.IsChecked = true;
            ElfHelper.OverrideYear((int)mi.Tag);
			_last = null;
			await TickAsync(true);
            UpdateNextButton();
			Utils.WriteConfig("year", ElfHelper.Year);
        }

        private async void Timer_Tick(object? sender, EventArgs e)
		{
			await TickAsync(false);
		}

		void Log(string str)
		{
			Utils.MonthLog(str);
			lst.Items.Insert(0, $"{DateTime.Now} {str}");
		}

		async Task TickAsync(bool force)
		{
            ElfHelper.UpdateCurrentDay();

            Title = $"Day {ElfHelper.Year} {ElfHelper.DayString}";

            if (ElfHelper.IsActive && ElfHelper.CurrentDayOrNull != null)
			{
				// current day is going to be calendar date, regardless of what is done
				var lastSent = Utils.ReadConfig("lastsentdate", -1);
				if (lastSent != ElfHelper.CurrentDayOrNull)
				{
					Send(ElfHelper.DailyUrl);
					Utils.WriteConfig("lastsentdate", ElfHelper.CurrentDayOrLast);

					if (ElfHelper.CurrentDayOrLast == ElfHelper.NextEmptyDay())
					{
						await ElfHelper.WriteStubFilesAsync(ElfHelper.CurrentDayOrLast, true);
						Log("Created next day" + ElfHelper.CurrentDayOrLast);
						UpdateNextButton();
					}
				}
			}		
			try
			{
				await ReadAsync(force);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}

		private void UpdateNextButton()
		{
			mnuAddNext.IsEnabled = false;
			var day = ElfHelper.NextEmptyDay();
			if (day > 0)
			{
				mnuAddNext.IsEnabled = true;
				mnuAddNext.Header = $"Add Day{day:00}";
				mnuAddNext.Tag = day;
			}
		}
		void Send(string str)
		{
			Log("Sending " + str);
			Sms.SendMessage("4109608923", "ELF Alert!" + Environment.NewLine + str);
		}
		async Task ReadAsync(bool force)
		{
			if (!force && DateTime.Now < _next)
				return;

			_last ??= ElfHelper.ReadFromFile();

			var elfResult = await ElfHelper.ReadAsync(force);
			Debug.Assert(elfResult != null);
			Log("Updating UI with data from: " + elfResult.Timestamp);
			var changes = elfResult.HasChanges(_last);
			if (ElfHelper.Year == DateTime.Today.Year && changes.Any())
			{
				foreach (var change in changes)
				{
					Log(change);
					PlaySound();
				}
				Send(string.Join(Environment.NewLine, changes));
                staLeft.Text = "Left today: " + elfResult.PointsLeftToday();
            }

            var ordered = elfResult.AllMembers(true).OrderByDescending(m => m.LocalScore);
			var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
			int i = 0;
			var vms = new List<MemberViewModel>();
			if (showables.Any())
			{
                var prevScore = showables.First().LocalScore;
                foreach (var showable in showables)
                {
                    ++i;
                    vms.Add(new MemberViewModel(showable, i, prevScore, elfResult.Members.Count()));
                }
            }
            grd.ItemsSource = vms;

			_next = elfResult.Timestamp.Add(ElfHelper.MinApiRefresh);
			staNext.Text = "Next update " + _next;
			_last = elfResult;
		}
		internal static void PlaySound()
		{
			new System.Media.SoundPlayer("bring.wav").Play();
		}
		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//await ElfHelper.WriteStubFiles(4, false);
			UpdateNextButton();
			await TickAsync(false);
		}

		private async void Now_Click(object sender, RoutedEventArgs e)
		{
			await TickAsync(true);
		}

		private async void AddNext_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure?", "Sure?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
			{
				var day = (int)mnuAddNext.Tag;
				await ElfHelper.WriteStubFilesAsync(day, true);
				Log("Created next day" + day);
				UpdateNextButton();
			}
		}

		private void Puzzle_Click(object sender, RoutedEventArgs e)
		{
			Utils.Open(ElfHelper.DailyUrl);
		}

		private void Leaderboard_Click(object sender, RoutedEventArgs e)
		{
			Utils.Open(ElfHelper.LeaderUrl);
		}

		private void Slack_Click(object sender, RoutedEventArgs e)
		{
			var channelId = "C02NY1HMTN0";
			Utils.Open($"slack://channel?team=TB4KLF92L&id={channelId}");
		}
	}
}
