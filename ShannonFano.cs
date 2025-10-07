using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kompresija
{
    public class ShannonFano
    {
        public static void Enkodiraj(List<Bajt> bajtovi)
        {
            if (bajtovi.Count <= 1) return;

            double ukupnaVerovatnoca = bajtovi.Sum(b => b.Verovatnoca);
            double zbir = 0;
            int granica = 0;

            for (int i = 0; i < bajtovi.Count; i++)
            {
                zbir += bajtovi[i].Verovatnoca;
                if (zbir >= ukupnaVerovatnoca / 2)
                {
                    granica = i;
                    break;
                }
            }

            for (int i = 0; i <= granica; i++) bajtovi[i].Kod += "0";
            for (int i = granica + 1; i < bajtovi.Count; i++) bajtovi[i].Kod += "1";

            Enkodiraj(bajtovi.Take(granica + 1).ToList());
            Enkodiraj(bajtovi.Skip(granica + 1).ToList());
        }

        public static string Kod(List<Bajt> bajtovi, byte[] podaci)
        {
            StringBuilder rezultat = new StringBuilder();

            foreach (byte b in podaci)
            {
                var simbol = bajtovi.FirstOrDefault(s => s.Vrednost == b);
                if (simbol != null)
                    rezultat.Append(simbol.Kod);
            }

            return rezultat.ToString();
        }

        public static void SacuvajUFajl(List<Bajt> bajtovi, string kodovaniBitovi, string izlaznaPutanja)
        {
            int brojPaddingBitova = (8 - (kodovaniBitovi.Length % 8)) % 8;

            using (FileStream fs = new FileStream(izlaznaPutanja, FileMode.Create))
            using (StreamWriter pisac = new StreamWriter(fs, Encoding.UTF8, 1024, true))
            {
                foreach (var b in bajtovi)
                    pisac.Write($"{b.Vrednost}:{b.Kod} ");
                pisac.Write($"PAD:{brojPaddingBitova}");
                pisac.WriteLine();
            }

            List<byte> bajtPodaci = new List<byte>();
            for (int i = 0; i < kodovaniBitovi.Length; i += 8)
            {
                string deo = kodovaniBitovi.Substring(i, Math.Min(8, kodovaniBitovi.Length - i));
                bajtPodaci.Add(Convert.ToByte(deo.PadRight(8, '0'), 2));
            }

            using (var fs = new FileStream(izlaznaPutanja, FileMode.Append))
            {
                fs.Write(bajtPodaci.ToArray(), 0, bajtPodaci.Count);
            }
        }
    }
}