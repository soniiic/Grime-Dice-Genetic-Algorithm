namespace nontransitivedicegenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Game : List<Die>
    {
        private bool isDraw;
        public bool HasDraw { get; private set; }
        public float Fitness { get; private set; }

        public float[] SingleBattleScores { get; } = new float[Global.NUMBER_OF_DICE];
        public float[] DoubleBattleScores { get; } = new float[Global.NUMBER_OF_DICE];
        public float[] CrossBattleScores { get; } = new float[Global.NUMBER_OF_DICE];
        public float[] DoubleCrossBattleScores { get; } = new float[Global.NUMBER_OF_DICE];

        public void TestFitness()
        {
            Fitness = Enumerable.Range(0, Global.NUMBER_OF_DICE).AsParallel().Sum(i =>
            {
                var singleBattleScore = TestSingleBattleScore(i);
                if (float.IsNaN(singleBattleScore))
                {
                    return 0;
                }
                var doubleBattleScore = TestDoubleBattleScore(i);
                if (float.IsNaN(doubleBattleScore))
                {
                    return 0;
                }
                var crossBattleScore = TestCrossBattleScore(i);
                if (float.IsNaN(crossBattleScore))
                {
                    return 0;
                }
                var crossDoubleBattleScore = TestDoubleCrossBattleScore(i);
                if (float.IsNaN(crossDoubleBattleScore))
                {
                    return 0;
                }
                return singleBattleScore + doubleBattleScore + crossBattleScore + crossDoubleBattleScore;
            });

            //Fitness -= SingleBattleScores.Max() - SingleBattleScores.Min();
            //Fitness -= DoubleBattleScores.Max() - DoubleBattleScores.Min();
            //Fitness -= CrossBattleScores.Max() - CrossBattleScores.Min();
        }

        private float TestSingleBattleScore(int i)
        {
            var die1 = this[i];
            var die2 = this[i == this.Count - 1 ? 0 : i + 1];
            var wins = 0;
            var draws = 0;
            foreach (var die1Side in die1.Sides)
            {
                foreach (var die2Side in die2.Sides)
                {
                    if (die1Side > die2Side)
                    {
                        wins++;
                    }
                    else if (die1Side == die2Side)
                    {
                        this.HasDraw = true;
                        draws++;
                    }
                }
            }

            SingleBattleScores[i] = ((float)wins) / (36) * 100.0f; ;
            return CalculateFitness(wins, 36, draws);
        }

        private static float CalculateFitness(int wins, int totalThrows, int draws)
        {
            var fitness = ((float) wins) / totalThrows * 100.0f;
            fitness = (Math.Min(fitness, 50));
            fitness -= ((float) draws) / (totalThrows) * 100.0f * 1.25f;
            return fitness;
        }

        private float TestDoubleBattleScore(int i)
        {
            var die1 = this[i];
            var die2 = this[i == 0 ? this.Count - 1 : i - 1];
            var wins = 0;
            var draws = 0;

            foreach (var die1Side1 in die1.Sides)
                foreach (var die1Side2 in die1.Sides)
                    foreach (var die2Side1 in die2.Sides)
                        foreach (var die2Side2 in die2.Sides)
                        {
                            if (die1Side1 + die1Side2 > die2Side1 + die2Side2)
                                wins++;
                            else if (die1Side1 + die1Side2 == die2Side1 + die2Side2)
                            {
                                this.HasDraw = true;
                                draws++;
                            }
                        }

            DoubleBattleScores[i] = ((float)wins) / (1296) * 100.0f;
            return CalculateFitness(wins, 1296, draws);
        }

        private float TestCrossBattleScore(int i)
        {
            var die1 = this[i];
            var die2 = this[i + 3 > this.Count - 1 ? i - 2 : i + 3];
            var wins = 0;
            var draws = 0;

            foreach (var die1Side in die1.Sides)
            {
                foreach (var die2Side in die2.Sides)
                {
                    if (die1Side > die2Side)
                    {
                        wins++;
                    }
                    else if (die1Side == die2Side)
                    {
                        this.HasDraw = true;
                        draws++;
                    }
                }
            }

            CrossBattleScores[i] = ((float)wins) / (36) * 100.0f;
            return CalculateFitness(wins, 36, draws);
        }

        private float TestDoubleCrossBattleScore(int i)
        {
            var die1 = this[i];
            var die2 = this[i + 3 > this.Count - 1 ? i - 2 : i + 3];
            var wins = 0;
            var draws = 0;

            foreach (var die1Side1 in die1.Sides)
                foreach (var die1Side2 in die1.Sides)
                    foreach (var die2Side1 in die2.Sides)
                        foreach (var die2Side2 in die2.Sides)
                        {
                            if (die1Side1 + die1Side2 > die2Side1 + die2Side2)
                                wins++;
                            else if (die1Side1 + die1Side2 == die2Side1 + die2Side2)
                            {
                                this.HasDraw = true;
                                draws++;
                            }
                        }

            DoubleCrossBattleScores[i] = ((float)wins) / (1296) * 100.0f; ;
            return CalculateFitness(wins, 1296, draws);
        }

        public static Game FromRandom()
        {
            var game = new Game();
            for (int diceIndex = 0; diceIndex < Global.NUMBER_OF_DICE; diceIndex++)
            {
                var die = Die.FromRandom();
                game.Add(die);
            }

            game.TestFitness();

            return game;
        }
    }
}