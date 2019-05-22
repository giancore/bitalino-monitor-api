namespace BitalinoMonitor.Domain.PatientContext.Services.Filter
{
    /// <summary>
    /// Fator de filtro
    /// </summary>
    public class FilterCoefficient
    {
        public double[,] Numerator;
        public double[,] Denominator;
        public int Sections;
        public int Order;
    }
}
