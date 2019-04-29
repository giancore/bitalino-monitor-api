using System;
using System.Data;
using System.Data.SqlClient;
using BitalinoMonitor.Shared;

namespace BitalinoMonitor.Infra.DataContexts
{
    public class BitalinoMonitorDataContext : IDisposable
    {
        public SqlConnection Connection { get; set; }

        public BitalinoMonitorDataContext()
        {
            Connection = new SqlConnection(Settings.ConnectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }
    }
}