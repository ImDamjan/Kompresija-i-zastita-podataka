using System;
using System.Collections.Generic;
using System.IO;

namespace Kompresija
{
    public class Entropija
    {
        public static List<byte> BajtoviNiz { get; private set; } = new List<byte>();
        public static List<double> VerovatnocaSvakogBajta { get; private set; } = new List<double>();
        public double IzracunajBajtEntropiju(string putanja)
        {
            try
            {
                BajtoviNiz.Clear();
                VerovatnocaSvakogBajta.Clear();

                long N = 0;  
                int bajtVrednost;
                long[] brojPojavljivanja = new long[256];

                using (FileStream fs = new FileStream(putanja, FileMode.Open, FileAccess.Read))
                {
                    while ((bajtVrednost = fs.ReadByte()) != -1)
                    {
                        N++;
                        brojPojavljivanja[bajtVrednost]++;
                    }
                }

                double[] p = new double[256];
                // Verovatnoca
                for (int i = 0; i < brojPojavljivanja.Length; i++)
                {
                    p[i] = (double)brojPojavljivanja[i] / N;
                }

                for (int byteValue = 0; byteValue < brojPojavljivanja.Length; byteValue++)
                {
                    if (brojPojavljivanja[byteValue] > 0)
                    {
                        byte bajt = (byte)byteValue;
                        double p_i = p[byteValue];
                        BajtoviNiz.Add(bajt);
                        VerovatnocaSvakogBajta.Add(p_i); 
                    }
                }

                // Izračunavanje entropije po formuli
                double entropija = 0;
                for (int i = 0; i < p.Length; i++)
                {
                    if (p[i] != 0)
                    {
                        entropija -= p[i] * (Math.Log(p[i]) / Math.Log(2));
                    }
                }

                return entropija;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}