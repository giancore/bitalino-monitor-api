using BitalinoMonitor.Domain.PatientContext.Commands.PatientCommands.Inputs;
using BitalinoMonitor.Domain.PatientContext.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BitalinoMonitor.Tests
{
    [TestClass]
    public class CustomerHandlerTests
    {
        [TestMethod]
        public void ShouldRegisterCustomerWhenCommandIsValid()
        {
            var command = new CreatePatientCommand();
            command.Name = "Gian Mella";
            command.Phone = "(51) 98184-1977";
            command.DateOfBirth = new DateTime(1990, 26, 06);

            var handler = new PatientHandler(new FakePatientRepository());
            var result = handler.Handle(command);

            Assert.AreNotEqual(null, result);
            Assert.AreEqual(true, handler.Valid);
        }
    }
}
