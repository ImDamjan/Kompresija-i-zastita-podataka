using System;
using System.Collections.Generic;
using System.IO;

namespace Kompresija
{
    public class LZ77
    {
        const int WINDOW_SIZE = 256; // Ostavio manji za brzinu
        const int LOOKAHEAD_BUFFER_SIZE = 8;

        // Kompresija: vraća listu tuple-a (distance, length, nextSymbol?)
        public static List<Tuple<int, int, byte?>> Kompresuj(byte[] input)
        {
            var output = new List<Tuple<int, int, byte?>>();
            int pos = 0;

            while (pos < input.Length)
            {

                int bestMatchDistance = 0, bestMatchLength = 0;
                int searchStart = Math.Max(0, pos - WINDOW_SIZE);

                // Traži najduže podudaranje u prozoru
                for (int i = searchStart; i < pos; i++)
                {
                    int length = 0;
                    while (length < LOOKAHEAD_BUFFER_SIZE &&
                           pos + length < input.Length &&
                           input[i + length] == input[pos + length])
                        length++;

                    if (length > bestMatchLength)
                    {
                        bestMatchDistance = pos - i;
                        bestMatchLength = length;
                    }
                }

                // Sledeći simbol samo ako postoji
                byte? nextSymbol = null;
                if (pos + bestMatchLength < input.Length)
                    nextSymbol = input[pos + bestMatchLength];

                output.Add(Tuple.Create(bestMatchDistance, bestMatchLength, nextSymbol));

                // Napredujemo bar za 1
                pos += (bestMatchLength == 0) ? 1 : bestMatchLength + 1;
            }

            return output;
        }

        // Dekompresija: iz liste tuple-a vraća byte[]
        public static byte[] Dekompresuj(List<Tuple<int, int, byte?>> input)
        {
            var output = new List<byte>();

            foreach (var tuple in input)
            {
                int distance = tuple.Item1, length = tuple.Item2;
                byte? symbol = tuple.Item3;

                if (distance > 0 && length > 0)
                {
                    int start = output.Count - distance;
                    for (int i = 0; i < length; i++)
                        output.Add(output[start + i]);
                }

                // Dodaj symbol samo ako postoji
                if (symbol.HasValue)
                    output.Add(symbol.Value);
            }

            return output.ToArray();
        }

        // Serijalizacija: flag + distance + length + symbol (ako flag=1)
        public static byte[] Serialize(List<Tuple<int, int, byte?>> input)
        {
            var bytes = new List<byte>();
            foreach (var tuple in input)
            {
                bytes.Add(tuple.Item3.HasValue ? (byte)1 : (byte)0); // flag: 0 ili 1
                bytes.AddRange(BitConverter.GetBytes((ushort)tuple.Item1)); // distance: 2 bajta
                bytes.Add((byte)tuple.Item2); // length: 1 bajt
                if (tuple.Item3.HasValue)
                    bytes.Add(tuple.Item3.Value); // symbol: 1 bajt
            }
            return bytes.ToArray();
        }

        // Deserijalizacija: čita flag, distance, length, symbol ako flag=1
        public static List<Tuple<int, int, byte?>> Deserialize(byte[] input)
        {
            var result = new List<Tuple<int, int, byte?>>();
            int i = 0;
            while (i < input.Length)
            {
                byte flag = input[i];
                int distance = BitConverter.ToUInt16(input, i + 1);
                int length = input[i + 3];
                byte? symbol = null;
                int advance = 4; // flag + distance(2) + length(1)
                if (flag == 1)
                {
                    symbol = input[i + 4];
                    advance = 5; // + symbol(1)
                }
                result.Add(Tuple.Create(distance, length, symbol));
                i += advance;
            }
            return result;
        }

        // Snimanje byte[] u fajl
        public static void SacuvajUFajl(string putanja, byte[] podaci)
        {
            File.WriteAllBytes(putanja, podaci);
        }

        // Učitavanje byte[] iz fajla
        public static byte[] UcitajIzFajla(string putanja)
        {
            return File.ReadAllBytes(putanja);
        }
    }
}