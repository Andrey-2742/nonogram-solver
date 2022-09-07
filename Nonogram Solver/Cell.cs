namespace Program.NonogramSolver
{
    public class Cell
    {
        private int determinedValue = -1;

        public bool[] PossibleValues { get; }

        public bool IsDetermined(out int cellValue)
        {
            if (determinedValue != -1)
            {
                cellValue = determinedValue;
                return true;
            }

            cellValue = -1;
            int numberOfPossibleValues = 0;

            for (int i = 0; i < PossibleValues.Length; i++)
            {
                if (PossibleValues[i])
                {
                    numberOfPossibleValues++;
                    cellValue = i;
                }
                if (numberOfPossibleValues > 1)
                {
                    cellValue = -1;
                    return false;
                }
            }
            return true;
        }

        public void SetValue(int cellValue)
        {
            for (int i = 0; i < PossibleValues.Length; i++)
            {
                PossibleValues[i] = false;
            }
            PossibleValues[cellValue] = true;
        }

        public Cell(int numberOfColors)
        {
            PossibleValues = new bool[numberOfColors];
            for (int i = 0; i < numberOfColors; i++)
            {
                PossibleValues[i] = true;
            }
        }
    }
}
