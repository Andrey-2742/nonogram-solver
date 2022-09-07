namespace Program.NonogramSolver
{
    public struct Clue
    {
        public int Length { get; }
        public int Color { get; }

        public Clue(int length, int color)
        {
            Length = length;
            Color = color;
        }
    }
}
