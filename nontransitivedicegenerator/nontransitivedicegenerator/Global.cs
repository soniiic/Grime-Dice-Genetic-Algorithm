namespace nontransitivedicegenerator
{
    using System;

    public static class Global
    {
        public const int NUMBER_OF_DICE = 5;
        public const int POPULATION_CAP = 100;
        public const int MIN_SIDE_SCORE = 0;
        public const int MAX_SIDE_SCORE = 9;
        public const int GENERATION_ITERATIONS = 10000;
        public const float MUTATION_RATE = 0.075f;
        public static Random random = new Random();
    }
}