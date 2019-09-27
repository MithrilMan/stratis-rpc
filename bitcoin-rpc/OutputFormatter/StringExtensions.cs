﻿using System;

namespace StratisRpc.OutputFormatter
{
    public static class StringExtensions
    {
        public static string Align(this string text, int totalWidth = OutputWriter.ConsoleWidth, char paddingCharacter = ' ')
        {
            return totalWidth > 0 ? text.PadRight(totalWidth, paddingCharacter) : text.PadLeft(Math.Abs(totalWidth), paddingCharacter);
        }

        public static string AlignLeft(this string text, int totalWidth = OutputWriter.ConsoleWidth, char paddingCharacter = ' ')
        {
            return text.PadRight(Math.Abs(totalWidth), paddingCharacter);
        }

        public static string AlignRight(this string text, int totalWidth = OutputWriter.ConsoleWidth, char paddingCharacter = ' ')
        {
            return text.PadLeft(Math.Abs(totalWidth), paddingCharacter);
        }

        public static string Center(this string text, int totalWidth = OutputWriter.ConsoleWidth, char paddingCharacter = ' ')
        {
            return text.PadLeft((totalWidth + text.Length) / 2, paddingCharacter);
        }
    }
}
