using System;
using System.IO;
using System.Text;
namespace FileManager
{
    public static class BL
    {
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDTH = 120;

        //формирование строки дерева
        public static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);
            if (lastDirectory)
            {
                tree.Append("└─");
                indent += "  ";
            }
            else
            {
                tree.Append("├─");
                indent += "│ ";
            }

            tree.Append($"{dir.Name}\n"); //<---------------------- ПЕРЕХОД НА СЛЕД СТРОКУ

            //TODO: Добавляем отображение файлов
            FileInfo[] subFiles = dir.GetFiles();
            for (int i = 0; i < subFiles.Length; i++)
            {
                if (i == subFiles.Length - 1)
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }

            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
        }

        //удаление файла
        public static void DeleteFile(string path)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                File.Delete(path);
                Console.WriteLine(path);
                Console.SetCursorPosition(1, 20);
                Console.WriteLine("Deleted");
            }
            catch (Exception ex)
            {
                Console.SetCursorPosition(1, 20);
                Console.WriteLine(ex.Message);
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }

        }

        //вывод информации о файле
        public static void FileInfo(string path)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                FileInfo fi = new FileInfo(path);
                Console.WriteLine("Path: " + fi.FullName);
                Console.SetCursorPosition(1, 20);
                Console.WriteLine("Size(bytes): " + fi.Length.ToString());
                Console.SetCursorPosition(1, 21);
                Console.WriteLine("Created: " + fi.CreationTime);
                Console.SetCursorPosition(1, 22);
                Console.WriteLine("Last Modified: " + fi.LastWriteTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }
        }

        //вывод текстового содержимого файла
        public static void CatFile(string path)
        {
            UI.DrawWindow(0, 0, WINDOW_WIDTH, 18);
            (int x, int y) = UI.GetCursorPosition();
            int count = 0;
            int firstLine = y;
            int n = Properties.Settings.Default.LinesNumber;
            try
            {
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (count == n)
                    {
                        Console.SetCursorPosition(x + 1, ++y);
                        Console.Write("======Для продолжения нажмите любую клавишу=====");
                        count = 0;
                        y = firstLine;
                        Console.ReadKey(true);
                    }
                    Console.SetCursorPosition(x + 1, ++y);
                    Console.Write(lines[i]);
                    count++;
                }
            }
            catch (Exception ex)
            {
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }
        }

        //вывод информации о каталоге
        public static void DirInfo(string path)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                string[] files = Directory.GetFiles(path);
                string[] folders = Directory.GetDirectories(path);
                DateTime dateTime = Directory.GetCreationTime(path);
                Console.WriteLine("Папка: " + path);
                Console.SetCursorPosition(1, 20);
                Console.WriteLine($"Создан: {dateTime} Файлов: {files.Length} Папок: {folders.Length}");
            }
            catch (Exception ex)
            {
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }

        }

        //копирование файла
        public static void CopyFile(string source, string target)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                File.Copy(source, target);
                Console.WriteLine($"{source} copied to: {target}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }

        }

        //копирование каталогов и подкаталогов с рекурсией
        public static bool CopyDir(string sourceFolder, string destFolder)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyDir(folder, dest);
                }
                return true;// Console.WriteLine($"{sourceFolder} copied to: {destFolder}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                BL.ErrorLog("random_name_exception.txt", ex.Message);
                return false;

            }

        }

        // удаление каталогов/подкаталогов с рекурсией и без
        public static void DelDir(string path, bool recurs)
        {
            UI.DrawWindow(0, 18, WINDOW_WIDTH, 8);
            Console.SetCursorPosition(1, 19);
            try
            {
                Directory.Delete(path, true);
                Console.WriteLine($" Directory {path} deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                BL.ErrorLog("random_name_exception.txt", ex.Message);
            }

        }

        // логгирование исключений в текстовый файл
        public static void ErrorLog(string logFile, string errMessage)
        {
            try
            {
                if (!Directory.Exists(@"errors"))
                {
                    Directory.CreateDirectory(@"errors");
                }
                string logFilePath = Path.Combine(@"errors", logFile);
                if (!File.Exists(logFilePath))
                {
                    File.Create(logFilePath);
                }
                using (StreamWriter sw = File.AppendText(logFilePath)) 
                {
                 sw.WriteLine(DateTime.Now.ToString()+": "+ errMessage);
                }
                
            }
            catch (Exception)
            {
                
            }


        }


    }
}
