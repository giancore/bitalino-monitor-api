using BitalinoMonitor.Domain.PatientContext.Services;

namespace BitalinoMonitor.Infra.PatientContext.Services
{
    public class EmailService : IEmailService
    {
        public void Send(string to, string from, string subject, string body)
        {
            // TODO: Implementar
        }
    }
}