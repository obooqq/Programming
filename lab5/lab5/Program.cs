using System;
using System.IO;
using System.Text.RegularExpressions;

namespace lab5
{
    public class Program
    {
        public const int MinValue = -100;
        public const int MaxValue = 100;
        public const int MaxSize = 20;

        private static Random random = new Random();

        private static string[] testStrings =
        {
            "static void PrintUpper string info 12346: WriteLine ToUpper info, 1234info.",
            ""
        };

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            try
            {
                RunMainMenu();
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("Ввод данных завершён, программа остановлена");
            }
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            int number;
            while (true)
            {
                Console.Write(prompt);
                string line = Console.ReadLine();
                if (line == null)
                    throw new EndOfStreamException();

                if (!int.TryParse(line, out number))
                    ShowError("нужно ввести целое число");
                else if (number < min || number > max)
                    ShowError("число должно быть от {0} до {1}", min, max);
                else
                    return number;
            }
        }

        private static int[] ReadRow(int length, int min = MinValue, int max = MaxValue)
        {
            int[] row = new int[length];
            for (int i = 0; i < length; i++)
                row[i] = ReadInt("  элемент " + (i + 1) + " (от " + min + " до " + max + "): ", min, max);

            return row;
        }

        private static int[,] ReadMatrix(int rows, int columns)
        {
            int[,] matrix = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                Console.WriteLine("Строка " + (i + 1) + ":");
                int[] row = ReadRow(columns);
                for (int j = 0; j < columns; j++)
                    matrix[i, j] = row[j];
            }

