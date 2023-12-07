using AoCLibrary;
using System.Diagnostics;
using System.IO;
using System.Windows;
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
		}

		private void Timer_Tick(object? sender, EventArgs e)
		{
			Tick(false);
		}

		void Log(string str)
		{
			Utils.Log(str);
			lst.Items.Insert(0, $"{DateTime.Now} {str}");
		}

		void Tick(bool force)
		{
			Title = "Day" + ElfHelper.DayString();

			if (LastSentDay != ElfHelper.Day)
			{
				Send(ElfHelper.DailyUrl);
				LastSentDay = ElfHelper.Day;

				if (ElfHelper.Day == ElfHelper.NextEmptyDay())
				{
					ElfHelper.WriteStubFiles(ElfHelper.Day, true);
					Log("Created next day" + ElfHelper.Day);
					UpdateNextButton();
				}
			}
			
			try
			{
				Read(force);
			}
			catch (Exception ex)
			{
				Log(ex.ToString());
			}
		}

		private void UpdateNextButton()
		{
			btnAddNext.IsEnabled = false;
			var day = ElfHelper.NextEmptyDay();
			if (day > 0)
			{
				btnAddNext.IsEnabled = true;
				btnAddNext.Content = $"Add Day{day:00}";
				btnAddNext.Tag = day;
			}
		}
		int _lastDaySent = -1;
		int LastSentDay
		{
			get
			{
				if (_lastDaySent == -1)
				{
					_lastDaySent = 0;
					if (File.Exists(Path.Combine(Utils.Dir, "LastDaySent.cfg")))
					{
						var str = File.ReadAllText(Path.Combine(Utils.Dir, "LastDaySent.cfg"));
						int.TryParse(str, out _lastDaySent);
					}
				}
				return _lastDaySent;
			}
			set
			{
				_lastDaySent = value;
				File.WriteAllText(Path.Combine(Utils.Dir, "LastDaySent.cfg"), _lastDaySent.ToString());
			}
		}
		void Send(string str)
		{
			Log("Sending " + str);
			Sms.SendMessage("4109608923", "ELF Alert!" + Environment.NewLine + str);
		}
		void Read(bool force)
		{
			if (!force && DateTime.Now < _next)
				return;

			if (_last == null)
				_last = ElfHelper.ReadFromFile();

			var elfResult = ElfHelper.Read(force);
			Debug.Assert(elfResult != null);
			Log("Updating UI with data from: " + elfResult.Timestamp);
			var changes = elfResult.HasChanges(_last);
			if (changes.Any())
			{
				foreach (var change in changes)
				{
					Log(change);
					PlaySound();
				}
				Send(string.Join(Environment.NewLine, changes));
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
				vms.Add(new MemberViewModel(showable, i, prevScore, elfResult.Members.Count()));
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
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//await ElfHelper.WriteStubFiles(4, false);
			UpdateNextButton();
			Tick(false);
		}

		private void Now_Click(object sender, RoutedEventArgs e)
		{
			Tick(true);
		}

		private void AddNext_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure?", "Sure?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
			{
				var day = (int)btnAddNext.Tag;
				ElfHelper.WriteStubFiles(day, true);
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
	}
}
