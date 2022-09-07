namespace Program.NonogramSolver
{
    public class CellCombinations
    {
        public double Total { get; set; }
        public double[][] WithCellValues { get; }
        public int NumberOfCells { get; }

        public CellCombinations(int numberOfCells, int numberOfColors)
        {
            NumberOfCells = numberOfCells;
            Total = 0;
            WithCellValues = new double[numberOfCells][];
            for (int i = 0; i < numberOfCells; i++)
            {
                WithCellValues[i] = new double[numberOfColors];
            }
        }

        public void AddCombinationsOfMainChild(CellCombinations childCombinations)
        {
            Total += childCombinations.Total;

            WithCellValues[0][0] += childCombinations.Total;

            for (int cell = 1; cell < NumberOfCells; cell++)
            {
                int numberOfColors = childCombinations.WithCellValues[cell - 1].Length;

                for (int color = 0; color < numberOfColors; color++)
                {
                    WithCellValues[cell][color] += childCombinations.WithCellValues[cell - 1][color];
                }
            }
        }

        public void AddCombinationsOfAdditionalChild(CellCombinations childCombinations, Clue firstClue,
            bool nextClueHasSameColor)
        {
            Total += childCombinations.Total;

            int pCell = 0;

            for (; pCell < firstClue.Length; pCell++)
            {
                WithCellValues[pCell][firstClue.Color] += childCombinations.Total;
            }

            if (nextClueHasSameColor)
            {
                WithCellValues[pCell][0] += childCombinations.Total;
                pCell++;
            }

            for (int cCell = 0; pCell < NumberOfCells; pCell++, cCell++)
            {
                int numberOfColors = childCombinations.WithCellValues[cCell].Length;

                for (int color = 0; color < numberOfColors; color++)
                {
                    WithCellValues[pCell][color] += childCombinations.WithCellValues[cCell][color];
                }
            }
        }
    }
}
