namespace nontransitivedicegenerator
{
    using System.Linq;

    internal class Die
    {
        public Die()
        {
            Sides = new int[6];
        }

        public int[] Sides { get; private set; }

        public static Die FromRandom()
        {
            return new Die
            {
                Sides = Enumerable.Range(0, 6).Select(e => Global.random.Next(Global.MIN_SIDE_SCORE, Global.MAX_SIDE_SCORE + 1)).ToArray()
            };
        }
    }
}