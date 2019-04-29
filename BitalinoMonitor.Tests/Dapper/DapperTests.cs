using BitalinoMonitor.Infra.DataContexts;
using BitalinoMonitor.Infra.PatientContext.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitalinoMonitor.Tests
{
    [TestClass]
    public class DapperTests
    {
        [TestMethod]
        public void DeleteMe()
        {
            var ctx = new BitalinoMonitorDataContext();
            var repo = new PatientRepository(ctx);

            //Assert.AreEqual(repo.CheckDocument("99999999999"), true);
        }

        [TestMethod]
        public void Get()
        {
            var ctx = new BitalinoMonitorDataContext();
            var repo = new PatientRepository(ctx);

            var patients = repo.Get();
        }
    }
}
