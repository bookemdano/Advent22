using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day2
    {
        public enum ShapeEnum
        {
            None,
            Rock,
            Paper,
            Scissors
        }
        public enum ResultEnum
        {
            None,
            Loss,
            Draw,
            Win
        }
        static public void Run()
        {
            // AX1 rock, BY2 paper, CZ3 scissors
            // win=6, draw=ShapeEnum.Scissors, loss=0
            // Day 1- quick
            var lines = File.ReadAllLines("Day2-input.txt");
            var score1 = 0;
            var score2 = 0;
            //lines = new string[] { "A Y", "B X", "C Z" };
            foreach (var line in lines)
            {
                var them = (ShapeEnum) (line[0] - 'A' + 1);

                // part 1
                var you = (ShapeEnum)(line[2] - 'X' + 1);
                score1 += Score(them, you);

                // part 2
                var plannedResult = (ResultEnum)(line[2] - 'X' + 1);
                if (plannedResult == ResultEnum.Draw)
                    you = them;
                else if (plannedResult == ResultEnum.Win)
                    you = Beats(them);
                else if (plannedResult == ResultEnum.Loss)
                    you = BeatBy(them);
                score2 += Score(them, you);
            }

            Console.WriteLine("score1 = " + score1);
            Console.WriteLine("score2 = " + score2);
        }
        // what beats thro
        static ShapeEnum Beats(ShapeEnum thro)
        {
            if (thro == ShapeEnum.Paper)
                return ShapeEnum.Scissors;
            else if (thro == ShapeEnum.Scissors)
                return ShapeEnum.Rock;
            else if (thro == ShapeEnum.Rock)
                return ShapeEnum.Paper;
            return ShapeEnum.None;
        }
        // what is beat by thro
        static ShapeEnum BeatBy(ShapeEnum thro)
        {
            if (thro == ShapeEnum.Paper)
                return ShapeEnum.Rock;
            else if (thro == ShapeEnum.Scissors)
                return ShapeEnum.Paper;
            else if (thro == ShapeEnum.Rock)
                return ShapeEnum.Scissors;
            return ShapeEnum.None;
        }
        static int Score(ShapeEnum them, ShapeEnum you)
        {
            var rv = (int)you;
            var result = ResultEnum.None;
            if (you == them)
            {
                rv += 3;
                result = ResultEnum.Draw;
            }
            else if (you == them + 1 || (them == ShapeEnum.Scissors && you == ShapeEnum.Rock))
            {
                rv += 6;
                result = ResultEnum.Win;
            }
            else
                result = ResultEnum.Loss;

            System.Diagnostics.Debug.Assert(result == Slow(them, you));

            return rv;
        }
        static ResultEnum Slow(ShapeEnum them, ShapeEnum you)
        {
            if (you == them)
                return ResultEnum.Draw;

            if (you == ShapeEnum.Paper && them == ShapeEnum.Rock)
                return ResultEnum.Win;
            if (you == ShapeEnum.Scissors && them == ShapeEnum.Rock)
                return ResultEnum.Loss;

            if (you == ShapeEnum.Rock && them == ShapeEnum.Paper)
                return ResultEnum.Loss;
            if (you == ShapeEnum.Scissors && them == ShapeEnum.Paper)
                return ResultEnum.Win;

            if (you == ShapeEnum.Rock && them == ShapeEnum.Scissors)
                return ResultEnum.Win;
            if (you == ShapeEnum.Paper && them == ShapeEnum.Scissors)
                return ResultEnum.Loss;

            return ResultEnum.None;
        }
    }
}
