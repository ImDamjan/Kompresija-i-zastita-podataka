using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekat2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int[,] H = Matrica.KreirajMatricu();
            List<int[]> binarneKombinacije = Matrica.GenerisiBinarneKomb();

            binarneKombinacije.Sort((a, b) =>
            {
                int wa = Matrica.BrojJedinica(a);
                int wb = Matrica.BrojJedinica(b);
                if (wa == wb)
                    for (int i = 0; i < a.Length; i++)
                        if (a[i] != b[i])
                            return a[i] - b[i];
                return wa - wb;
            });

            int[,] matricaKomb = new int[binarneKombinacije.Count, binarneKombinacije[0].Length];
            for (int i = 0; i < binarneKombinacije.Count; i++)
                for (int j = 0; j < binarneKombinacije[i].Length; j++)
                    matricaKomb[i, j] = binarneKombinacije[i][j];

            int[,] s = Matrica.RacunajSindrom(H, matricaKomb);
            List<int[]> sindromi = Matrica.MinimalWeight(s);
            List<int[]> eMin = Matrica.Indeksi.Select(i => binarneKombinacije[i]).ToList();

            Matrica.StampajRez(sindromi, eMin);

            int kodnoRastojanje = Matrica.KodnoRas(H, binarneKombinacije);
            Console.WriteLine("Kodno rastojanje: " + kodnoRastojanje);

            for (int i = 0; i < H.GetLength(0); i++)
            {
                int[] red = new int[H.GetLength(1)];
                for (int j = 0; j < H.GetLength(1); j++)
                    red[j] = H[i, j];

                int[] dekodirano = Matrica.GallagerBAlgoritam(red, 0.5, 0.5, 183);
                Console.WriteLine("Dekodirano: " + string.Join(", ", dekodirano));
            }

            Console.ReadKey();
        }
    }
}