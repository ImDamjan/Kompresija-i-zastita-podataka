namespace Kompresija
{
    public class Bajt
    {
        public byte Vrednost { get; set; }
        public double Verovatnoca { get; set; }
        public string Kod { get; set; } = "";

        public Bajt(byte vrednost, double verovatnoca)
        {
            Vrednost = vrednost;
            Verovatnoca = verovatnoca;
        }
    }
}
