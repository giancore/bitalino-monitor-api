using BitalinoMonitor.Domain.PatientContext.Entities;
using BitalinoMonitor.Domain.PatientContext.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitalinoMonitor.Tests
{
    [TestClass]
    public class DocumentTests
    {
        private Document validDocument;
        private Document invalidDocument;

        public DocumentTests()
        {
            validDocument = new Document("28659170377");
            invalidDocument = new Document("12345678910");
        }

        [TestMethod]
        public void ShouldReturnNotificationWhenDocumentIsNotValid()
        {
            Assert.AreEqual(false, invalidDocument.Valid);
            Assert.AreEqual(1, invalidDocument.Notifications.Count);
        }

        [TestMethod]
        public void ShouldReturnNotNotificationWhenDocumentIsValid()
        {
            Assert.AreEqual(true, validDocument.Valid);
            Assert.AreEqual(0, validDocument.Notifications.Count);
        }
    }
}
