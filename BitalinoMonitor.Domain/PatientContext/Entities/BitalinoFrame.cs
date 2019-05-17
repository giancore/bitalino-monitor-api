using BitalinoMonitor.Shared.Entities;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Entities
{
    public class BitalinoFrame : Entity
    {
        public string Identifier { get; private set; }
        public int Seq { get; private set; }
        public int[] Analog { get; private set; }
        public int[] Digital { get; private set; }

        public BitalinoFrame(string identifier, int seq, int[] analog, int[] digital)
        {
            Identifier = identifier;
            Seq = seq;
            Analog = analog;
            Digital = digital;
        }

        public BitalinoFrame(Guid id, string identifier, int seq, int a0, int a1, int a2, int a3, int a4, int a5, int d0, int d1, int d2, int d3)
        {
            SetId(id);
            Identifier = identifier;
            Seq = seq;
            Analog = new int[] { a0, a1, a2, a3, a4, a5 };
            Digital = new int[] { d0, d1, d2, d3 };
        }

        public int GetAnalog(int pos)
        {
            return Analog[pos];
        }

        public int GetDigital(int pos)
        {
            return Digital[pos];
        }
    }
}
