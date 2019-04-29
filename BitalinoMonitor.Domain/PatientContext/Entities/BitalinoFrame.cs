using BitalinoMonitor.Shared.Entities;

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
