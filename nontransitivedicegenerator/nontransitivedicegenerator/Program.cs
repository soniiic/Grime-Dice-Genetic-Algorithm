using System;

namespace nontransitivedicegenerator
{
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var world = InitialiseGenerationZero(Global.POPULATION_CAP);

                for (int i = 0; i < Global.GENERATION_ITERATIONS; i++)
                {
                    //Console.WriteLine("Iteration: " + i);

                    world = NextGeneration(world);

                    var topGame = world.First(g => g.Fitness == world.Max(g2 => g2.Fitness));

                    Console.WriteLine($"fitness: {topGame.Fitness}, SScore: {string.Join(", ", topGame.SingleBattleScores)}, DScore: {string.Join(", ", topGame.DoubleBattleScores)}, CScore: {string.Join(", ", topGame.CrossBattleScores)}, DCScore: { string.Join(", ", topGame.DoubleCrossBattleScores)}");

                    var allOver50 = world.FirstOrDefault(g => g.SingleBattleScores.All(s => s > 50) && g.DoubleBattleScores.All(s => s > 50) &&
                                                                   g.CrossBattleScores.All(s => s > 50) && g.DoubleCrossBattleScores.All(s => s > 50));
                    if (allOver50 != null)
                    {
                        for (var index = 0; index < allOver50.Count; index++)
                        {
                            var die = allOver50[index];
                            Console.WriteLine($"Die: {index} Sides: {string.Join(", ", die.Sides)}");
                        }
                        goto end_of_loop;
                    }
                }
            }
            end_of_loop: { }
            Console.WriteLine("Solution Found!");
            Console.ReadLine();
        }

        private static World NextGeneration(World world)
        {
            var newWorld = new World();
            var orderedFitness = world.OrderBy(g => g.Fitness).ToList();

            // Add the strong
            var fitnessOfTop10Percent = orderedFitness.ElementAt((int)(Global.POPULATION_CAP * 0.95)).Fitness;
            var top5Percent = orderedFitness.Where(g => g.Fitness >= fitnessOfTop10Percent).ToList();
            newWorld.AddRange(top5Percent);

            while (newWorld.Count < Global.POPULATION_CAP)
            {
                var baby = GetBaby(world);
                newWorld.Add(baby);
            }

            return newWorld;
        }

        private static Game GetBaby(World world)
        {
            Game baby;
            var random = Global.random.NextDouble();
            if (random > 0.8)
            {
                // Mutate
                var original = world.ElementAt(Global.random.Next(world.Count));
                baby = Mutate(original);
            }
            else if (random > 0.75)
            {
                baby = Game.FromRandom();
            }
            else
            {
                // Crossover
                var mother = world.ElementAt(Global.random.Next(world.Count));
                var father = world.ElementAt(Global.random.Next(world.Count));

                baby = Breed(mother, father);
            }

            return baby;
        }

        private static Game Mutate(Game original)
        {
            var baby = new Game();
            for (int i = 0; i < Global.NUMBER_OF_DICE; i++)
            {
                var die = new Die();
                for (int j = 0; j < 6; j++)
                {
                    if (Global.random.NextDouble() < Global.MUTATION_RATE)
                    {
                        die.Sides[j] = Global.random.Next(Global.MIN_SIDE_SCORE, Global.MAX_SIDE_SCORE + 1);
                    }
                    else
                    {
                        die.Sides[j] = original[i].Sides[j];
                    }
                }

                baby.Add(die);
            }

            baby.TestFitness();

            return baby;
        }

        private static Game Breed(Game mother, Game father)
        {
            var baby = new Game();
            for (int i = 0; i < Global.NUMBER_OF_DICE; i++)
            {
                var die = new Die();
                for (int j = 0; j < 6; j++)
                {
                    if (Global.random.NextDouble() < 0.02)
                    {
                        die.Sides[j] = Global.random.Next(Global.MIN_SIDE_SCORE, Global.MAX_SIDE_SCORE + 1);
                    }
                    else
                    {
                        die.Sides[j] = Global.random.Next(2) == 1 ? mother[i].Sides[j] : father[i].Sides[j];
                    }
                }
                baby.Add(die);
            }

            baby.TestFitness();

            return baby;
        }

        private static World InitialiseGenerationZero(int populationCap)
        {
            var world = new World();

            for (int pop = 0; pop < populationCap; pop++)
            {
                var game = Game.FromRandom();

                world.Add(game);
            }

            return world;
        }
    }
}
