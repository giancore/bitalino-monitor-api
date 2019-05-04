using System;
using System.Collections.Generic;
using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.Queries;
using BitalinoMonitor.Domain.PatientContext.Repositories;

namespace BitalinoMonitor.Tests
{
    public class FakePatientRepository : IPatientRepository
    {
        public void Delete(Guid idPatient)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ListPatientQueryResult> Get()
        {
            throw new NotImplementedException();
        }

        public GetPatientQueryResult Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public GetPatientExamQueryResult GetExam(Guid idExam)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ListPatientExamsQueryResult> GetExams(Guid idPatient)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ListPatientExamsQueryResult> ListExams(Guid idPatient)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ListPatientExamsBitalinoFrameQueryResult> ListFrames(Guid idExam)
        {
            throw new NotImplementedException();
        }

        public void Save(Patient Patient)
        {
            
        }

        public void Save(Exam exam, Guid idPatient)
        {
            throw new NotImplementedException();
        }

        public void Update(Patient patient, Guid idPatient)
        {
            throw new NotImplementedException();
        }
    }
}
