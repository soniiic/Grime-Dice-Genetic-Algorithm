﻿using System;

namespace nontransitivedicegenerator
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using ILGPU;
    using ILGPU.Runtime.CPU;
    using ILGPU.Runtime.Cuda;

    class Program
    {
        static void Main(string[] args)
        {
            using (Process p = Process.GetCurrentProcess())
                p.PriorityClass = ProcessPriorityClass.Idle;

            GeneticAlgorithm();
            //BruteForce();
        }

        private static void BruteForce()
        {
            int[] sides = Enumerable.Range(0, 30).Select(_ => 0).ToArray();

            var game = new Game();
            game.Add(new Die());
            game.Add(new Die());
            game.Add(new Die());
            game.Add(new Die());
            game.Add(new Die());
            var sideFocus = 0;

            while (true)
            {
                if (sides.All(s => s == 6))
                {
                    break;
                }

                bool found = false;
                for (int i = 0; i < sideFocus; i++)
                {
                    if (sides[i] == 6)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            sides[j] = 0;
                        }

                        sides[i] = 0;
                        sides[i + 1]++;
                        found = true;
                        break;
                    }

                    sides[i]++;
                    found = true;
                    break;
                }

                if (!found)
                {
                    sides[sideFocus]++;
                    if (sides[sideFocus] == 7)
                    {
                        sides[sideFocus] = 0;
                        sideFocus++;
                        sides = Enumerable.Range(0, 30).Select(_ => 0).ToArray();
                        sides[sideFocus]++;
                    }
                }

                for (int i = 0; i < 30; i++)
                {
                    game[i / 6].Sides[i % 6] = sides[i];
                }

                game.TestFitness();

                Console.WriteLine("f: " + game.Fitness + " s: " + string.Join("", sides));
            }

            Console.WriteLine("Done");
        }

        private static void GeneticAlgorithm()
        {
            while (true)
            {
                var world = InitialiseGenerationZero(Global.POPULATION_CAP);
                float fitness = Single.NaN;

                for (int i = 0; i < Global.GENERATION_ITERATIONS; i++)
                {
                    world = NextGeneration(world);

                    if (i % 50 == 0)
                    {
                        var topFitness = world.Max(g2 => g2.Fitness);
                        if (topFitness != fitness)
                        {
                            i = Math.Min(i, (int) (Global.GENERATION_ITERATIONS * 0.8));
                            fitness = topFitness;
                        }
                        var topGame = world.First(g => g.Fitness == topFitness);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"fitness: {topGame.Fitness}, hasDraw: {topGame.HasDraw}");

                        WriteScores("SScore: ", topGame.SingleBattleScores);
                        WriteScores("DScore: ", topGame.DoubleBattleScores);
                        WriteScores("CScore: ", topGame.CrossBattleScores);
                        WriteScores("DCScore: ", topGame.DoubleCrossBattleScores);

                        Console.WriteLine();
                    }

                    var successfulGame = world.FirstOrDefault(g => !g.HasDraw && g.Fitness > 1000);
                    if (successfulGame != null)
                    {
                        for (var index = 0; index < successfulGame.Count; index++)
                        {
                            var die = successfulGame[index];
                            Console.WriteLine($"Die: {index} Sides: {string.Join(", ", die.Sides)}");
                        }

                        goto end_of_loop;
                    }
                }

                var closeGame = world.Where(g => !g.HasDraw && g.Fitness > 998).OrderByDescending(g => g.Fitness).FirstOrDefault();

                if (closeGame != null)
                {
                    var lines = new List<string>();
                    lines.Add($"fitness: {closeGame.Fitness}, hasDraw: {closeGame.HasDraw}");
                    for (var index = 0; index < closeGame.Count; index++)
                    {
                        var die = closeGame[index];
                        lines.Add($"Die: {index} Sides: {string.Join(", ", die.Sides)}");
                    }
                    lines.Add(string.Empty);

                    File.AppendAllLines(@"C:\Temp\dicefinds.txt", lines);
                }
            }

            end_of_loop: { }
            Console.WriteLine("Solution Found!");
            Console.ReadLine();
        }

        private static void WriteScores(string name, float[] scores)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(name);
            for (var index = 0; index < scores.Length; index++)
            {
                var score = scores[index];
                Console.ForegroundColor = score > 50 ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write(score.ToString("N2") + (index == scores.Length - 1 ? "" : ", "));
            }
        }

        private static World NextGeneration(World world)
        {
            var newWorld = new World();

            // Add the strong
            var top5Percent = world.OrderByDescending(g => g.Fitness).Take((int)(Global.POPULATION_CAP * 0.05)).ToList();
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
