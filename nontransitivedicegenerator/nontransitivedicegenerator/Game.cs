namespace nontransitivedicegenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Game : List<Die>
    {
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
                var doubleBattleScore = TestDoubleBattleScore(i);
                var crossBattleScore = TestCrossBattleScore(i);
                var crossDoubleBattleScore = TestDoubleCrossBattleScore(i);
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
                        draws++;
                    }
                }
            }

            var battleScore = (float)wins / (36 - draws) * 100.0f;
            SingleBattleScores[i] = battleScore;

            battleScore += (Math.Min(battleScore, 50)) * 2;
            return battleScore;
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
                    draws++;
                }
            }

            //for (int battleNumber = 0; battleNumber < Global.ROLLS_TO_TEST_FITNESS; battleNumber++)
            //{
            //    if (die1.Sides[random.Next(6)] + die1.Sides[random.Next(6)] > die2.Sides[random.Next(6)] + die2.Sides[random.Next(6)])
            //    {
            //        wins++;
            //    }
            //}

            var battleScore = (float)wins / (1296 - draws) * 100.0f;
            DoubleBattleScores[i] = battleScore;

            battleScore += (Math.Min(battleScore, 50)) * 2;
            return battleScore;
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
                        draws++;
                    }
                }
            }

            var battleScore = (float)wins / (36 - draws) * 100.0f;
            CrossBattleScores[i] = battleScore;
            battleScore += (Math.Min(battleScore, 50)) * 2;
            return battleScore;
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
                    draws++;
                }
            }

            //for (int battleNumber = 0; battleNumber < Global.ROLLS_TO_TEST_FITNESS; battleNumber++)
            //{
            //    if (die1[random.Next(6)] + die1[random.Next(6)] > die2[random.Next(6)] + die2[random.Next(6)])
            //    {
            //        wins++;
            //    }
            //}

            var battleScore = (float)wins / (1296 - draws) * 100.0f;
            DoubleCrossBattleScores[i] = battleScore;

            battleScore += (Math.Min(battleScore, 50)) * 2;
            return battleScore;
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