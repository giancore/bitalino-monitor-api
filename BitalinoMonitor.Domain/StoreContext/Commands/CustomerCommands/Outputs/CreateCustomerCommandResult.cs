using System;
using BitalinoMonitor.Shared.Commands;
using FluentValidator;
using FluentValidator.Validation;

namespace BitalinoMonitor.Domain.PatientContext.CustomerCommands.Inputs
{
    public class CreateCustomerCommandResult : ICommandResult
    {
        public CreateCustomerCommandResult(bool success, string message, object data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}