            return matrix;
        }

        private static int[][] ReadJagged(int rows)
        {
            int[][] jagged = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                int length = ReadInt("Длина строки " + (i + 1) + " (1.." + MaxSize + "): ", 1, MaxSize);
                jagged[i] = ReadRow(length);
            }

            return jagged;
        }

        private static void ShowError(string message, params object[] args)
        {
            Console.WriteLine("Ошибка: " + string.Format(message, args));
        }

        private static void Print(int[,] matrix)
        {
            if (matrix == null || matrix.Length == 0)
            {
                Console.WriteLine("Массив пуст");
                return;
            }

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                    Console.Write("{0,6}", matrix[i, j]);

                Console.WriteLine();
            }
        }

        private static void Print(int[][] jagged)
        {
            if (jagged == null || jagged.Length == 0)
            {
                Console.WriteLine("Массив пуст");
                return;
            }

            for (int i = 0; i < jagged.Length; i++)
            {
                Console.Write("Строка {0,2}:", i + 1);
                foreach (int value in jagged[i])
                    Console.Write("{0,6}", value);

                Console.WriteLine();
            }
        }

        private static void Print(string text)
        {
            if (IsEmpty(text))
                Console.WriteLine("Строка пуста");
            else
                Console.WriteLine(text);
        }

        public static int[,] CreateRandomMatrix(int rows, int columns, int min = MinValue, int max = MaxValue)
        {
            int[,] matrix = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                    matrix[i, j] = random.Next(min, max + 1);
            }

            return matrix;
        }

        public static int[,] InsertRows(int[,] matrix, int startRow, int[,] newRows)
        {
            if (matrix == null || matrix.Length == 0)
                throw new InvalidOperationException("Массив пуст");

            if (newRows == null || newRows.Length == 0)
                throw new ArgumentException("Нет строк для добавления");

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);
            int count = newRows.GetLength(0);
            if (newRows.GetLength(1) != columns)
                throw new ArgumentException("Число столбцов добавляемых строк не совпадает с числом столбцов массива");

            if (startRow < 1 || startRow > rows + 1)
                throw new ArgumentException("Номер строки должен быть от 1 до " + (rows + 1));

            int[,] result = new int[rows + count, columns];
            for (int i = 0; i < rows + count; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (i < startRow - 1)
                        result[i, j] = matrix[i, j];
                    else if (i < startRow - 1 + count)
                        result[i, j] = newRows[i - (startRow - 1), j];
                    else
                        result[i, j] = matrix[i - count, j];
                }
            }

            return result;
        }

        public static int[][] CreateRandomJagged(int rows, int maxLength = 10)
        {
            int[][] jagged = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                jagged[i] = new int[random.Next(1, maxLength + 1)];
                for (int j = 0; j < jagged[i].Length; j++)
                    jagged[i][j] = random.Next(MinValue, MaxValue + 1);
            }

            return jagged;
        }

        public static void AddRowsToEnd(ref int[][] jagged, int[][] newRows)
        {
            if (newRows == null || newRows.Length == 0)
                throw new ArgumentException("Нет строк для добавления");

            int oldLength = 0;
            if (jagged != null)
                oldLength = jagged.Length;

            Array.Resize(ref jagged, oldLength + newRows.Length);
            for (int i = 0; i < newRows.Length; i++)
                jagged[oldLength + i] = newRows[i];
        }

        public static bool IsEmpty(string text)
        {
            return text == null || text.Trim() == "";
        }

        public static string RemoveWordsStartingWithDigit(string text)
        {
            if (IsEmpty(text))
                return text ?? "";

            string result = Regex.Replace(text, @"[,;:]?\s*\b\d\w*(?=[.!?])", "");
            result = Regex.Replace(result, @"\s*\b\d\w*", "");
            if (!text.StartsWith(" "))
                result = result.TrimStart();

            return result;
        }

        private static void RunMainMenu()
        {
            int[,] matrix = null;
            int[][] jagged = null;
            string text = "";
            int choice;
            do
            {
                Console.WriteLine("\nЛабораторная работа №5, вариант 22");
                Console.WriteLine("1. Двумерный массив");
                Console.WriteLine("2. Рваный массив");
                Console.WriteLine("3. Строка");
                Console.WriteLine("0. Выход");
                choice = ReadInt("Ваш выбор: ", 0, 3);
                switch (choice)
                {
                    case 1:
                        RunMatrixMenu(ref matrix);
                        break;
                    case 2:
                        RunJaggedMenu(ref jagged);
                        break;
                    case 3:
                        RunStringMenu(ref text);
                        break;
                }
            }
            while (choice != 0);
        }

        private static void RunMatrixMenu(ref int[,] matrix)
        {
            int choice;
            do
            {
                Console.WriteLine("\nДвумерный массив");
                Console.WriteLine("1. Создать массив (ввод с клавиатуры)");
                Console.WriteLine("2. Создать массив (случайные числа)");
                Console.WriteLine("3. Печать массива");
                Console.WriteLine("4. Добавить K строк, начиная со строки N");
                Console.WriteLine("0. Назад");
                choice = ReadInt("Ваш выбор: ", 0, 4);
                try
                {
                    switch (choice)
                    {
                        case 1:
                            matrix = CreateMatrix(false);
                            break;
                        case 2:
                            matrix = CreateMatrix(true);
                            break;
                        case 3:
                            Print(matrix);
                            break;
                        case 4:
                            AddRowsToMatrix(ref matrix);
                            break;
                    }
                }
                catch (ArgumentException e)
                {
                    ShowError(e.Message);
                }
                catch (InvalidOperationException e)
                {
                    ShowError(e.Message);
                }
            }
            while (choice != 0);
        }

        private static int[,] CreateMatrix(bool isRandom)
        {
            int rows = ReadInt("Число строк (1.." + MaxSize + "): ", 1, MaxSize);
            int columns = ReadInt("Число столбцов (1.." + MaxSize + "): ", 1, MaxSize);
            int[,] matrix;
            if (isRandom)
                matrix = CreateRandomMatrix(rows, columns);
            else
                matrix = ReadMatrix(rows, columns);

            Console.WriteLine("Массив создан");
            return matrix;
        }

        private static void AddRowsToMatrix(ref int[,] matrix)
        {
            if (matrix == null || matrix.Length == 0)
            {
                Console.WriteLine("Массив пуст, добавление невозможно");
                return;
            }

            int startRow = ReadInt("Номер строки N, с которой добавлять: ", int.MinValue, int.MaxValue);
            int count = ReadInt("Сколько строк K добавить: ", int.MinValue, int.MaxValue);
            if (count < 1 || count > MaxSize)
            {
                ShowError("K должно быть от 1 до {0}", MaxSize);
                return;
            }

            int columns = matrix.GetLength(1);
            int[,] newRows;
            if (ReadInt("Заполнить новые строки: 1 - с клавиатуры, 2 - случайно: ", 1, 2) == 1)
                newRows = ReadMatrix(count, columns);
            else
                newRows = CreateRandomMatrix(count, columns);

            matrix = InsertRows(matrix, startRow, newRows);
            Console.WriteLine("Строки добавлены, массив теперь такой:");
            Print(matrix);
        }

        private static void RunJaggedMenu(ref int[][] jagged)
        {
            int choice;
            do
            {
                Console.WriteLine("\nРваный массив");
                Console.WriteLine("1. Создать массив (ввод с клавиатуры)");
                Console.WriteLine("2. Создать массив (случайные числа)");
                Console.WriteLine("3. Печать массива");
                Console.WriteLine("4. Добавить K строк в конец массива");
                Console.WriteLine("0. Назад");
                choice = ReadInt("Ваш выбор: ", 0, 4);
                try
                {
                    switch (choice)
                    {
                        case 1:
                            jagged = CreateJagged(false);
                            break;
                        case 2:
                            jagged = CreateJagged(true);
                            break;
                        case 3:
                            Print(jagged);
                            break;
                        case 4:
                            AddRowsToJagged(ref jagged);
                            break;
                    }
                }
                catch (ArgumentException e)
                {
                    ShowError(e.Message);
                }
            }
            while (choice != 0);
        }

        private static int[][] CreateJagged(bool isRandom)
        {
            int rows = ReadInt("Число строк (1.." + MaxSize + "): ", 1, MaxSize);
            int[][] jagged;
            if (isRandom)
                jagged = CreateRandomJagged(rows);
            else
                jagged = ReadJagged(rows);

            Console.WriteLine("Массив создан");
            return jagged;
        }

        private static void AddRowsToJagged(ref int[][] jagged)
        {
            int count = ReadInt("Сколько строк K добавить: ", int.MinValue, int.MaxValue);
            if (count < 1 || count > MaxSize)
            {
                ShowError("K должно быть от 1 до {0}", MaxSize);
                return;
            }

            int[][] newRows;
            if (ReadInt("Заполнить новые строки: 1 - с клавиатуры, 2 - случайно: ", 1, 2) == 1)
                newRows = ReadJagged(count);
            else
                newRows = CreateRandomJagged(count);

            AddRowsToEnd(ref jagged, newRows);
            Console.WriteLine("Строки добавлены, массив теперь такой:");
            Print(jagged);
        }

        private static void RunStringMenu(ref string text)
        {
            int choice;
            do
            {
                Console.WriteLine("\nСтрока");
                Console.WriteLine("1. Ввести строку с клавиатуры");
                Console.WriteLine("2. Выбрать строку из тестового массива");
                Console.WriteLine("3. Печать строки");
                Console.WriteLine("4. Удалить слова, начинающиеся с цифры");
                Console.WriteLine("0. Назад");
                choice = ReadInt("Ваш выбор: ", 0, 4);
                switch (choice)
                {
                    case 1:
                        Console.Write("Введите строку: ");
                        text = Console.ReadLine();
                        if (text == null)
                            throw new EndOfStreamException();
                        break;
                    case 2:
                        text = ChooseTestString();
                        break;
                    case 3:
                        Print(text);
                        break;
                    case 4:
                        RemoveDigitWords(text);
                        break;
                }
            }
            while (choice != 0);
        }

        private static string ChooseTestString()
        {
            for (int i = 0; i < testStrings.Length; i++)
            {
                if (testStrings[i] == "")
                    Console.WriteLine((i + 1) + ". <пустая строка>");
                else
                    Console.WriteLine((i + 1) + ". " + testStrings[i]);
            }

            int number = ReadInt("Номер строки: ", 1, testStrings.Length);
            return testStrings[number - 1];
        }

        private static void RemoveDigitWords(string text)
        {
            if (IsEmpty(text))
            {
                Console.WriteLine("Строка пуста, обрабатывать нечего");
                return;
            }

            Console.WriteLine("Исходная строка:");
            Print(text);
            Console.WriteLine("Результат:");
            Print(RemoveWordsStartingWithDigit(text));
        }
    }
}
