using Projekat11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kompresija
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string FilePath1 = "ulazNiskaEntropija.bin";
            //string FilePath2 = "ulazVisokaEntropija.bin";
            string direktorijum = Directory.GetCurrentDirectory();
            string putanja = Path.Combine(direktorijum, FilePath1);

            Entropija entropija = new Entropija();
            double vrednostEntropije = entropija.IzracunajBajtEntropiju(putanja);
            Console.WriteLine("Vrednost entropije fajla je: " + vrednostEntropije);

            List<Bajt> bajtovi = new List<Bajt>();
            for (int i = 0; i < Entropija.BajtoviNiz.Count; i++)
            {
                bajtovi.Add(new Bajt(Entropija.BajtoviNiz[i], Entropija.VerovatnocaSvakogBajta[i]));
            }

            // Shannon-Fano kompresija
            var bajtoviSF = bajtovi.OrderByDescending(b => b.Verovatnoca).ToList();
            ShannonFano.Enkodiraj(bajtoviSF);

            byte[] podaci = File.ReadAllBytes(FilePath1);
            string kodovaniBitoviSF = ShannonFano.Kod(bajtoviSF, podaci);

            ShannonFano.SacuvajUFajl(bajtoviSF, kodovaniBitoviSF, "shannon_fano.bin");

            // Huffman kompresija
            var cvorHuffman = Huffman.kreirajStablo(bajtovi);
            var mapaHuffman = new Dictionary<byte, string>();
            Huffman.dodeliKodove(cvorHuffman, "", mapaHuffman);
            string kodovaniBitoviHuffman = Huffman.Kodiraj(podaci, mapaHuffman);
            Huffman.SacuvajUFajl(mapaHuffman, kodovaniBitoviHuffman, "huffman.bin");

            // Dekodiranje Shannon-Fano fajla
            var ucitaniBajtoviSF = Decode.UcitaniBajt("shannon_fano.bin");
            string bitniNizSF = Decode.DajOstatak("shannon_fano.bin");
            int brojPaddingBitovaSF = Decode.ProcitajPaddingBitova("shannon_fano.bin");
            string bitniNizBezPaddingaSF = Decode.UkloniCifre(bitniNizSF, brojPaddingBitovaSF);
            byte[] dekodiraniSF = Decode.Dekodiraj(bitniNizBezPaddingaSF, ucitaniBajtoviSF);
            string rezultatSF = Decode.Uporedi(dekodiraniSF);
            Console.WriteLine($"Rezultat dekodiranja Shannon-Fano: {rezultatSF}");
            Decode.SacuvajDekodiran("dekodirano_shannon_fano.bin", dekodiraniSF);

            // Dekodiranje Huffman fajla
            var ucitaniBajtoviHuffman = Decode.UcitaniBajt("huffman.bin");
            string bitniNizHuffman = Decode.DajOstatak("huffman.bin");
            int brojPaddingBitovaHuffman = Decode.ProcitajPaddingBitova("huffman.bin");
            string bitniNizBezPaddingaHuffman = Decode.UkloniCifre(bitniNizHuffman, brojPaddingBitovaHuffman);
            byte[] dekodiraniHuffman = Decode.Dekodiraj(bitniNizBezPaddingaHuffman, ucitaniBajtoviHuffman);
            string rezultatHuffman = Decode.Uporedi(dekodiraniHuffman);
            Console.WriteLine($"Rezultat dekodiranja Huffman: {rezultatHuffman}");
            Decode.SacuvajDekodiran("dekodirano_huffman.bin", dekodiraniHuffman);

            //LZ77 
            var lz77Kompresovano = LZ77.Kompresuj(podaci);
            byte[] lz77Fajl = LZ77.Serialize(lz77Kompresovano);
            LZ77.SacuvajUFajl("lz77.bin", lz77Fajl);
            var lz77Dekompresovano = LZ77.Dekompresuj(LZ77.Deserialize(LZ77.UcitajIzFajla("lz77.bin")));
            Decode.SacuvajDekodiran("dekodirano_lz77.bin", lz77Dekompresovano);
            Console.WriteLine("LZ77 dekodiranje: " + Decode.Uporedi(lz77Dekompresovano));


            // LZW 
            var lzwKompresovano = LZW.Kompresuj(podaci);
            byte[] lzwFajl = LZW.Serialize(lzwKompresovano);
            LZW.SacuvajUFajl("lzw.bin", lzwFajl);
            var lzwDekompresovano = LZW.Dekompresuj(LZW.Deserialize(LZW.UcitajIzFajla("lzw.bin")));
            Decode.SacuvajDekodiran("dekodirano_lzw.bin", lzwDekompresovano);
            Console.WriteLine("LZW dekodiranje: " + Decode.Uporedi(lzwDekompresovano));

            // Stepeni kompresije
            var stepen = new StepenKompresije();

            Console.WriteLine("\nStepeni kompresije:");

            double sf = stepen.StepenKompresijeShannonFano();
            double hf = stepen.StepenKompresijeHuffman();
            double lz77 = stepen.StepenKompresijeLZ77();
            double lzw = stepen.StepenKompresijeLZW();

            Console.WriteLine($"Shannon-Fano: {sf}x (ušteda: {Math.Round((1 - 1 / sf) * 100, 2)}%)");
            Console.WriteLine($"Huffman:      {hf}x (ušteda: {Math.Round((1 - 1 / hf) * 100, 2)}%)");
            Console.WriteLine($"LZ77:         {lz77}x (ušteda: {Math.Round((1 - 1 / lz77) * 100, 2)}%)");
            Console.WriteLine($"LZW:          {lzw}x (ušteda: {Math.Round((1 - 1 / lzw) * 100, 2)}%)");
            Console.ReadLine();
        }
    }
}