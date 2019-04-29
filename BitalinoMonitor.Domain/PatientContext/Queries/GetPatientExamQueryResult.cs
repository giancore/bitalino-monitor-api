using System;
using System.Collections.Generic;

namespace BitalinoMonitor.Domain.PatientContext.Queries
{
    public class GetPatientExamQueryResult
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public int Channel { get; set; }
        public List<ListPatientExamsBitalinoFrameQueryResult> Frames { get; set; }
    }
}
