namespace BattlePlannerApp
{
    class Program
    {
        static void Main()
        {
            string ownInput = "Spearmen#10;Militia#30;FootArcher#20;LightCavalry#1000;HeavyCavalry#120";
            string opponentInput = "Militia#10;Spearmen#10;FootArcher#1000;LightCavalry#120;CavalryArcher#100";

            var ownPlatoons = ownInput.Split(';').Select(s =>  Platoon.Parse(s)).ToList();
            var opponentPlatoons = opponentInput.Split(';').Select(s =>  Platoon.Parse(s)).ToList();

            int wins = 0;
            for (int i = 0; i < ownPlatoons.Count; i++)
            {
                Outcome result = Battle.Resolve(ownPlatoons[i], opponentPlatoons[i]);
                if (result == Outcome.Win)
                    wins++;
            }

            Console.WriteLine(wins >= 3
                ? string.Join(';', ownPlatoons)
                : "There is no chance of winning");
        }
    }

    public enum Outcome { Win = 1, Draw = 0, Loss = -1 }

    public class Platoon
    {
        public string Type { get; }
        public int Count { get; }

        private Platoon(string SoliderType, int count)
        {
            Type = SoliderType;
            Count = count;
        }

        public static Platoon Parse(string input)
        {
            var parts = input.Split('#');
            return new Platoon(parts[0], int.Parse(parts[1]));
        }

        public override string ToString() => $"{Type}#{Count}";
    }

    public static class Battle
    {
        private static readonly Dictionary<string, HashSet<string>> AdvantageByClass = new()
        {
            ["Militia"] = new() { "Spearmen", "LightCavalry" },
            ["Spearmen"] = new() { "LightCavalry", "HeavyCavalry" },
            ["LightCavalry"] = new() { "FootArcher", "CavalryArcher" },
            ["HeavyCavalry"] = new() { "Militia", "FootArcher", "LightCavalry" },
            ["CavalryArcher"] = new() { "Spearmen", "HeavyCavalry" },
            ["FootArcher"] = new() { "Militia", "CavalryArcher" }
        };

        public static Outcome Resolve(Platoon attacker, Platoon enemy)
        {
            bool hasAdvantage = AdvantageByClass.TryGetValue(attacker.Type, out var targets)
                                && targets.Contains(enemy.Type);

            int power = attacker.Count * (hasAdvantage ? 2 : 1);

            return power > enemy.Count ? Outcome.Win : power == enemy.Count ? Outcome.Draw : Outcome.Loss;
        }
    }
}
