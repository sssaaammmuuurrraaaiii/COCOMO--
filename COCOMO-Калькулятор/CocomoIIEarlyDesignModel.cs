using System;

namespace COCOMO_Калькулятор
{
    public static class CocomoIIEarlyDesignModel
    {
        static float coefficientA;
        static float coefficientB;
        static float coefficientC;
        static float coefficientD;

        public static float[][] cocomoIIEarlyDesignScaleFactorsValuesTable = new float[5][];
        public static float[][] effortMultipliersValuesTable = new float[7][];

        static CocomoIIEarlyDesignModel()
        {
            coefficientA = 2.94f;
            coefficientB = 0.91f;
            coefficientC = 3.67f;
            coefficientD = 0.28f;

            cocomoIIEarlyDesignScaleFactorsValuesTable[0] = new[] { 6.20f, 4.96f, 3.72f, 2.48f, 1.24f, 0f };
            cocomoIIEarlyDesignScaleFactorsValuesTable[1] = new[] { 5.07f, 4.05f, 3.04f, 2.03f, 1.01f, 0f };
            cocomoIIEarlyDesignScaleFactorsValuesTable[2] = new[] { 7.07f, 5.65f, 4.24f, 2.83f, 1.41f, 0f };
            cocomoIIEarlyDesignScaleFactorsValuesTable[3] = new[] { 5.48f, 4.38f, 3.29f, 2.19f, 1.1f, 0f };
            cocomoIIEarlyDesignScaleFactorsValuesTable[4] = new[] { 7.8f, 6.24f, 4.68f, 3.12f, 1.56f, 0f };

            effortMultipliersValuesTable[0] = new[] { 2.12f, 1.62f, 1.26f, 1f, 0.83f, 0.63f, 0.5f };
            effortMultipliersValuesTable[1] = new[] { 1.59f, 1.33f, 1.22f, 1f, 0.87f, 0.74f, 0.62f };
            effortMultipliersValuesTable[2] = new[] { 0.49f, 0.6f, 0.83f, 1f, 1.33f, 1.91f, 2.72f };
            effortMultipliersValuesTable[3] = new[] { 0.95f, 1f, 1.07f, 1.15f, 1.24f };
            effortMultipliersValuesTable[4] = new[] { 0.87f, 1f, 1.29f, 1.81f, 2.61f };
            effortMultipliersValuesTable[5] = new[] { 1.43f, 1.3f, 1.1f, 1f, 0.87f, 0.73f, 0.62f };
            effortMultipliersValuesTable[6] = new[] { 1.43f, 1.14f, 1f, 1f };
        }

        //E-коэффициент
        public static double ECoefficient(float sumOfScaleFactors)
        {
            return coefficientB + 0.01f * sumOfScaleFactors;
        }

        //Трудоемкость
        public static double GetEfforts(float eaf, float sumOfScaleFactors, float amountProgramCode)
        {
            return eaf * coefficientA * Math.Pow(amountProgramCode, ECoefficient(sumOfScaleFactors));
        }

        //Трудоемкость без учёта SCED
        public static double GetEffortsWithoutSCED(float eafWithoutSCED, float sumOfScaleFactors, float amountProgramCode)
        {
            return eafWithoutSCED * coefficientA * Math.Pow(amountProgramCode, ECoefficient(sumOfScaleFactors));
        }

        //Время разработки
        public static double GetTimeToDevelop(float sced, float eafWithoutSCED, float amountProgramCode, float sumOfScaleFactors)
        {
            return sced * coefficientC * Math.Pow(GetEffortsWithoutSCED(eafWithoutSCED, sumOfScaleFactors, amountProgramCode), coefficientD + 0.2f * (ECoefficient(sumOfScaleFactors) - coefficientB));
        }
    }
}
