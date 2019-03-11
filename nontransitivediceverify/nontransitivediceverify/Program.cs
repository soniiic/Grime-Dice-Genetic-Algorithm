using System;

namespace nontransitivediceverify
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game()
            {

                new Die() {2, 2, 2, 6, 2, 6},
                new Die() {1, 4, 1, 4, 5, 4},
                new Die() {3, 6, 3, 3, 2, 3},
                new Die() {1, 5, 5, 2, 5, 1},
                new Die() { 3, 2, 3, 4, 4, 3 }
            };
            game.TestFitness();
            Console.WriteLine($"fitness: {game.Fitness}, SScore: {string.Join(", ", game.SingleBattleScores)}, DScore: {string.Join(", ", game.DoubleBattleScores)}, CScore: {string.Join(", ", game.CrossBattleScores)}, DCScore: {string.Join(", ", game.DoubleCrossBattleScores)}");
            Console.WriteLine($"Next die: {string.Join(", ", game.SingleBattleScores.Select(s => Math.Round(s, 1)))}");
            Console.WriteLine($"Prev die (double): {string.Join(", ", game.DoubleBattleScores.Select(s => Math.Round(s, 1)))}");
            Console.WriteLine($"Cross die: {string.Join(", ", game.CrossBattleScores.Select(s => Math.Round(s, 1)))}");
            Console.WriteLine($"Cross die (double): {string.Join(", ", game.DoubleCrossBattleScores.Select(s => Math.Round(s, 1)))}");
            Console.ReadLine();
        }
    }
}
