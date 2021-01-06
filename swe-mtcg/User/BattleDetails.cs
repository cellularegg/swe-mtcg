using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace swe_mtcg.User
{
    public class BattleDetails
    {
        // TODO NOT TESTED
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string Winner { get; set; }
        public Dictionary<int, string> Rounds { get; set; }

        public BattleDetails(string u1, string u2)
        {
            User1 = u1;
            User2 = u2;
            Rounds = new Dictionary<int, string>();
        }

        public void AddRound(string msg)
        {
            if (Rounds.Count == 0)
            {
                Rounds.Add(1, msg);
            }
            else
            {
                Rounds.Add(Rounds.Keys.Max() + 1, msg);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"User1: {this.User1} VS User2: {User2}");
            sb.AppendLine($"Total Rounds: {Rounds.Count}");
            foreach (KeyValuePair<int,string> round in Rounds)
            {
                sb.AppendLine($"{round.Key} - {round.Value}");
            }

            if (Winner == "Draw")
            {
                sb.AppendLine("Draw! Nobody won.");
            }
            else
            {
                sb.AppendLine($"{Winner} won!");
            }

            return sb.ToString();
        }
    }
}