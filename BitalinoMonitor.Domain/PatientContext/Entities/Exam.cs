using BitalinoMonitor.Domain.PatientContext.Enums;
using BitalinoMonitor.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitalinoMonitor.Domain.PatientContext.Entities
{
    public class Exam : Entity
    {
        readonly IEnumerable<BitalinoFrame> _frames;

        public EExamType Type { get; private set; }
        public DateTime Date { get; private set; }
        public int Channel { get; private set; }
        public IReadOnlyCollection<BitalinoFrame> Frames => _frames.ToArray();

        public Exam(int channel, DateTime date, EExamType type, IEnumerable<BitalinoFrame> frames)
        {
            Channel = channel;
            Date = date;
            Type = type;
            _frames = frames;
        }
    }
}
