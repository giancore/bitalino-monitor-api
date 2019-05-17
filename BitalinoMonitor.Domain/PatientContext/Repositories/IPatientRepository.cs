using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Queries;
using System;
using System.Collections.Generic;

namespace BitalinoMonitor.Domain.PatientContext.Repositories
{
    public interface IPatientRepository
    {
        Patient GetPatient(Guid id);
        Patient GetPatientByIdExam(Guid idExam);
        Exam GetExam(Guid idExam);

        GetPatientExamQueryResult GetExamAsQueryResult(Guid idExam);
        IEnumerable<ListPatientQueryResult> GetPatients();
        IEnumerable<ListPatientExamsQueryResult> ListExams(Guid idPatient);
        IEnumerable<ListPatientExamsBitalinoFrameQueryResult> ListFrames(Guid idExam);
        void Save(Patient patient);
        void Update(Patient patient, Guid idPatient);
        void Save(Exam exam, Guid idPatient);
        void Delete(Guid idPatient);
    }
}
