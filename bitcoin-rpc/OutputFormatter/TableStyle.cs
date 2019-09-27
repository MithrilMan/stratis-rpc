namespace StratisRpc.OutputFormatter
{
    public class TableStyle
    {
        public char? Top { get; }
        public char? Right { get; }
        public char? Bottom { get; }
        public char? Left { get; }
        public string Separator { get; }

        public TableStyle(char? top = '-', char? right = null, char? bottom = '-', char? left = '|', string separator = " | ")
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
            Separator = separator;
        }
    }
}