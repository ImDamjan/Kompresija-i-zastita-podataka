using System;
using System.IO;

namespace Projekat11
{
    public class StepenKompresije
    {
        static string currentDirectory = Directory.GetCurrentDirectory();

        // Privatna funkcija za računanje odnosa veličina
        private double IzracunajStepenKompresije(long inputSize, long outputSize)
        {
            if (outputSize == 0) return 0;
            double ratio = (double)inputSize / outputSize;
            return Math.Round(ratio, 4);
        }

        // Privatna funkcija za čitanje veličina fajlova i izračunavanje stepena
        private double IzracunajStepen(string inputFile, string outputFile)
        {
            try
            {
                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"❌ Ulazni fajl nije pronađen: {inputFile}");
                    return 0;
                }

                if (!File.Exists(outputFile))
                {
                    Console.WriteLine($"❌ Izlazni fajl nije pronađen: {outputFile}");
                    return 0;
                }

                long inputSize = new FileInfo(inputFile).Length;
                long outputSize = new FileInfo(outputFile).Length;

                return IzracunajStepenKompresije(inputSize, outputSize);
            }
            catch (IOException e)
            {
                Console.WriteLine($"⚠️ Greška pri čitanju fajlova: {e.Message}");
                return 0;
            }
        }

        // Shannon-Fano
        public double StepenKompresijeShannonFano()
        {
            string inputFile = Path.Combine(currentDirectory, "ulazNiskaEntropija.bin");
            string outputFile = Path.Combine(currentDirectory, "shannon_fano.bin");
            return IzracunajStepen(inputFile, outputFile);
        }

        // Huffman
        public double StepenKompresijeHuffman()
        {
            string inputFile = Path.Combine(currentDirectory, "ulazNiskaEntropija.bin");
            string outputFile = Path.Combine(currentDirectory, "huffman.bin");
            return IzracunajStepen(inputFile, outputFile);
        }

        // LZ77
        public double StepenKompresijeLZ77()
        {
            string inputFile = Path.Combine(currentDirectory, "ulazNiskaEntropija.bin");
            string outputFile = Path.Combine(currentDirectory, "lz77.bin");
            return IzracunajStepen(inputFile, outputFile);
        }

        // LZW
        public double StepenKompresijeLZW()
        {
            string inputFile = Path.Combine(currentDirectory, "ulazNiskaEntropija.bin");
            string outputFile = Path.Combine(currentDirectory, "lzw.bin");
            return IzracunajStepen(inputFile, outputFile);
        }
    }
}
