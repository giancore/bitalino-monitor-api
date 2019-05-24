using BitalinoMonitor.Domain.PatientContext.Enums;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Services.Filter
{
    //Baseado no código disponível em https://github.com/nakajima-hro/ButterworthFilter
    public class ButterworthFilterService
    {
        FilterCoefficient _coeff;
        double[,] u;

        // Fator de filtro
        public FilterCoefficient Coeff
        {
            set
            {
                _coeff = value;

                // Atraso na criação do objeto buffer
                u = new double[_coeff.Sections, 2];
            }
            get
            {
                return _coeff;
            }
        }

        public ButterworthFilterService(double frequency, EExamType type)
        {
            int order = 4;
            double cutoff = SensorService.GetCuttoffFrequency(type);
            double Wd = cutoff / frequency * 2;

            Coeff = ButterworthLowPassFilter(order, Wd);
        }

        /// <summary>
        /// Filtro IIR (Transposição direta de tipo II, segunda ordem de múltiplos estágios)
        /// </summary>
        /// <param name="x">Dados</param>
        /// <returns>Dados filtrados</returns>
        public double Filter(double x)
        {
            double y = 0;

            for (var k = 0; k < _coeff.Sections; k++)
            {
                // Cálculo de saída
                y = (_coeff.Numerator[k, 0] * x + u[k, 0]) / _coeff.Denominator[k, 0];

                // Atraso no cálculo do buffer
                u[k, 0] = _coeff.Numerator[k, 1] * x - _coeff.Denominator[k, 1] * y + u[k, 1];
                u[k, 1] = _coeff.Numerator[k, 2] * x - _coeff.Denominator[k, 2] * y;

                // Entrada do próximo estágio
                x = y;
            }

            // Saída do resultado
            return y;
        }

        /// <summary>
        /// Cálculo do coeficiente do filtro digital do filtro de passagem lenta Butterworth
        /// </summary>
        /// <param name="order">Ordem de filtro</param>
        /// <param name="Wd">Frequência de corte normalizada do filtro ou frequência central (normalizada na frequência de Nyquist)</param>
        /// <returns>FilterCoefficient - Coeficientes do filtro digital transformado bilinearmente</returns>
        FilterCoefficient ButterworthLowPassFilter(int order, double Wd)
        {
            // Pré-distorção da frequência de corte adimensional (digital, valor de projeto) (recalcula a frequência do filtro analógico de acordo com o valor de projeto)
            double Wc = 2 / Math.PI * Math.Tan(Math.PI / 2 * Wd);
            //double Wc = 2 * Math.Atan(Math.PI / 2 * Wd);

            // Mesmo os filtros de ordem impar de determinação
            bool odd = order % 2 != 0;

            // Número de estágios de filtro
            int sections = (int)((order + 1) / 2);

            // Inicialize o array de coeficientes pelo número de estágios de filtro
            var a = new double[sections, 3];
            var b = new double[sections, 3];
            var aAnalog = new double[sections, 3];
            var bAnalog = new double[sections, 3];

            // Calcular os polos do polinômio de Butterworth normalizado
            var pk = NormalizedButterworthPoles(order);

            if (odd)
            {
                // Cálculo dos coeficientes do filtro analógico （a2s^2+a1s+a0) / (b2s^2+b1s+b0)
                // 1º estágio (primário)
                aAnalog[0, 0] = 1;
                aAnalog[0, 1] = 0;
                aAnalog[0, 2] = 0;

                bAnalog[0, 0] = 1;
                bAnalog[0, 1] = 1;
                bAnalog[0, 2] = 0;

                // Segundo e subseqüentes estágios (secundário)
                for (var k = 1; k < sections; k++)
                {
                    aAnalog[k, 0] = 1;
                    aAnalog[k, 1] = 0;
                    aAnalog[k, 2] = 0;

                    bAnalog[k, 0] = 1;
                    bAnalog[k, 1] = 2 * Math.Cos(pk[k]);
                    bAnalog[k, 2] = 1;
                }
            }
            else
            {
                // Número par
                // Cálculo dos coeficientes do filtro analógico
                for (var k = 0; k < sections; k++)
                {
                    aAnalog[k, 0] = 1;
                    aAnalog[k, 1] = 0;
                    aAnalog[k, 2] = 0;

                    bAnalog[k, 0] = 1;
                    bAnalog[k, 1] = 2 * Math.Cos(pk[k]);
                    bAnalog[k, 2] = 1;
                }
            }

            // Transformação bilinear
            var filter = BilinearTransform(order, Wc, aAnalog, bAnalog);


            return filter;
        }

        /// <summary>
        /// Calcular polos de polinômios de Butterworth normalizados
        /// </summary>
        /// <param name="order">Ordem do filtro lowpass</param>
        /// <returns>double[] s Ângulo da metade superior do poste no plano</returns>
        private static double[] NormalizedButterworthPoles(int order)
        {
            bool odd = order % 2 != 0;

            // Número de polos (metade superior do plano s)
            int numberOfPoles = (int)((order + 1) / 2);

            // Pólos de polinômios de Butterworth normalizados
            var poleAngles = new double[numberOfPoles];

            if (odd)
            {
                // Quando um número ímpar
                for (var k = 0; k < numberOfPoles; k++)
                {
                    // 極は、kPI/N ( k=0,1,... (order+1)/2, N:order)
                    // O polo é、kPI/N ( k=0,1,... (order+1)/2, N:order)
                    poleAngles[k] = k * Math.PI / order;
                }
            }
            else
            {
                // Quando um número par
                for (var k = 0; k < numberOfPoles; k++)
                {
                    // 極は(2k-1)PI/2N ( k=0,1,... order/2, N:order)
                    // O polo é, (2k-1)PI/2N ( k=0,1,... order/2, N:order)
                    poleAngles[k] = (2 * (k + 1) - 1) * Math.PI / 2 / order;
                }
            }

            return poleAngles;
        }

        /// <summary>
        /// Transformação bilinear
        /// </summary>
        /// <param name="order">Ordem de filtro</param>
        /// <param name="Wa">Frequência de corte normalizada do filtro ou frequência central (normalizada na frequência de Nyquist)</param>
        /// <param name="aAnalog">Coeficiente do numerador do filtro analógico</param>
        /// <param name="bAnalog">Coeficiente de denominador do filtro analógico</param>
        /// <returns>FilterCoefficient - Coeficientes do filtro digital transformado bilinearmente</returns>
        private static FilterCoefficient BilinearTransform(int order, double Wa, double[,] aAnalog, double[,] bAnalog)
        {
            // Matriz para armazenamento de transformações bilineares
            var a = new double[aAnalog.GetLength(0), aAnalog.GetLength(1)];
            var b = new double[bAnalog.GetLength(0), bAnalog.GetLength(1)];

            // Julgamento uniforme e estranho de ordem de filtro
            bool odd = order % 2 != 0;

            // Número de seções
            int sections = aAnalog.GetLength(0);

            // Transformar o coeficiente de transformação bilinear
            double h = 1 / (Wa * Math.PI / 2);

            for (var k = 0; k < sections; k++)
            {
                double BB;

                if (k == 0 && odd)
                {
                    // Low-pass high-pass somente para primeira ordem, primeira ordem
                    // Inverso do coeficiente b0 do denominador
                    BB = 1 / (h + bAnalog[0, 0]);
                    a[k, 0] = BB * (aAnalog[0, 1] * h + aAnalog[0, 0]);
                    a[k, 1] = BB * (-aAnalog[0, 1] * h + aAnalog[0, 0]);
                    a[k, 2] = 0;

                    b[k, 0] = BB * (h + bAnalog[0, 0]);
                    b[k, 1] = BB * (-h + bAnalog[0, 0]);
                    b[k, 2] = 0;
                }
                else
                {
                    // Inverso do coeficiente b0 do denominador
                    BB = 1 / (h * h + bAnalog[k, 1] * h + bAnalog[k, 2]);

                    a[k, 0] = BB * (aAnalog[k, 2] * h * h + aAnalog[k, 1] * h + aAnalog[k, 0]);
                    a[k, 1] = BB * 2 * (-aAnalog[k, 2] * h * h + aAnalog[k, 0]);
                    a[k, 2] = BB * (aAnalog[k, 2] * h * h - aAnalog[k, 1] * h + aAnalog[k, 0]);

                    b[k, 0] = BB * (h * h + bAnalog[k, 1] * h + bAnalog[k, 2]);
                    b[k, 1] = BB * (-2 * h * h + 2 * bAnalog[k, 2]);
                    b[k, 2] = BB * (h * h - bAnalog[k, 1] * h + bAnalog[k, 2]);
                }
            }

            // Reúna os ganhos na primeira seção
            double gain = 1;
            for (var k = 0; k < sections; k++)
            {
                gain *= a[k, 0];
                a[k, 2] /= a[k, 0];
                a[k, 1] /= a[k, 0];
                a[k, 0] /= a[k, 0];
            }
            a[0, 0] *= gain;
            a[0, 1] *= gain;
            a[0, 2] *= gain;

            var filterValue = new FilterCoefficient();

            filterValue.Numerator = a;
            filterValue.Denominator = b;
            filterValue.Sections = sections;
            filterValue.Order = order;

            return filterValue;
        }
    }
}
