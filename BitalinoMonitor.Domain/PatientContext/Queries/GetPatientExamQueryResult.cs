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
        public int Frequency { get; set; }
        public long Duration { get; set; }
        public List<GetPatientExamFramesQueryResult> Frames { get; set; }

        public GetPatientExamQueryResult()
        {
            Frames = new List<GetPatientExamFramesQueryResult>();
        }
    }
}
