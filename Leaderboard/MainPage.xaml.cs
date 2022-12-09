using AoCLibrary;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text.Json;

namespace Leaderboard
{
    public partial class MainPage : ContentPage, ILogger
    {
        IDispatcherTimer _timer;
        public MainPage()
        {
            InitializeComponent();
            lst.ItemsSource = Members;
            lstLog.ItemsSource = LogCollection;
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            await Refresh(false);
            var now = DateTime.Now;
            var start = new DateTime(2022, 12, 1);
            var minutes = (now - start).TotalMinutes;
            var rate = 15;
            var nextBlockMinute = (int) (1 + (minutes / rate)) * rate;

            var intervalMinutes = nextBlockMinute - minutes;
            if (intervalMinutes < 1)
                intervalMinutes += rate;
            _timer.Interval = TimeSpan.FromMinutes(intervalMinutes);
            staNext.Text = "Next: " + now.AddMinutes(intervalMinutes).ToString("H:mm:ss");
        }

        public ObservableCollection<MemberModel> Members { get; } = new ObservableCollection<MemberModel>();
        public ObservableCollection<string> LogCollection { get; } = new ObservableCollection<string>();
        AoCResult _last = null;

        private async void Refresh_Clicked(object sender, EventArgs e)
        {
            await Refresh(false);
        }
        async Task Refresh(bool overrideThrottle)
        {
            Log("Checking");
            var result = await Communicator.Read("https://adventofcode.com/2022/leaderboard/private/view/1403088.json", overrideThrottle);
            var json = result.Item1;
            var realRead = result.Item2;
            if (realRead)
                Log("Novel Read");

            var aocResult = AoCHelper.Deserialize(json);
            if (aocResult.HasChanges(_last, this))
            {
                if (_last != null)
                    staLast.Text = "Last Change: " + DateTime.Now;
                //var showables = aocResult.AllMembers.OrderByDescending(m => m.LocalScore).Take(10).ToArray();
                var ordered = aocResult.AllMembers().OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                Members.Clear();
                int i = 1;
                foreach (var showable in showables)
                    Members.Add(new MemberModel(showable, i++));
            }
            _last = aocResult;
        }

        public void Log(string str)
        {
            LogCollection.Insert(0, DateTime.Now.ToString() + ": " + str);
        }

        private async void Export_Clicked(object sender, EventArgs e)
        {
            if (_last == null)
                await Refresh(false);
            AoCHelper.Export(_last, this);
            Log("Exported");
        }

        private async void Force_Clicked(object sender, EventArgs e)
        {
            Log("Forced refresh");
            await Refresh(true);
        }
    }
    public class MemberModel
    {
        private Member _member;
        private int _order;

        public MemberModel(Member member, int order)
        {
            _member = member;
            _order = order;
        }
        public string Name
        {
            get
            {
                var days = (int)((DateTime.Today - new DateTime(2022, 11, 30)).TotalDays);
                if (days > 25)
                    days = 25;
                string starString = null;
                var maxStars = days * 2;
                if (_member.Stars == maxStars)
                    starString = "⭐";
                else
                {
                    var delta = maxStars - _member.Stars;
                    if (delta % 2 == 1)
                        starString = $"({(delta / 2)}½)";
                    else
                        starString = $"({(delta / 2)})";
                }
                return $"{_order}{starString} {_member.GetName()}";
            }
        }
        public string Detail
        {
            get
            {
                var avg = "-";
                if (_member.Stars > 0)
                    avg = ((double)_member.LocalScore / (_member.Stars / 2.0)).ToString("0.0");
                var timeString = _member.LastTime().ToString("M/d H:mm");
                if (DateTime.Today == _member.LastTime().Date)
                    timeString = _member.LastTime().ToString("H:mm");

                return $"Σ:{_member.LocalScore} μ:{avg} {timeString}";
            }
        }

    }
}