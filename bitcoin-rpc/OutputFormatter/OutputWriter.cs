using System;

namespace StratisRpc.OutputFormatter
{
    public class OutputWriter
    {
        public const int ConsoleWidth = 80;

        /// <summary>
        /// Produces a text.
        /// </summary>
        /// <param name="text">The text.</param>
        public delegate void Writer(string text);

        private Writer writer;

        public OutputWriter(Writer writer = null)
        {
            this.writer = writer ?? Console.Write;
        }


        public void Write(string text)
        {
            this.writer(text);
        }


        public void WriteLine(string text = null)
        {
            this.writer(text + '\n');
        }

        public void DrawLine(char? character = '-', int lenght = ConsoleWidth)
        {
            if (character == null)
                return;

            WriteLine(string.Empty.PadRight(lenght, character.Value));
        }
    }
}
