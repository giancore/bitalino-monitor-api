using System;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class ListPatientExamsBitalinoFrameQueryResult
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; }
        public int Seq { get; set; }
        public int A0 { get; set; }
        public int A1 { get; set; }
        public int A2 { get; set; }
        public int A3 { get; set; }
        public int A4 { get; set; }
        public int A5 { get; set; }
        public int D0 { get; set; } 
        public int D1 { get; set; }
        public int D2 { get; set; }
        public int D3 { get; set; }
    }
}
