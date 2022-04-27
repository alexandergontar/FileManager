using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
namespace FileManager
{
    class Program
    {
        private static int WINDOW_HEIGHT;
        private static int WINDOW_WIDTH;
        private static string currentDir = Directory.GetCurrentDirectory();
        private static List<string> commBuffer;
        private static int lastCommIndex;

        static void Main(string[] args)
        {
            commBuffer = new List<string>();
            // установка параметров страници и текущей папки из файла конфигурации
            WINDOW_HEIGHT = Properties.Settings.Default.W_height;
            WINDOW_WIDTH = Properties.Settings.Default.W_width;

            if (Directory.Exists(Properties.Settings.Default.Path))
            {
                currentDir = Properties.Settings.Default.Path;
            }
            else 
            {
                currentDir = Directory.GetDirectoryRoot(Directory.GetCurrentDirectory());
            }

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Title = "FileManager";

            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);
            
            UI.DrawWindow(0, 0, WINDOW_WIDTH, 18); // окно вывода дерева или текста файла
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8); // окно вывода информации           
            UI.Greeting();  // приветствие при запуске
            UpdateConsole(); // цикл ввода в консоль
            Console.ReadKey(true);
        }
        static void UpdateConsole()
        {
            UI.DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);
            ProcessEnterCommand(WINDOW_WIDTH);
        }
        // формирование строки вводимой команды и вывод истории команд
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = UI.GetCursorPosition();
            StringBuilder command = new StringBuilder();
            char key;
            do
            {
                var keyInfo = Console.ReadKey();
                var k = keyInfo.Key;  // получить клавишу              
                key = keyInfo.KeyChar; // получить символ

                if (key != 8 && key != 13 && key != (int)ConsoleKey.UpArrow && key != (int)ConsoleKey.DownArrow)
                    command.Append(key);

                (int currentLeft, int currentTop) = UI.GetCursorPosition();

                if (currentLeft == width - 2)
                {
                    Console.SetCursorPosition(currentLeft - 1, top);
                    Console.Write(" ");
                    Console.SetCursorPosition(currentLeft - 1, top);
                }
                if (k == ConsoleKey.UpArrow && commBuffer.Count > 0 && lastCommIndex >= 0)
                {
                    string lastCommand = commBuffer[lastCommIndex];
                    if(lastCommIndex > 0) lastCommIndex--;                    
                    Console.SetCursorPosition(left, top);
                    while (Console.CursorLeft<119) { Console.Write(" "); }                   
                    Console.SetCursorPosition(left, top);
                    Console.Write(lastCommand);
                    command = new StringBuilder(lastCommand);
                }
                if (k == ConsoleKey.DownArrow && commBuffer.Count > 1 && lastCommIndex < commBuffer.Count-1)
                {
                    string lastCommand = commBuffer[lastCommIndex];
                    lastCommIndex++;
                    Console.SetCursorPosition(left, top);
                    while (Console.CursorLeft < 119) { Console.Write(" "); }                   
                    Console.SetCursorPosition(left, top);
                    Console.Write(lastCommand);
                    command = new StringBuilder(lastCommand);
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
            while (key != (char)13); // enter key
            commBuffer.Add(command.ToString());
            lastCommIndex = commBuffer.Count - 1;
            ParseCommandString(command.ToString());
        }

        // парсинг введенной командной строки и вызов методов - обработчиков
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
                            Properties.Settings.Default.Path = currentDir;
                            Properties.Settings.Default.Save();
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
                    case "cat":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                        {
                            BL.CatFile(commandParams[1]);
                        }

                        break;
                    case "dir":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                        {
                            BL.DirInfo(commandParams[1]);
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
                            if (BL.CopyDir(commandParams[1], commandParams[2]))
                            {
                              Console.WriteLine($"{commandParams[1]} copied to: {commandParams[2]}");
                            }                           
                    
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
                    case "help":                        
                        if (commandParams.Length == 1)
                        {
                            UI.Help();  
                        }

                        break;

                }
            }
            UpdateConsole(); // возвращение в цикл ввода команд
        }
        
        // отображение дерева в верхнем окне
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();
            BL.GetTree(tree, dir, "", true); // получнеие строки дерева
           // преобразование одной строки в массив строк, отображающих странцу дерева
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
