using System;
using BitalinoMonitor.Domain.PatientContext.Enums;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;

namespace BitalinoMonitor.Domain.PatientContext.CustomerCommands.Inputs
{
    public class AddAddressCommand : Notifiable, ICommand
    {
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string Number { get;  set; }
        public string Complement { get; set; }
        public string District { get;  set; }
        public string City { get;  set; }
        public string State { get;  set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }

        bool ICommand.IsValid()
        {
            return Valid;
        }
    }
}