using System;
using System.Collections.ObjectModel;

namespace Program.NonogramSolver
{
    public class NonogramClues
    {
        private readonly RowClues[][] clues = new RowClues[2][];

        public ReadOnlyCollection<RowClues> Horizontal
        {
            get
            {
                return Array.AsReadOnly(clues[0]);
            }
        }
        public ReadOnlyCollection<RowClues> Vertical
        {
            get
            {
                return Array.AsReadOnly(clues[1]);
            }
        }

        public NonogramClues(int[][][] horizontal, int[][][] vertical)
        {
            clues[0] = new RowClues[horizontal.Length];
            clues[1] = new RowClues[vertical.Length];

            for (int i = 0; i < horizontal.Length; i++)
            {
                clues[0][i] = new RowClues(horizontal[i]);
            }

            for (int i = 0; i < vertical.Length; i++)
            {
                clues[1][i] = new RowClues(vertical[i]);
            }
        }
    }
}
