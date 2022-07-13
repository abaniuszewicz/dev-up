namespace DevUp.Sdk.Sample
{
    public static class CustomConsole
    {
        public static void WriteMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }

        public static void LineBreak()
        {
            Console.WriteLine();
        }

        public static string ReadLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            return Console.ReadLine()!;
        }
    }
}
