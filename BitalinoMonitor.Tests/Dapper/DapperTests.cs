using BitalinoMonitor.Infra.DataContexts;
using BitalinoMonitor.Infra.PatientContext.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
            var idExam = new Guid("89532c4f-6a79-4fa5-a656-6559b01763be");

            var patient = _patientRepository.GetPatientByIdExam(idExam);

            Assert.IsNotNull(patient);
        }

        [TestMethod]
        public void GetPatient()
        {
            var idExam = new Guid("fa112359-5ef3-4075-b0a2-85380173f00a");

            var patient = _patientRepository.GetPatient(idExam);

            Assert.IsNotNull(patient);
        }

        [TestMethod]
        public void GetExam()
        {
            var idExam = new Guid("89532c4f-6a79-4fa5-a656-6559b01763be");

            var patient = _patientRepository.GetExam(idExam);

            Assert.IsNotNull(patient);
        }
    }
}
