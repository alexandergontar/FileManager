using System;
using System.IO;
using System.Text;

namespace FileManager
{
    public static class UI
    {
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDTH = 120;

        public static void DrawWindow(int x, int y, int width, int height)
        {
            Console.SetCursorPosition(x, y);
            // header - шапка
            Console.Write("╔");
            for (int i = 0; i < width - 2; i++) // 2 - уголки
                Console.Write("═");
            Console.Write("╗");

            Console.SetCursorPosition(x, y + 1);
            for (int i = 0; i < height - 2; i++)
            {
                Console.Write("║");
                for (int j = x + 1; j < x + width - 1; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("║");
            }

            // footer - подвал
            Console.Write("╚");
            for (int i = 0; i < width - 2; i++) // 2 - уголки
                Console.Write("═");
            Console.Write("╝");
            Console.SetCursorPosition(x, y);
        }

        public static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            DrawWindow(x, y, width, height);
            Console.SetCursorPosition(x + 1, y + height / 2);
            Console.Write($"{dir}>");
        }

        public static (int left, int top) GetCursorPosition()
        {
            // Кортежи
            // https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/builtin-types/value-tuples
            return (Console.CursorLeft, Console.CursorTop);
        }

        public static void Help() 
        {
            UI.DrawWindow(0, 0, WINDOW_WIDTH, 18);
            (int x, int y) = UI.GetCursorPosition();
            Console.SetCursorPosition(x+1, y+1);
            Console.Write("Help:");
            Console.SetCursorPosition(x + 1, y + 2);
            Console.Write("Help:");
        }
        public static void Greeting()
        {
            //UI.DrawWindow(0, 0, WINDOW_WIDTH, 18);
            Console.SetCursorPosition(1, 1);
            Console.WriteLine("Введите help в коммандной строке  для справки а командам");
        }

    }
}
