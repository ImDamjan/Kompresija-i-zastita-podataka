using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kompresija
{
    public class UcitaniBajt
    {
        public string BajtVrednost { get; set; }
        public string KodBajta { get; set; }

        public UcitaniBajt(string bajtVrednost, string kodBajta)
        {
            BajtVrednost = bajtVrednost;
            KodBajta = kodBajta;
        }
    }
}
