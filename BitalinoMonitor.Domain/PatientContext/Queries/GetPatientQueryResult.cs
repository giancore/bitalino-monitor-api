using System;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class GetPatientQueryResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PhotoPath { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
