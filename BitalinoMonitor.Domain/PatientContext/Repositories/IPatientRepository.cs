using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Queries;
using System;
using System.Collections.Generic;

namespace BitalinoMonitor.Domain.PatientContext.Repositories
{
    public interface IPatientRepository
    {
        GetPatientQueryResult Get(Guid id);
        GetPatientExamQueryResult GetExam(Guid idExam);
        IEnumerable<ListPatientQueryResult> Get();
        IEnumerable<ListPatientExamsQueryResult> ListExams(Guid idPatient);
        IEnumerable<ListPatientExamsBitalinoFrameQueryResult> ListFrames(Guid idExam);
        void Save(Patient patient);
        void Update(Patient patient, Guid idPatient);
        void Save(Exam exam, Guid idPatient);
        void Delete(Guid idPatient);
    }
}
