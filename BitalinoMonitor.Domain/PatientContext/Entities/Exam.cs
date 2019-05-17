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

        public DateTime Date { get; private set; }
        public int Channel { get; private set; }
        public TimeSpan Duration { get; private set; }
        public int Frequency { get; private set; }
        public EExamType Type
        {
            get
            {
                return (EExamType)Channel;
            }
        }

        public IReadOnlyCollection<BitalinoFrame> Frames => _frames.ToArray();

        public Exam(int channel, int frequency, TimeSpan duration, DateTime date, IEnumerable<BitalinoFrame> frames)
        {
            Channel = channel;
            Frequency = frequency;
            Duration = duration;
            Date = date;
            _frames = frames;
        }

        public Exam(Guid id, int channel, int frequency, TimeSpan duration, DateTime date, IEnumerable<BitalinoFrame> frames)
        {
            SetId(id);
            Channel = channel;
            Frequency = frequency;
            Duration = duration;
            Date = date;
            _frames = frames;
        }
    }
}
