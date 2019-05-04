using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using FluentValidator.Validation;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs
{
    public class CreatePatientCommand : Notifiable, ICommand
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PhotoPath { get; set; }
        public DateTime DateOfBirth { get; set; }

        public bool IsValid()
        {
            AddNotifications(new ValidationContract()
                .HasMinLen(Name, 3, "Name", "O nome deve conter pelo menos 3 caracteres")
            );

            return Valid;
        }
    }
}
