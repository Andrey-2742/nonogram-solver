namespace Program.NonogramSolver
{
    public class RowClues
    {
        private readonly Clue[] clues;

        public Clue this[int i]
        {
            get
            {
                return clues[i];
            }
        }
        public int Number { get; }

        public RowClues(int[][] clues)
        {
            this.clues = new Clue[clues.Length];
            for (int i = 0; i < clues.Length; i++)
            {
                int length = clues[i][0];
                int color = clues[i][1];
                this.clues[i] = new Clue(length, color);
            }
            Number = clues.Length;
        }

        public int SliceLength(int offset)
        {
            int totalLength = 0;

            for (int i = offset; i < clues.Length; i++)
            {
                totalLength += clues[i].Length;
                if (i < clues.Length - 1 && clues[i].Color == clues[i + 1].Color)
                {
                    totalLength += 1;
                }
            }

            return totalLength;
        }
    }
}
