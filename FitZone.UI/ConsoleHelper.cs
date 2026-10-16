using System;
using System.Text;

namespace FitZone.UI
{
    public static class ConsoleHelper
    {
        public static void Header(string title)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine();
            Console.WriteLine(
                "╔══════════════════════════════════════════════════╗"
            );

            Console.Write("║  ");

            Console.Write(
                title.PadRight(46)
            );

            Console.WriteLine("║");

            Console.WriteLine(
                "╚══════════════════════════════════════════════════╝"
            );

            Console.ResetColor();

            Console.WriteLine();
        }

        public static void Success(string message)
        {
            Console.ForegroundColor =
                ConsoleColor.Green;

            Console.WriteLine(
                $"  ✓ {message}"
            );

            Console.ResetColor();
        }

        public static void Error(string message)
        {
            Console.ForegroundColor =
                ConsoleColor.Red;

            Console.WriteLine(
                $"  ✗ {message}"
            );

            Console.ResetColor();
        }

        public static void Info(string message)
        {
            Console.ForegroundColor =
                ConsoleColor.Yellow;

            Console.WriteLine(
                $"  • {message}"
            );

            Console.ResetColor();
        }

        public static void Title(string message)
        {
            Console.ForegroundColor =
                ConsoleColor.Magenta;

            Console.WriteLine(
                $"  {message}"
            );

            Console.ResetColor();
        }

        public static void MenuOption(
            string number,
            string text)
        {
            Console.ForegroundColor =
                ConsoleColor.Cyan;

            Console.Write(
                $"  [{number}] "
            );

            Console.ResetColor();

            Console.WriteLine(text);
        }

        public static string ReadPassword()
        {
            var password =
                new StringBuilder();

            while (true)
            {
                ConsoleKeyInfo key =
                    Console.ReadKey(
                        intercept: true
                    );

                if (key.Key ==
                    ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }

                if (key.Key ==
                    ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Length--;

                        Console.Write(
                            "\b \b"
                        );
                    }

                    continue;
                }

                if (!char.IsControl(
                        key.KeyChar))
                {
                    password.Append(
                        key.KeyChar
                    );

                    Console.Write("*");
                }
            }

            return password.ToString();
        }

        public static void Pause()
        {
            Console.ForegroundColor =
                ConsoleColor.DarkGray;

            Console.WriteLine();

            Console.Write(
                "  Press ENTER to continue..."
            );

            Console.ResetColor();

            Console.ReadLine();
        }
    }
}