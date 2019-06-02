using BitalinoMonitor.Infra.DataContexts;
using BitalinoMonitor.Infra.PatientContext.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace BitalinoMonitor.Tests
{
    [TestClass]
    public class DapperTests
    {
        PatientRepository _patientRepository;

        public DapperTests()
        {
            var ctx = new BitalinoMonitorDataContext();
            _patientRepository = new PatientRepository(ctx);
        }

        [TestMethod]
        public void GetPatientByIdExam()
        {
            var idExam = new Guid("5a519201-e3e4-47e4-9f35-3b613cfc8b0d");
            var patient = _patientRepository.GetPatientByIdExam(idExam);

            Assert.IsNotNull(patient);
        }

        [TestMethod]
        public void GetPatient()
        {
            var idExam = new Guid("5a519201-e3e4-47e4-9f35-3b613cfc8b0d");
            var patient = _patientRepository.GetPatient(idExam);

            Assert.IsNotNull(patient);
        }

        [TestMethod]
        public void GetExam()
        {
            var idExam = new Guid("5a519201-e3e4-47e4-9f35-3b613cfc8b0d");
            var exam = _patientRepository.GetExam(idExam);

            Assert.IsNotNull(exam);
        }

        [TestMethod]
        public void BulkDuplicateFrames()
        {
            var idExam = new Guid("5a519201-e3e4-47e4-9f35-3b613cfc8b0d");
            var exam = _patientRepository.GetExam(idExam);
            Assert.IsNotNull(exam);

            var patient = _patientRepository.GetPatientByIdExam(idExam);
            Assert.IsNotNull(patient);

            var newExamID = Guid.NewGuid();

            exam.Frames.ToList().ForEach(f => f.SetId(Guid.NewGuid()));
            exam.SetId(newExamID);

            _patientRepository.Save(exam, patient.Id);
        }
    }
}
