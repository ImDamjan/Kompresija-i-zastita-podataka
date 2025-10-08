using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kompresija
{
    public class Cvor
    {
        public double Verovatnoca { get; set; }
        public byte Simbol { get; set; }
        public Cvor Levo { get; set; }
        public Cvor Desno { get; set; }

        public Cvor(double verovatnoca, byte simbol, Cvor levo, Cvor desno)
        {
            Verovatnoca = verovatnoca;
            Simbol = simbol;
            Levo = levo;
            Desno = desno;
        }
    }

    public class Huffman
    {
        public static Cvor kreirajStablo(List<Bajt> bajtovi)
        {
            var cvorovi = bajtovi.Select(b => new Cvor(b.Verovatnoca, b.Vrednost, null, null)).ToList();

            while (cvorovi.Count > 1)
            {
                cvorovi = cvorovi.OrderBy(c => c.Verovatnoca).ToList();
                Cvor levo = cvorovi[0];
                Cvor desno = cvorovi[1];
                cvorovi.RemoveRange(0, 2);
                Cvor roditelj = new Cvor(levo.Verovatnoca + desno.Verovatnoca, 0, levo, desno);
                cvorovi.Add(roditelj);
            }

            return cvorovi.First();
        }

        public static void dodeliKodove(Cvor cvor, string kod, Dictionary<byte, string> mapa)
        {
            if (cvor.Levo == null && cvor.Desno == null)
            {
                mapa[cvor.Simbol] = kod;
                return;
            }
            if (cvor.Levo != null) dodeliKodove(cvor.Levo, kod + "0", mapa);
            if (cvor.Desno != null) dodeliKodove(cvor.Desno, kod + "1", mapa);
        }

        public static string Kodiraj(byte[] podaci, Dictionary<byte, string> mapa)
        {
            StringBuilder rezultat = new StringBuilder();
            foreach (byte b in podaci)
                rezultat.Append(mapa[b]);
            return rezultat.ToString();
        }

        public static void SacuvajUFajl(Dictionary<byte, string> mapa, string kodovaniBitovi, string izlaznaPutanja)
        {
            int brojPaddingBitova = (8 - (kodovaniBitovi.Length % 8)) % 8;

            using (FileStream fs = new FileStream(izlaznaPutanja, FileMode.Create))
            using (StreamWriter pisac = new StreamWriter(fs, Encoding.UTF8, 1024, true))
            {
                foreach (var kv in mapa)
                    pisac.Write($"{kv.Key}:{kv.Value} ");
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