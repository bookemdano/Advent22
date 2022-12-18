using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static Advent22.Day16;
using static Advent22.Days.Day14;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Advent22
{
    internal class Day16b
    {
        public class ValveList
        {
            public ValveList(string[] input)
            {
                Valves = new List<Valve>();
                foreach (var line in input)
                {
                    var valve = Valve.Parse(line);
                    if (valve != null)
                        Valves.Add(valve);
                }

                // connect
                foreach (var valve in Valves)
                    valve.Connect(Valves);

            }

            public List<Valve> Valves { get; set; }
            public void Prune()
            {
                var keepGoing = true;
                while (keepGoing)
                {
                    var removes = new List<Valve>();
                    keepGoing = false;
                    foreach (var valve in Valves)
                    {
                        if (valve.Prune(Valves))
                            keepGoing = true;
                        if (!valve.Connections.Any())
                        {
                            removes.Add(valve);
                            keepGoing = true;
                        }
                    }
                    foreach (var valve in removes)
                        Valves.Remove(valve);
                }
                Valves = Valves.OrderByDescending(v => v.Rate).ToList();
            }

            internal Valve Move()
            {
                return Valves.FirstOrDefault(v => v.Opened == false);
            }

            internal int Flow()
            {
                return Valves.Where(v => v.Opened).Sum(v => v.Rate);
            }
        }

        public class Valve
        {
            public string Name { get; set; }
            public int Rate { get; set; }
            string _connectionString { get; set; }
            public List<Valve> Connections { get; set; }
            public bool Opened { get; private set; }
            public bool Closed
            {
                get
                {
                    return !Opened;
                }
            }
            public void Open()
            {
                Opened = true;
                Helper.Log("Opened " + this);
            }

            static public Valve Parse(string line)
            {

                var halves = line.Split(';');
                var parts = halves[0].Split("=; ".ToCharArray());
                var rate = int.Parse(parts[5]);
                var rv = new Valve();
                rv.Rate = rate;
                rv.Name = parts[1];
                rv._connectionString = halves[1].Substring(23).Trim();
                return rv;
            }
            public void Connect(List<Valve> valves)
            {
                var parts = _connectionString.Split(", ");
                Connections = new List<Valve>();
                foreach (var part in parts)
                {
                    var found = valves.FirstOrDefault(v => v.Name == part);
                    if (found != null)
                        Connections.Add(found);
                }
            }

            public bool Prune(List<Valve> valves)
            { 
                if (!Connections.Any())
                    return false;

                var removes = new List<Valve>();
                bool change = false;
                foreach(var conn in Connections)
                {
                    if (!valves.Contains(conn))
                        removes.Add(conn);
                }

                foreach (var valve in removes)
                {
                    Connections.Remove(valve);
                    change = true;
                }
                return change;
            }
            public override string ToString()
            {
                var rv = $"{Name} r:{Rate} {(Opened?"O":"X")}";
                if (Connections?.Any() == true)
                    rv += $" to:{string.Join(",", Connections.Select(c => c.Name))}";
                return rv;
            }


            internal Valve Move(int daysRemaining)
            {
                var options = new List<ValveOption>();
                var rate = Rate * (daysRemaining);
                options.Add(new ValveOption(this, rate));
                int level = 5;
                foreach (var conn in Connections)
                {
                    var option = new ValveOption(conn, conn.Rate * (daysRemaining - 1));
                    foreach (var conn2 in conn.Connections)
                    {
                        rate = conn2.Rate * (daysRemaining - 2);
                        if (rate > option.Rate)
                            option.Rate = rate;
                    }
                    options.Add(option);
                    
                }
                return this;
                //return Connections.FirstOrDefault(c => !c.Opened);
                /*
                var max = 0;
                Valve target = null;
                foreach(var valve in Connections)
                {
                    if (valve.Opened == false && valve.Rate > max)
                    {
                        max = valve.Rate;
                        target = valve;
                    }
                }
                return target;
                */
            }
        }
        internal class ValveOption
        {
            public ValveOption(Valve valve, int rate)
            {
                Target = valve;
                Rate = rate;
            }

            public Valve Target { get; }
            public int Rate { get; set; }
            public override string ToString()
            {
                return $"{Target} {Rate}";
            }
        }
        internal class Chain
        {
            public List<ValveDay> Valves { get; set; }
        }
        internal class ValveDay
        {
            public ValveDay(Valve from, Valve to, int rollRate, int days)
            {
                From = from;
                At = to;
                RollRate = rollRate;
                Days = days;
            }
            public string Edge
            {
                get
                {
                    return $"{From?.Name}-{At?.Name}";
                }
            }
            public int Days { get; set; }
            public Valve From { get; set; }
            public Valve At { get; set; }
            public int RollRate { get; set; }
            public override string ToString()
            {
                return $"at:{At} edge:{Edge} d:{Days} rollRate:{RollRate}";
            }
        }
        static public void Run()
        {
            Day1();
            //Day2();
        }
        static void Day1()
        {
            var input = File.ReadAllLines("DayFake16.txt");
            var valveList = new ValveList(input);
            //valveList.Prune();
            var currentValve = valveList.Valves.First(v => v.Name == "AA");
            var flow = 0;
            for(int i = 0; i < 30; i++)
            {
                flow += valveList.Flow();
                if (currentValve.Rate == 0 || currentValve.Opened)
                {
                    var possible = currentValve.Move(30 - i);
                    //var possible = currentValve.Move();
                    if (possible != currentValve)
                    {
                        currentValve = possible;
                        continue;
                    }
                }
                if (!currentValve.Opened)
                {
                    var possible = currentValve.Move(30 - i);
                    if (possible.Rate > currentValve.Rate)
                        currentValve = possible;
                    else
                        currentValve.Open();
                }
                Helper.Log($"Flow at {i} = {flow} on {currentValve?.Name}");
            }
            Helper.Log("Star1 Score: " + flow); 
        }
        static void Day2()
        {
            var input = File.ReadAllLines("DayFake15.txt");
            var score = 0;
            Helper.Log("Star2 Score: " + score);
        }
    }
}

