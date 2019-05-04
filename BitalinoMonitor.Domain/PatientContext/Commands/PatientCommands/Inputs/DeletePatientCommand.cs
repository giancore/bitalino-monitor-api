using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs
{
    public class DeletePatientCommand : Notifiable, ICommand
    {
        public Guid IdPatient { get; set; }

        public bool IsValid()
        {
            return Valid;
        }
    }
}
