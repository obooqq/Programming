using System;

namespace Lab3
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const double s = 0.1;
            const double e = 1.0;
            const int n = 35;
            const int points = 10;
            const double epsilon = 0.0001;

            PrintTable(s, e, n, points, epsilon);
        }

        private static void PrintTable(double s, double e, int n, int points, double epsilon)
        {
            double step = (e - s) / (points - 1);
            Console.WriteLine("Вычисление функции y = (1-x^2/2)*cos(x)-(x/2)*sin(x)");
            Console.WriteLine($"n = {n}, количество точек = {points}, точность = {epsilon}");
            Console.WriteLine();

            for (int i = 0; i < points; i++)
            {
                double x = s + step * i;
                double sn = GetSum(x, n);
                double se = CalcSumByEpsilon(x, epsilon);
                double y = (1 - x * x / 2) * Math.Cos(x) - x / 2 * Math.Sin(x);

                PrintRow(x, sn, se, y);
            }
        }

        private static void PrintRow(double x, double sn, double se, double y)
        {
            Console.WriteLine($"X={x,-10:F2} SN={sn,-14:F8} SE={se,-14:F8} Y={y,-14:F8}");
        }
        
        private static double GetSum(double x, int n)
        {
            double factor = 1;
            double sum = factor;

            for (int i = 1; i <= n; i++)
            {
                factor *= -(x * x) / ((2 * i - 1) * (2 * i));
                sum += (2 * i * i + 1) * factor;
            }

            return sum;
        }
        
        private static double CalcSumByEpsilon(double x, double epsilon)
        {
            double factor = 1;
            double term = factor;
            double sum = 0;
            int i = 0;

            while (Math.Abs(term) >= epsilon)
            {
                sum += term;
                i++;
                factor *= -(x * x) / ((2 * i - 1) * (2 * i));
                term = (2 * i * i + 1) * factor;
            }

            return sum;
        }
    }
}
