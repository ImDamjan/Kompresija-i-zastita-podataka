using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Kompresija
{
    public class LZW
    {
        private static List<char> uniqueSymbols = new List<char>(); // Lista za čuvanje jedinstvenih simbola iz ulaznog fajla
        private static List<byte> uniqueSymbolsFromFile = new List<byte>(); // Lista jedinstvenih simbola iz fajla (za dekodiranje)

        // Funkcija za LZW enkodiranje - radi sa binarnim fajlom
        public List<int> LZWEnkoder(string inputPath)
        {
            List<int> output = new List<int>(); // Lista za čuvanje enkodiranih vrednosti
            Dictionary<string, int> dictionary = new Dictionary<string, int>(); // Rečnik za čuvanje sekvenci i njihovih kodova

            try
            {
                byte[] inputBytes = File.ReadAllBytes(inputPath); // Čitanje celog fajla kao bajtova

                // Dodavanje jedinstvenih bajtova u listu
                uniqueSymbols = inputBytes.Distinct().Select(b => (char)b).ToList();

                // Dodavanje jedinstvenih bajtova u rečnik
                int code = 1;
                foreach (char c in uniqueSymbols)
                {
                    dictionary[c.ToString()] = code++;
                }

                if (inputBytes.Length == 0) return output; // Ako je fajl prazan, vrati praznu listu

                string currentSequence = ((char)inputBytes[0]).ToString(); // Početna sekvenca je prvi bajt
                for (int i = 1; i < inputBytes.Length; i++)
                {
                    char ch = (char)inputBytes[i];
                    string nextSequence = currentSequence + ch;
                    if (dictionary.ContainsKey(nextSequence))
                    {
                        currentSequence = nextSequence; // Ako rečnik sadrži kombinaciju trenutne sekvence i narednog bajta, ažuriraj trenutnu sekvencu
                    }
                    else
                    {
                        output.Add(dictionary[currentSequence]); // Dodaj kod trenutne sekvence u izlaznu listu
                        dictionary[nextSequence] = code++; // Dodaj novu sekvencu u rečnik
                        currentSequence = ch.ToString(); // Postavi trenutnu sekvencu na naredni bajt
                    }
                }

                output.Add(dictionary[currentSequence]); // Dodaj preostalu sekvencu u izlaz
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message); // Obrada greške prilikom čitanja fajla
            }

            return output; // Vraća listu enkodiranih vrednosti
        }

        // Getter za listu jedinstvenih simbola
        public static List<char> GetUniqueSymbols()
        {
            return uniqueSymbols; // Vraća listu jedinstvenih simbola
        }

        // Funkcija za čuvanje enkodiranih podataka u binarni fajl
        public void SacuvajUFajl(List<int> kodiraniIzlazLZW)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open("lzw.bin", FileMode.Create)))
                {
                    writer.Write(uniqueSymbols.Count); // Broj jedinstvenih simbola
                    foreach (char c in uniqueSymbols)
                    {
                        writer.Write((byte)c); // Jedinstveni simboli kao bajtovi
                    }
                    writer.Write(kodiraniIzlazLZW.Count); // Broj kodiranih vrednosti
                    foreach (int i in kodiraniIzlazLZW)
                    {
                        writer.Write(i); // Kodirane vrednosti
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message); // Obrada greške prilikom pisanja fajla
            }
        }

        // Funkcija za učitavanje enkodiranih podataka iz binarnog fajla
        public List<int> CitajIzFajla()
        {
            List<int> indeksiLZW = new List<int>(); // Lista dekodiranih indeksa

            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open("lzw.bin", FileMode.Open)))
                {
                    int uniqueCount = reader.ReadInt32(); // Broj jedinstvenih simbola
                    uniqueSymbolsFromFile = new List<byte>();
                    for (int i = 0; i < uniqueCount; i++)
                    {
                        uniqueSymbolsFromFile.Add(reader.ReadByte()); // Jedinstveni simboli
                    }
                    int codesCount = reader.ReadInt32(); // Broj kodiranih vrednosti
                    for (int i = 0; i < codesCount; i++)
                    {
                        indeksiLZW.Add(reader.ReadInt32()); // Kodirane vrednosti
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message); // Obrada greške prilikom čitanja fajla
            }

            return indeksiLZW; // Vraća listu dekodiranih indeksa
        }

        // Funkcija za LZW dekodiranje - vraća bajt niz
        public byte[] LZWDekoder(List<int> kodiraniLZWFromFile)
        {
            Dictionary<int, string> recnik = new Dictionary<int, string>(); // Rečnik za dekodiranje
            for (int i = 0; i < uniqueSymbolsFromFile.Count; i++)
            {
                recnik[i + 1] = ((char)uniqueSymbolsFromFile[i]).ToString(); // Popunjavanje rečnika jedinstvenim simbolima iz fajla
            }

            if (kodiraniLZWFromFile.Count == 0) return new byte[0]; // Ako nema kodova, vrati prazni niz

            int trenutniIndeks = recnik.Count + 1; // Postavljanje početnog indeksa za nove sekvence
            string rezultatStr = recnik[kodiraniLZWFromFile[0]]; // Početak dekodiranog stringa
            string trenutnaSekvencija = recnik[kodiraniLZWFromFile[0]]; // Trenutna sekvenca

            foreach (int indeks in kodiraniLZWFromFile.Skip(1))
            {
                string novaSekvencija;
                if (recnik.ContainsKey(indeks))
                {
                    novaSekvencija = recnik[indeks]; // Ako indeks postoji u rečniku, postavi novu sekvencu na vrednost iz rečnika
                }
                else if (indeks == trenutniIndeks)
                {
                    novaSekvencija = trenutnaSekvencija + trenutnaSekvencija[0]; // Ako je indeks jednak trenutnom indeksu, dodaj prvi karakter trenutne sekvence
                }
                else
                {
                    throw new ArgumentException("Neispravan niz indeksa."); // Bacanje greške ako je indeks neispravan
                }

                rezultatStr += novaSekvencija; // Dodavanje nove sekvence u rezultat

                recnik[trenutniIndeks] = trenutnaSekvencija + novaSekvencija[0]; // Dodavanje nove sekvence u rečnik
                trenutniIndeks++;

                trenutnaSekvencija = novaSekvencija; // Ažuriranje trenutne sekvence
            }

            return rezultatStr.Select(c => (byte)c).ToArray(); // Vraća dekodirani niz bajtova
        }

        // Pomoćna klasa za poređenje lista bajtova (više nije potrebna jer koristimo string)
        // private class ByteListComparer : IEqualityComparer<List<byte>> { ... }

        // Static methods for consistency with other algorithms

        public static List<int> Kompresuj(byte[] podaci)
        {
            try
            {
                List<int> output = new List<int>();
                Dictionary<string, int> dictionary = new Dictionary<string, int>();

                uniqueSymbols = podaci.Distinct().Select(b => (char)b).ToList();

                int code = 1;
                foreach (char c in uniqueSymbols)
                {
                    dictionary[c.ToString()] = code++;
                }

                if (podaci.Length == 0) return output;

                string currentSequence = ((char)podaci[0]).ToString();
                for (int i = 1; i < podaci.Length; i++)
                {
                    char ch = (char)podaci[i];
                    string nextSequence = currentSequence + ch;
                    if (dictionary.ContainsKey(nextSequence))
                    {
                        currentSequence = nextSequence;
                    }
                    else
                    {
                        output.Add(dictionary[currentSequence]);
                        dictionary[nextSequence] = code++;
                        currentSequence = ch.ToString();
                    }
                }
                output.Add(dictionary[currentSequence]);
                return output;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW Kompresuj: " + e.Message);
                return new List<int>();
            }
        }

        public static byte[] Serialize(List<int> kodiraniIzlazLZW)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    writer.Write(uniqueSymbols.Count);
                    foreach (char c in uniqueSymbols)
                        writer.Write((byte)c);
                    writer.Write(kodiraniIzlazLZW.Count);
                    foreach (int i in kodiraniIzlazLZW)
                        writer.Write(i);
                    return ms.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW Serialize: " + e.Message);
                return new byte[0];
            }
        }

        public static void SacuvajUFajl(string path, byte[] data)
        {
            try
            {
                File.WriteAllBytes(path, data);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW SacuvajUFajl: " + e.Message);
            }
        }

        public static byte[] UcitajIzFajla(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW UcitajIzFajla: " + e.Message);
                return new byte[0];
            }
        }

        public static List<int> Deserialize(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    int uniqueCount = reader.ReadInt32();
                    uniqueSymbolsFromFile = new List<byte>();
                    for (int i = 0; i < uniqueCount; i++)
                        uniqueSymbolsFromFile.Add(reader.ReadByte());
                    int codesCount = reader.ReadInt32();
                    List<int> indeksiLZW = new List<int>();
                    for (int i = 0; i < codesCount; i++)
                        indeksiLZW.Add(reader.ReadInt32());
                    return indeksiLZW;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW Deserialize: " + e.Message);
                return new List<int>();
            }
        }

        public static byte[] Dekompresuj(List<int> kodiraniLZWFromFile)
        {
            try
            {
                Dictionary<int, string> recnik = new Dictionary<int, string>();
                for (int i = 0; i < uniqueSymbolsFromFile.Count; i++)
                {
                    recnik[i + 1] = ((char)uniqueSymbolsFromFile[i]).ToString();
                }

                if (kodiraniLZWFromFile.Count == 0) return new byte[0];

                int trenutniIndeks = recnik.Count + 1;
                StringBuilder rezultat = new StringBuilder(recnik[kodiraniLZWFromFile[0]]);
                string trenutnaSekvencija = recnik[kodiraniLZWFromFile[0]];

                foreach (int indeks in kodiraniLZWFromFile.Skip(1))
                {
                    string novaSekvencija;
                    if (recnik.ContainsKey(indeks))
                    {
                        novaSekvencija = recnik[indeks];
                    }
                    else if (indeks == trenutniIndeks)
                    {
                        novaSekvencija = trenutnaSekvencija + trenutnaSekvencija[0];
                    }
                    else
                    {
                        throw new ArgumentException("Neispravan niz indeksa.");
                    }

                    rezultat.Append(novaSekvencija);

                    recnik[trenutniIndeks] = trenutnaSekvencija + novaSekvencija[0];
                    trenutniIndeks++;

                    trenutnaSekvencija = novaSekvencija;
                }

                return rezultat.ToString().Select(c => (byte)c).ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in LZW Dekompresuj: " + e.Message);
                return new byte[0];
            }
        }
    }
}