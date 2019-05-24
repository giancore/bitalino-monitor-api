using BitalinoMonitor.Domain.PatientContext.Enums;
using System;

namespace BitalinoMonitor.Domain.PatientContext.Services
{
    public static class SensorService
    {
        static double CutoffFrequencyECG = 40;
        static double CutoffFrequencyEEG = 48;
        static double CutoffFrequencyEMG = 480;
        static double CutoffFrequencyEDA = 2.8;

        static readonly int gainEcg = 1100;
        static readonly int gainEcmg = 1009;
        static readonly int gainEeg = 40000;
        static readonly double vcc = 3.3;

        /*
        * The number of bits for each channel depends on the resolution of the Analog-to-Digital Converter (ADC); in
        * BITalino the first four channels are sampled using 10-bit resolution (n = 10), while the last two may be sampled using
        * 6-bit (n = 6).*/
        static readonly int n = 10;

        public static double GetTransferFunction(double value, EExamType type)
        {
            switch (type)
            {
                case EExamType.Electromyography:
                    return CalculateElectromyographyValue(value);
                case EExamType.Electrocardiography:
                    return CalculateElectrocardiographyValue(value);
                case EExamType.ElectrodermalActivity:
                    return CalculateElectrodermalActivityValue(value);
                case EExamType.Electroencephalography:
                    return CalculateElectroencephalographyValue(value);
            }

            return 0.0;
        }

        public static double GetCuttoffFrequency(EExamType type)
        {
            switch (type)
            {
                case EExamType.Electromyography:
                    return CutoffFrequencyEMG;
                case EExamType.Electrocardiography:
                    return CutoffFrequencyECG;
                case EExamType.ElectrodermalActivity:
                    return CutoffFrequencyEDA;
                case EExamType.Electroencephalography:
                    return CutoffFrequencyEEG;
                default:
                    return 0.0;
            }
        }

        static double CalculateElectrocardiographyValue(double adc)
        {
            double partOne = (adc / Math.Pow(2, n)) - 0.5;
            double result = (partOne * vcc) / gainEcg;

            double resultInMilliVolts = result * 1000;

            return resultInMilliVolts;
        }

        static double CalculateElectrodermalActivityValue(double adc)
        {
            double constant = 0.132;
            double partOne = (adc / Math.Pow(2, n));
            double resultInMicroSeconds = (partOne * vcc) / constant;

            return resultInMicroSeconds;
        }

        static double CalculateElectromyographyValue(double adc)
        {
            double partOne = (adc / Math.Pow(2, n)) - 0.5;
            double result = (partOne * vcc) / gainEcmg;

            double resultInMilliVolts = result * 1000;

            return resultInMilliVolts;
        }

        static double CalculateElectroencephalographyValue(double adc)
        {
            double partOne = (adc / Math.Pow(2, n)) - 0.5;
            double result = (partOne * vcc) / gainEeg;

            double resultInMicroVolts = result * 0.000001;

            return resultInMicroVolts;
        }
    }
}
