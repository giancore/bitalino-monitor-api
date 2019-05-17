using BitalinoMonitor.Domain.PatientContext.Entities;

namespace BitalinoMonitor.Domain.PatientContext.Services
{
    public interface IOpenEHRService
    {
        string CreateCompositionAsXml(Patient patient, Exam exam);
    }
}
