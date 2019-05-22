using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs
{
    public class GetExamCommand : Notifiable, ICommand
    {
        public Guid IdExam { get; set; }

        bool ICommand.IsValid()
        {
            return Valid;
        }
    }
}
