using System;

namespace AxxenyUtilities.Helpers
{
    public class ConsoleHelper
    {
        public static void CleanCurrentLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }

        public static void WaitForEnterPressed()
        {
            Console.WriteLine("Press enter to continue");
            Console.ReadLine();
        }
    }
}