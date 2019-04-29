using System;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class ListPatientExamsBitalinoFrameQueryResult
    {
        public Guid Id { get; set; }
        public string Identifier { get; private set; }
        public int Seq { get; private set; }
        public int[] Analog { get; private set; }
        public int[] Digital { get; private set; }
    }
}
