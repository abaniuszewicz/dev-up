namespace DevUp.Sdk.Sample
{
    public static class CustomConsole
    {
        public static void WriteMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            ResetColor();
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            ResetColor();
        }

        public static string ReadLine()
        {
            return Console.ReadLine()!;
        }

        public static void LineBreak()
        {
            Console.WriteLine();
        }

        private static void ResetColor()
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
