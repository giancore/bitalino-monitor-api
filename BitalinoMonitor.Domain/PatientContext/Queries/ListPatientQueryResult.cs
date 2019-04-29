using System;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class ListPatientQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PhotoPath { get; set; }
    }
}
