using BitalinoMonitor.Domain.PatientContext.Enums;
using BitalinoMonitor.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BitalinoMonitor.Domain.PatientContext.Entities
{
    public class Exam : Entity
    {
        List<BitalinoFrame> _frames;

        public DateTime Date { get; private set; }
        public int Channel { get; private set; }
        public long Duration { get; private set; }
        public TimeSpan DurationAsTimeSpan
        {
            get
            {
                return TimeSpan.FromMilliseconds(Duration);

            }
        }
        public int Frequency { get; private set; }
        public EExamType Type
        {
            get
            {
                return (EExamType)Channel;
            }
        }

        public IReadOnlyCollection<BitalinoFrame> Frames => _frames.ToArray();

        public Exam(int channel, int frequency, long duration, DateTime date, IEnumerable<BitalinoFrame> frames)
        {
            Channel = channel;
            Frequency = frequency;
            Duration = duration;
            Date = date;
            _frames = frames.ToList();
        }

        public Exam(Guid id, int channel, int frequency, TimeSpan duration, DateTime date, IEnumerable<BitalinoFrame> frames)
        {
            SetId(id);
            Channel = channel;
            Frequency = frequency;
            Duration = (long)duration.TotalMilliseconds;
            Date = date;
            _frames = frames.ToList();
        }

        public void AddFrame(BitalinoFrame frame)
        {
            _frames.Add(frame);
        }

        public void ClearFrames()
        {
            _frames.Clear();
        }
    }
}
