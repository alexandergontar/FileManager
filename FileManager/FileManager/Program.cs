using System;
using System.IO;
using System.Text;

namespace FileManager
{
    class Program
    {
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDTH = 120;
        private static string currentDir = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager Study";

            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);

            UI.DrawWindow(0, 0, WINDOW_WIDTH, 18);
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            //UI.DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
            UpdateConsole();
            Console.ReadKey(true);
        }
        static void UpdateConsole()
        {
            UI.DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
            ProcessEnterCommand(WINDOW_WIDTH);
        }
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = UI.GetCursorPosition();
            StringBuilder command = new StringBuilder();
            char key;
            do
            {
                key = Console.ReadKey().KeyChar;

                if (key != 8 && key != 13)
                    command.Append(key);

                (int currentLeft, int currentTop) = UI.GetCursorPosition();

                if (currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }
                if (key == (char)8/*ConsoleKey.Backspace*/)
                {
                    if (command.Length > 0)
                        command.Remove(command.Length - 1, 1);
                    if (currentLeft >= left)
                    {
                        Console.SetCursorPosition(currentLeft, top);
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        Console.SetCursorPosition(left, top);
                    }
                }
            }
            while (key != (char)13);
            ParseCommandString(command.ToString());
        }

        static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');
            if (commandParams.Length > 0)
            {
                switch (commandParams[0])
                {
                    case "cd":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            currentDir = commandParams[1];
                        }

                        break;
                    case "ls":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), n);
                            }
                            else
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);
                            }
                        }
                        break;
                    case "delfile":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            BL.DeleteFile(commandParams[1]);
                        }

                        break;
                    case "file":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            BL.FileInfo(commandParams[1]);
                        }

                        break;
                    case "cp":
                        if (commandParams.Length > 2 && File.Exists(commandParams[1]))
                        {
                            BL.CopyFile(commandParams[1], commandParams[2]);
                        }

                        break;
                    case "cpdir":
                        if (commandParams.Length > 2 && Directory.Exists(commandParams[1]))
                        {
                            BL.CopyDir(commandParams[1], commandParams[2]);
                        }

                        break;
                    case "deldir":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            BL.DelDir(commandParams[1], false);
                        }
                        if (commandParams.Length > 2 && commandParams[1] == "-r" && Directory.Exists(commandParams[2]))
                        {
                            BL.DelDir(commandParams[2], true);
                        }

                        break;
                }
            }
            UpdateConsole();
        }
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            BL.GetTree(tree, dir, "", true);
            UI.DrawWindow(0, 0, WINDOW_WIDTH, 18);
            (int currentLeft, int currentTop) = UI.GetCursorPosition();
            int pageLines = 16;
            string[] lines = tree.ToString().Split(new char[] { '\n' });
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;
            if (page > pageTotal)
                page = pageTotal;

            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)
                {
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);
                }
            }

            //footer
            string footer = $"╡ {page} of {pageTotal} ╞";
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - footer.Length / 2, 17);
            Console.WriteLine(footer);
        }
    }
}
