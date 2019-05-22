using BitalinoMonitor.Shared.Entities;
using System;
using System.Linq;

namespace BitalinoMonitor.Domain.PatientContext.Entities
{
    public class BitalinoFrame : Entity
    {
        public string Identifier { get; private set; }
        public int Seq { get; private set; }
        public double[] Analog { get; private set; }
        public double[] Digital { get; private set; }

        public BitalinoFrame(string identifier, int seq, double[] analog, double[] digital)
        {
            Identifier = identifier;
            Seq = seq;
            Analog = analog;
            Digital = digital;
        }

        public BitalinoFrame(string identifier, int seq, int[] analog, int[] digital)
        {
            Identifier = identifier;
            Seq = seq;
            Analog = analog.Select(a => Convert.ToDouble(a)).ToArray();
            Digital = digital.Select(a => Convert.ToDouble(a)).ToArray();
        }

        public BitalinoFrame(Guid id, string identifier, int seq, int a0, int a1, int a2, int a3, int a4, int a5, int d0, int d1, int d2, int d3)
        {
            SetId(id);
            Identifier = identifier;
            Seq = seq;
            Analog = new double[] { a0, a1, a2, a3, a4, a5 };
            Digital = new double[] { d0, d1, d2, d3 };
        }

        public double GetAnalog(int pos)
        {
            return Analog[pos];
        }

        public double GetDigital(int pos)
        {
            return Digital[pos];
        }
    }
}
