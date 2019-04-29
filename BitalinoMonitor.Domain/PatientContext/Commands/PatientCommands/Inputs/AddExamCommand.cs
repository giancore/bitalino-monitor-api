using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System;
using System.Collections.Generic;

namespace BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs
{
    public class AddExamCommand : Notifiable, ICommand
    {
        public Guid IdPatient { get; set; }
        public DateTime Date { get; set; }
        public int Channel { get; set; }
        public List<BitalinoFramesCommand> Frames { get; set; }

        public AddExamCommand()
        {
            Frames = new List<BitalinoFramesCommand>();
        }

        bool ICommand.IsValid()
        {
            return Valid;
        }
    }

    public class BitalinoFramesCommand
    {
        public string Identifier { get; set; }
        public int Seq { get; set; }
        public int[] Analog { get; set; }
        public int[] Digital { get; set; }
    }
}
