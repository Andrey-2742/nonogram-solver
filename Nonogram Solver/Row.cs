using System.Collections.Generic;


namespace Program.NonogramSolver
{
    public class Row
    {
        private CellCombinations[,] combinationsTable;
        private bool isComplete = false;

        public int Length { get; }
        public Cell[] Cells { get; }
        public RowClues Clues { get; }
        public int NumberOfColors { get; }
        public CellCombinations CombinationsInfo { get; private set; }


        public Row(Cell[] cells, RowClues clues, int numberOfColors)
        {
            Length = cells.Length;
            Cells = new Cell[Length];
            cells.CopyTo(Cells, 0);
            Clues = clues;
            NumberOfColors = numberOfColors;
            CombinationsInfo = new CellCombinations(Length, numberOfColors);
        }


        public bool Update(out HashSet<int> updatedCells)
        {
            updatedCells = new HashSet<int>();

            if (IsComplete())
            {
                return IsCorrect();
            }

            UpdateCellCombinations();

            if (CombinationsInfo.Total == 0)
            {
                return false;
            }

            updatedCells = UpdateCells();
            return true;
        }


        private bool IsComplete()
        {
            if (isComplete)
            {
                return true;
            }

            for (int i = 0; i < Length; i++)
            {
                if (!Cells[i].IsDetermined(out int _))
                {
                    return false;
                }
            }

            isComplete = true;
            return true;
        }


        private bool IsCorrect()
        {
            List<Clue> intervals = new List<Clue>();
            int lastIntervalStart = 0;
            int lastIntervalColor = 0;

            for (int i = 0; i < Length; i++)
            {
                if (Cells[i].IsDetermined(out int value) && value != lastIntervalColor)
                {
                    if (lastIntervalColor != 0)
                    {
                        intervals.Add(new Clue(i - lastIntervalStart, lastIntervalColor));
                    }
                    lastIntervalStart = i;
                    lastIntervalColor = value;
                }
            }
            if (lastIntervalColor != 0)
            {
                intervals.Add(new Clue(Length - lastIntervalStart, lastIntervalColor));
            }

            if (intervals.Count != Clues.Number)
            {
                return false;
            }
            for (int i = 0; i < Clues.Number; i++)
            {
                if (intervals[i].Length != Clues[i].Length
                    || intervals[i].Color != Clues[i].Color)
                {
                    return false;
                }
            }

            return true;
        }


        private HashSet<int> UpdateCells()
        {
            HashSet<int> updatedCells = new HashSet<int>();

            for (int i = 0; i < Length; i++)
            {
                Cell cell = Cells[i];
                if (cell.IsDetermined(out int _))
                {
                    continue;
                }

                bool updated = false;

                for (int c = 0; c < cell.PossibleValues.Length; c++)
                {
                    if (cell.PossibleValues[c] && CombinationsInfo.WithCellValues[i][c] == 0)
                    {
                        cell.PossibleValues[c] = false;
                        updated = true;
                    }
                }
                if (updated)
                {
                    updatedCells.Add(i);
                }
            }

            return updatedCells;
        }


        private void UpdateCellCombinations()
        {
            combinationsTable = new CellCombinations[Length + 1, Clues.Number + 1];
            CombinationsInfo = GetCellCombinationsForRowSlice(Length, Clues.Number);
            combinationsTable = null;
        }


        private CellCombinations GetCellCombinationsForRowSlice(int sliceLength, int cluesTaken)
        {
            if (combinationsTable[sliceLength, cluesTaken] != null)
            {
                return combinationsTable[sliceLength, cluesTaken];
            }

            if (cluesTaken == 0)
            {
                combinationsTable[sliceLength, cluesTaken] = CheckLastRowSlice(sliceLength);
                return combinationsTable[sliceLength, cluesTaken];
            }

            int cluesToSkip = Clues.Number - cluesTaken;
            int minLengthOfClues = Clues.SliceLength(cluesToSkip);
            int freeSpace = sliceLength - minLengthOfClues;

            if (freeSpace < 0)
            {
                combinationsTable[sliceLength, cluesTaken] = new CellCombinations(0, 0);
                return combinationsTable[sliceLength, cluesTaken];
            }

            combinationsTable[sliceLength, cluesTaken] = CheckIntermediateRowSlice(sliceLength, cluesTaken);
            return combinationsTable[sliceLength, cluesTaken];
        }


