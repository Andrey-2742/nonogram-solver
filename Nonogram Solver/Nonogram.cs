using System;
using System.Collections.Generic;
using System.Linq;


namespace Program.NonogramSolver
{
    public class Nonogram
    {
        private NonogramField field;

        public int Width { get; }
        public int Height { get; }
        public int NumberOfColors { get; }
        public NonogramClues Clues { get; }
        public bool IsSolved { get; private set; }


        public Nonogram(int[][][][] clues)
        {
            Height = clues[0].Length;
            Width = clues[1].Length;
            NumberOfColors = GetNumberOfColors(clues);
            Clues = new NonogramClues(clues[0], clues[1]);
            field = new NonogramField(Clues, NumberOfColors);
            IsSolved = false;
        }


        public int GetNumberOfColors(int[][][][] clues)
        {
            HashSet<int> colors = new HashSet<int>();
            for (int direction = 0; direction <= 1; direction++)
            {
                for (int row = 0; row < clues[direction].Length; row++)
                {
                    for (int clue = 0; clue < clues[direction][row].Length; clue++)
                    {
                        colors.Add(clues[direction][row][clue][1]);
                    }
                }
            }
            return colors.Count + 1;
        }


        public int[,] Solve()
        {
            if (IsSolved)
            {
                return GetSolvedField();
            }

            int resultCode = DeriveCellValuesFromClues();
            if (resultCode == 0)
            {
                resultCode = FindRemainingCellValues();
            }
            if (resultCode == -1)
            {
                throw new NonogramIncorrectException("The nonogram has no solution.");
            }

            IsSolved = true;
            return GetSolvedField();
        }


        private int[,] GetSolvedField()
        {
            int[,] solvedField = new int[Height, Width];

            for (int i = 0; i < solvedField.GetLength(0); i++)
            {
                for (int j = 0; j < solvedField.GetLength(1); j++)
                {
                    field[i, j].IsDetermined(out int cellValue);
                    solvedField[i, j] = cellValue;
                }
            }

            return solvedField;
        }


        private int DeriveCellValuesFromClues()
        {
            UpdateRows(Enumerable.Range(0, Height).ToHashSet(), 1, out HashSet<int> _);

            HashSet<int> modifiedRows = Enumerable.Range(0, Width).ToHashSet();
            int traversingDirection = 0;

            while (modifiedRows.Count > 0)
            {
                bool success = UpdateRows(modifiedRows, traversingDirection, out HashSet<int> newModifiedRows);
                if (!success)
                {
                    return -1;
                }
                modifiedRows = newModifiedRows;
                traversingDirection = (traversingDirection + 1) % 2;
            }

            return field.IsSolved() ? 1 : 0;
        }


        private int FindRemainingCellValues()
        {
            NonogramField preservedField = field.Copy();

            Tuple<int, int, int> cell = field.FindMostPossibleCellValue();
            int cellY = cell.Item1;
            int cellX = cell.Item2;
            int cellValue = cell.Item3;

            field[cellY, cellX].SetValue(cellValue);

            int resultCode = DeriveCellValuesFromUpdatedCell(cellY, cellX);

            if (resultCode == 0)
            {
                resultCode = FindRemainingCellValues();
            }
            if (resultCode == 1)
            {
                return 1;
            }
            if (resultCode == -1)
            {
                field = preservedField;
                field[cellY, cellX].PossibleValues[cellValue] = false;

                resultCode = DeriveCellValuesFromUpdatedCell(cellY, cellX);

                if (resultCode == 0)
                {
                    resultCode = FindRemainingCellValues();
                }

                return resultCode;
            }

            throw new NonogramIncorrectException("Incorrect result code.");
        }


        private int DeriveCellValuesFromUpdatedCell(int cellY, int cellX)
        {
            HashSet<int> modifiedRows;
            bool success = UpdateRows(new HashSet<int> { cellY }, 1, out modifiedRows);
            if (!success)
            {
                return -1;
            }
            modifiedRows.Add(cellX);

            int traversingDirection = 0;

            while (modifiedRows.Count > 0)
            {
                success = UpdateRows(modifiedRows, traversingDirection, out HashSet<int> newModifiedRows);
                if (!success)
                {
                    return -1;
                }
                modifiedRows = newModifiedRows;
                traversingDirection = (traversingDirection + 1) % 2;
            }

            return field.IsSolved() ? 1 : 0;
        }


        private bool UpdateRows(IEnumerable<int> rowIndices, int direction, out HashSet<int> updatedCells)
        {
            updatedCells = new HashSet<int>();

            foreach (int rowIndex in rowIndices)
            {
                Row row = direction == 1
                    ? field.GetHorizontalRow(rowIndex)
                    : field.GetVerticalRow(rowIndex);

                bool success = row.Update(out HashSet<int> updatedCellsInCurrentRow);
                if (!success)
                {
                    return false;
                }

                updatedCells.UnionWith(updatedCellsInCurrentRow);
            }

            return true;
        }
    }
}
