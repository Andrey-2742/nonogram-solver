using System;


namespace Program.NonogramSolver
{
    public class NonogramField
    {
        private Cell[,] cells;
        private Row[][] rows = new Row[2][];
        private int numberOfColors;
        private NonogramClues clues;

        public Cell this[int i, int j]
        {
            get
            {
                return cells[i, j];
            }
        }


        public NonogramField(NonogramClues clues, int numberOfColors)
        {
            int height = clues.Horizontal.Count;
            int width = clues.Vertical.Count;

            cells = new Cell[height, width];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i / width, i % width] = new Cell(numberOfColors);
            }

            rows[0] = new Row[height];
            rows[1] = new Row[width];

            for (int i = 0; i < height; i++)
            {
                Cell[] rowCells = new Cell[width];
                for (int j = 0; j < width; j++)
                {
                    rowCells[j] = cells[i, j];
                }
                rows[0][i] = new Row(rowCells, clues.Horizontal[i], numberOfColors);
            }

            for (int i = 0; i < width; i++)
            {
                Cell[] rowCells = new Cell[height];
                for (int j = 0; j < height; j++)
                {
                    rowCells[j] = cells[j, i];
                }
                rows[1][i] = new Row(rowCells, clues.Vertical[i], numberOfColors);
            }

            this.numberOfColors = numberOfColors;
            this.clues = clues;
        }


        public NonogramField Copy()
        {
            NonogramField copiedField = new NonogramField(clues, numberOfColors);

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j].PossibleValues.CopyTo(copiedField.cells[i, j].PossibleValues, 0);
                }
            }

            for (int direction = 0; direction <= 1; direction++)
            {
                for (int i = 0; i < cells.GetLength(direction); i++)
                {
                    CellCombinations combinations = rows[direction][i].CombinationsInfo;
                    CellCombinations copiedCombinations = copiedField.rows[direction][i].CombinationsInfo;

                    for (int cell = 0; cell < combinations.WithCellValues.Length; cell++)
                    {
                        combinations.WithCellValues[cell].CopyTo(copiedCombinations.WithCellValues[cell], 0);
                        copiedCombinations.Total = combinations.Total;
                    }
                }
            }

            return copiedField;
        }


        public Row GetHorizontalRow(int rowIndex)
        {
            return rows[0][rowIndex];
        }


        public Row GetVerticalRow(int rowIndex)
        {
            return rows[1][rowIndex];
        }


        public Tuple<int, int, int> FindMostPossibleCellValue()
        {
            Tuple<int, int, int> mostPossibleValue = new Tuple<int, int, int>(-1, -1, -1);
            double maxProbability = 0;

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].IsDetermined(out int _))
                    {
                        continue;
                    }
                    for (int c = 0; c < cells[i, j].PossibleValues.Length; c++)
                    {
                        double Nxh = rows[0][i].CombinationsInfo.WithCellValues[j][c];
                        double Nxv = rows[1][j].CombinationsInfo.WithCellValues[i][c];
                        double Noh = rows[0][i].CombinationsInfo.Total - Nxh;
                        double Nov = rows[1][j].CombinationsInfo.Total - Nxv;

                        double Nx = Nxh * Nxv;
                        double No = Noh * Nov;
                        double N = Nx + No;
                        double P = Nx / N;

                        if (P > maxProbability)
                        {
                            maxProbability = P;
                            mostPossibleValue = new Tuple<int, int, int>(i, j, c);
                        }
                    }
                }
            }

            return mostPossibleValue;
        }


        public bool IsSolved()
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (!cells[i, j].IsDetermined(out int _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
