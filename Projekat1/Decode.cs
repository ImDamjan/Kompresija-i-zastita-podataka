using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kompresija
{
    public class Decode
    {

        public static List<UcitaniBajt> UcitaniBajt(string putanja)
        {
            var lista = new List<UcitaniBajt>();

            using (var sr = new StreamReader(new FileStream(putanja, FileMode.Open, FileAccess.Read), Encoding.UTF8))
            {
                string prviRed = sr.ReadLine();

                var parovi = prviRed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var par in parovi)
                {
                    var delovi = par.Split(':');
                    if (delovi.Length != 2) continue;
                    if (delovi[0] == "PAD") continue; // preskoči padding info
                    lista.Add(new UcitaniBajt(delovi[0], delovi[1]));
                }
            }

            return lista;
        }

        public static string DajOstatak(string putanja)
        {
            byte[] svi = File.ReadAllBytes(putanja);

            int idx = 0;
            while (idx < svi.Length && svi[idx] != (byte)'\n')
                idx++;

            int pocetak = (idx < svi.Length) ? idx + 1 : 0;

            var sb = new StringBuilder((svi.Length - pocetak) * 8);
            for (int i = pocetak; i < svi.Length; i++)
            {
                sb.Append(Convert.ToString(svi[i], 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
        public static string UkloniCifre(string binarniNiz, int brojCifaraZaUklanjanje)
        {
            if (string.IsNullOrEmpty(binarniNiz)) return binarniNiz;
            if (brojCifaraZaUklanjanje <= 0) return binarniNiz;
            if (brojCifaraZaUklanjanje >= binarniNiz.Length)
                return string.Empty;
            return binarniNiz.Substring(0, binarniNiz.Length - brojCifaraZaUklanjanje);
        }

        public static byte[] Dekodiraj(string bitniNiz, List<UcitaniBajt> mapa)
        {
            var kodURBajt = new Dictionary<string, byte>(StringComparer.Ordinal);
            foreach (var ub in mapa)
            {
                if (byte.TryParse(ub.BajtVrednost, out byte b))
                {
                    kodURBajt[ub.KodBajta] = b;
                }
                else
                {
                    throw new Exception($"Nevalidna bajt vrednost u mapi: '{ub.BajtVrednost}'");
                }
            }

            var rezultat = new List<byte>();
            var trenutni = new StringBuilder();
            for (int i = 0; i < bitniNiz.Length; i++)
            {
                trenutni.Append(bitniNiz[i]);
                string key = trenutni.ToString();
                if (kodURBajt.TryGetValue(key, out byte vrednost))
                {
                    rezultat.Add(vrednost);
                    trenutni.Clear();
                }
            }

            return rezultat.ToArray();
        }

        public static string Uporedi(byte[] dekodirani)
        {
            string FilePath1 = "ulazNiskaEntropija.bin";
            //string FilePath2 = "ulazVisokaEntropija.bin";
            string cwd = Directory.GetCurrentDirectory();
            string ulaz = Path.Combine(cwd, FilePath1);

            if (!File.Exists(ulaz))
                return $"Originalni fajl '{ulaz}' ne postoji!";

            byte[] original = File.ReadAllBytes(ulaz);

            if (original.Length != dekodirani.Length)
                return $"Dekodiranje nije uspelo, dužine se razlikuju";

            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] != dekodirani[i])
                    return $"Dekodiranje nije uspelo (razlika na poziciji {i}).";
            }

            return "Ulazni fajl je isti nakon kompresije i dekompresije";
        }

        public static void SacuvajDekodiran(string izlaznaPutanja, byte[] podaci)
        {
            File.WriteAllBytes(izlaznaPutanja, podaci);
        }

        public static int ProcitajPaddingBitova(string putanja)
        {
            using (var sr = new StreamReader(putanja))
            {
                string prviRed = sr.ReadLine();
                var padPar = prviRed.Split(' ').FirstOrDefault(par => par.StartsWith("PAD:"));
                if (padPar != null)
                    return int.Parse(padPar.Split(':')[1]);
            }
            return 0;
        }
    }
}