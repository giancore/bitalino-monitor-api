using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BitalinoMonitor.Tests
{
    [TestClass]
    public class CreateCustomerCommandTests
    {
        [TestMethod]
        public void ShouldValidateWhenCommandIsValid()
        {
            var command = new CreatePatientCommand();
            command.Name = "Gian Mella";
            command.Phone = "(51) 98184-1977";
            command.DateOfBirth = new DateTime(1990, 26, 06);

            Assert.AreEqual(true, command.IsValid());
        }
    }
}
