using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekat2
{
    public class Matrica
    {
        public static List<int> Indeksi = new List<int>();

        public static int[,] KreirajMatricu()
        {
            Random random = new Random(71);
            int n = 15;
            int k = 6;
            int wr = 5;
            int wc = 3;
            int[,] H = new int[n - k, n];

            for (int i = 0; i < wc; i++)
            {
                int pocetak = i * wr;
                int kraj = pocetak + wr;
                for (int j = pocetak; j < kraj; j++)
                    H[i, j] = 1;
            }

            for (int j = 0; j < n; j++)
            {
                int rbr = random.Next(3) + wc;
                for (int i = wc; i < wc * 2; i++)
                    H[i, j] = (rbr == i) ? 1 : 0;
            }

            for (int j = 0; j < n; j++)
            {
                int rbr = random.Next(3) + wc * 2;
                for (int i = wc * 2; i < wc * 3; i++)
                    H[i, j] = (rbr == i) ? 1 : 0;
            }

            return H;
        }

        public static void StampajMatricu(int[,] H)
        {
            for (int i = 0; i < H.GetLength(0); i++)
            {
                for (int j = 0; j < H.GetLength(1); j++)
                    Console.Write(H[i, j] + " ");
                Console.WriteLine();
            }
        }

        public static List<int[]> GenerisiBinarneKomb()
        {
            int n = 15;
            List<int[]> binaryCombinations = new List<int[]>();
            for (int i = 0; i < (1 << n); i++)
            {
                int[] combination = new int[n];
                for (int j = 0; j < n; j++)
                    combination[j] = (i >> j) & 1;
                binaryCombinations.Add(combination);
            }
            return binaryCombinations;
        }

        private static int[,] Transponovanje(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] result = new int[cols, rows];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    result[j, i] = matrix[i, j];
            return result;
        }

        public static int[,] RacunajSindrom(int[,] H, int[,] e)
        {
            int[,] e1 = Transponovanje(e);
            int[,] result = new int[H.GetLength(0), e1.GetLength(1)];

            for (int i = 0; i < H.GetLength(0); i++)
            {
                for (int j = 0; j < e1.GetLength(1); j++)
                {
                    for (int k = 0; k < H.GetLength(1); k++)
                        result[i, j] += H[i, k] * e1[k, j];
                    result[i, j] %= 2;
                }
            }

            int[,] transposedResult = new int[result.GetLength(1), result.GetLength(0)];
            for (int i = 0; i < result.GetLength(0); i++)
                for (int j = 0; j < result.GetLength(1); j++)
                    transposedResult[j, i] = result[i, j];

            return transposedResult;
        }

        public static List<int[]> MinimalWeight(int[,] transposedResult)
        {
            List<int[]> s = new List<int[]>();
            for (int i = 0; i < transposedResult.GetLength(0); i++)
            {
                int[] currentRow = Enumerable.Range(0, transposedResult.GetLength(1))
                                             .Select(j => transposedResult[i, j])
                                             .ToArray();
                bool isUnique = true;
                foreach (int[] uniqueRow in s)
                    if (currentRow.SequenceEqual(uniqueRow))
                    {
                        isUnique = false;
                        break;
                    }
                if (isUnique)
                {
                    s.Add(currentRow);
                    Indeksi.Add(i);
                }
            }
            return s;
        }

        public static void StampajRez(List<int[]> jedinstveniS, List<int[]> eMin)
        {
            Console.WriteLine("E -> S");
            for (int i = 0; i < jedinstveniS.Count; i++)
                Console.WriteLine($"[{string.Join(", ", jedinstveniS[i])}] -> [{string.Join(", ", eMin[i])}]");
        }

        public static int BrojJedinica(int[] array) => array.Count(x => x == 1);

        public static int KodnoRas(int[,] H, List<int[]> kombinacije)
        {
            int brojRedova = H.GetLength(0);
            int kodnoRastojanje = -1;

            for (int i = 1; i < kombinacije.Count; i++)
            {
                int[] sumaReda = new int[brojRedova];
                int brojJedinica = 0;
                int[] trenutna = kombinacije[i];

                for (int indeks = 0; indeks < trenutna.Length; indeks++)
                {
                    if (trenutna[indeks] == 1)
                    {
                        brojJedinica++;
                        for (int j = 0; j < brojRedova; j++)
                            sumaReda[j] += H[j, indeks];
                    }
                }

                for (int j = 0; j < brojRedova; j++)
                    sumaReda[j] %= 2;

                if (sumaReda.Sum() == 0)
                {
                    if (kodnoRastojanje == -1)
                        kodnoRastojanje = brojJedinica;
                    else
                        kodnoRastojanje = Math.Min(kodnoRastojanje, brojJedinica);
                }
            }
            return kodnoRastojanje;
        }

        public static int[] GallagerBAlgoritam(int[] y, double th0, double th1, int maxIterations)
        {
            int n = y.Length;
            int[] x = (int[])y.Clone();

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                for (int j = 0; j < n; j++)
                {
                    int brojNula = 0, brojJedinica = 0;
                    for (int i = 0; i < n; i++)
                    {
                        if (i == j) continue;
                        if (x[i] == 0) brojNula++;
                        else brojJedinica++;
                    }

                    if (brojNula >= th0 * (n - 1)) x[j] = 0;
                    else if (brojJedinica >= th1 * (n - 1)) x[j] = 1;
                }

                if (x.SequenceEqual(y))
                    break;
            }
            return x;
        }
    }
}