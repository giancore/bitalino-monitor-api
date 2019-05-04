using System;
using System.Collections.Generic;
using System.Text;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class GetPatientExamFramesQueryResult
    {
        public Guid Id { get; set; }
        public string Identifier { get; set; }
        public int Seq { get; set; }
        public int[] Analog { get; set; }
        public int[] Digital { get; set; }
    }
}
