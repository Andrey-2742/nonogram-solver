using System;

namespace Program.NonogramSolver
{
    public class NonogramIncorrectException : Exception
    {
        public NonogramIncorrectException(string message) : base(message)
        {
        }
    }
}
