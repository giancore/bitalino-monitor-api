using System;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class ListPatientExamsQueryResult
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Channel { get; set; }
    }
}
