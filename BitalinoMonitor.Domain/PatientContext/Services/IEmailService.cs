using BitalinoMonitor.Domain.PatientContext.Entities;

namespace BitalinoMonitor.Domain.PatientContext.Services
{
    public interface IEmailService
    {
        void Send(string to, string from, string subject, string body);
    }
}