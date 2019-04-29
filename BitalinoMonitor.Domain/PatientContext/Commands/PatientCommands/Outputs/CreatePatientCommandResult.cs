using BitalinoMonitor.Shared.Commands;

namespace BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Outputs
{
    public class CreatePatientCommandResult : ICommandResult
    {
        public CreatePatientCommandResult(bool success, string message, object data)
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