        private CellCombinations CheckLastRowSlice(int sliceLength)
        {
            int cellOffset = Length - sliceLength;

            for (int i = cellOffset; i < Length; i++)
            {
                if (!Cells[i].PossibleValues[0])
                {
                    return new CellCombinations(0, 0);
                }
            }

            CellCombinations combinations = new CellCombinations(sliceLength, NumberOfColors);
            combinations.Total = 1;
            for (int i = 0; i < sliceLength; i++)
            {
                combinations.WithCellValues[i][0] = 1;
            }

            return combinations;
        }


        private CellCombinations CheckIntermediateRowSlice(int sliceLength, int cluesTaken)
        {
            int clueOffset = Clues.Number - cluesTaken;
            bool nextClueHasSameColor = cluesTaken > 1 && Clues[clueOffset].Color == Clues[clueOffset + 1].Color;

            CellCombinations parentCombinations = new CellCombinations(sliceLength, NumberOfColors);

            if (UseMainChild(sliceLength, cluesTaken, out CellCombinations childSliceMain))
            {
                parentCombinations.AddCombinationsOfMainChild(childSliceMain);
            }
            if (UseAdditionalChild(sliceLength, cluesTaken, nextClueHasSameColor,
                out CellCombinations childSliceAddition))
            {
                parentCombinations.AddCombinationsOfAdditionalChild(
                    childSliceAddition, Clues[clueOffset], nextClueHasSameColor);
            }

            return parentCombinations;
        }


        private bool UseMainChild(int sliceLength, int cluesTaken, out CellCombinations childCombinations)
        {
            int cellOffset = Length - sliceLength;

            if (Cells[cellOffset].PossibleValues[0])
            {
                childCombinations = GetCellCombinationsForRowSlice(sliceLength - 1, cluesTaken);
                return childCombinations.Total > 0;
            }
            else
            {
                childCombinations = new CellCombinations(0, 0);
                return false;
            }
        }


        private bool UseAdditionalChild(int sliceLength, int cluesTaken, bool nextClueHasSameColor,
            out CellCombinations childCombinations)
        {
            int cellOffset = Length - sliceLength;
            int clueOffset = Clues.Number - cluesTaken;

            if (CheckCluePosition(clueOffset, cellOffset))
            {
                int additionalChildOffset = Clues[clueOffset].Length;
                if (nextClueHasSameColor)
                {
                    additionalChildOffset += 1;
                }

                childCombinations = GetCellCombinationsForRowSlice(sliceLength - additionalChildOffset, cluesTaken - 1);
                return childCombinations.Total > 0;
            }
            else
            {
                childCombinations = new CellCombinations(0, 0);
                return false;
            }
        }


        private bool CheckCluePosition(int clueIndex, int firstCoveredCell)
        {
            Clue clue = Clues[clueIndex];

            if (firstCoveredCell != 0)
            {
                Cell previousCell = Cells[firstCoveredCell - 1];
                if (previousCell.IsDetermined(out int color) && color == clue.Color)
                {
                    return false;
                }
            }

            int lastCoveredCell = firstCoveredCell + clue.Length - 1;

            for (int i = firstCoveredCell; i <= lastCoveredCell; i++)
            {
                Cell coveredCell = Cells[i];
                if (!coveredCell.PossibleValues[clue.Color])
                {
                    return false;
                }
            }

            if (lastCoveredCell != Cells.Length - 1)
            {
                Cell nextCell = Cells[lastCoveredCell + 1];
                if (nextCell.IsDetermined(out int color) && color == clue.Color)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
