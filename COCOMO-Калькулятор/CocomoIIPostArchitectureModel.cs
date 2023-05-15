using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COCOMO_Калькулятор
{
    public static class CocomoIIPostArchitectureModel
    {
        static float cocomoIIPostArchitectureCoefficientA;
        static float cocomoIIPostArchitectureCoefficientB;
        static float cocomoIIPostArchitectureCoefficientC;
        static float cocomoIIPostArchitectureCoefficientD;

        public static float[][] cocomoIIPostArchitectureScaleFactorsValuesTable = new float[5][];
        public static float[][] cocomoIIPostArchitectureEffortMultipliersValuesTable = new float[17][];

        static CocomoIIPostArchitectureModel() {
            cocomoIIPostArchitectureCoefficientA = 2.94f;
            cocomoIIPostArchitectureCoefficientB = 0.91f;
            cocomoIIPostArchitectureCoefficientC = 3.67f;
            cocomoIIPostArchitectureCoefficientD = 0.28f;

            cocomoIIPostArchitectureScaleFactorsValuesTable[0] = new[] { 6.20f, 4.96f, 3.72f, 2.48f, 1.24f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[1] = new[] { 5.07f, 4.05f, 3.04f, 2.03f, 1.01f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[2] = new[] { 7.07f, 5.65f, 4.24f, 2.83f, 1.41f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[3] = new[] { 5.48f, 4.38f, 3.29f, 2.19f, 1.1f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[4] = new[] { 7.8f, 6.24f, 4.68f, 3.12f, 1.56f, 0f };

            //Факторы персонала
            cocomoIIPostArchitectureEffortMultipliersValuesTable[0] = new[] { 1.42f, 1.29f, 1f, 0.85f, 0.71f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[1] = new[] { 1.22f, 1.1f, 1f, 0.88f, 0.81f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[2] = new[] { 1.34f, 1.15f, 1f, 0.88f, 0.76f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[3] = new[] { 1.29f, 1.12f, 1f, 0.9f, 0.81f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[4] = new[] { 1.19f, 1.09f, 1f, 0.91f, 0.85f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[5] = new[] { 1.2f, 1.09f, 1f, 0.91f, 0.84f };
        
            //Факторы продукта
            cocomoIIPostArchitectureEffortMultipliersValuesTable[6] = new[] { 0.84f, 0.92f, 1f, 1.1f, 1.26f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[7] = new[] { 0.23f, 1f, 1.14f, 1.28f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[8] = new[] { 0.73f, 0.87f, 1f, 1.17f, 1.34f, 1.74f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[9] = new[] { 0.95f, 1f, 1.07f, 1.15f, 1.24f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[10] = new[] {0.81f, 0.91f, 1f, 1.11f, 1.23f };

            //Факторы платформы
            cocomoIIPostArchitectureEffortMultipliersValuesTable[11] = new[] { 1f, 1.11f, 1.29f, 1.63f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[12] = new[] { 1f, 1.05f, 1.17f, 1.46f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[13] = new[] { 0.87f, 1f, 1.15f, 1.3f };

            //Факторы проекта
            cocomoIIPostArchitectureEffortMultipliersValuesTable[14] = new[] { 1.17f, 1.09f, 1f, 0.9f, 0.78f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[15] = new[] { 1.22f, 1.09f, 1f, 0.93f, 0.86f, 0.8f };
            cocomoIIPostArchitectureEffortMultipliersValuesTable[16] = new[] { 1.43f, 1.14f, 1f, 1f, 1f };
        }

        //E-коэффициент
        public static double ECoefficient(float sumOfScaleFactors)
        {
            return cocomoIIPostArchitectureCoefficientB + 0.01f * sumOfScaleFactors;
        }

        //Трудоемкость
        public static double GetEfforts(float eaf, float sumOfScaleFactors, float amountProgramCode)
        {
            return eaf * cocomoIIPostArchitectureCoefficientA * Math.Pow(amountProgramCode, ECoefficient(sumOfScaleFactors));
        }

        //Трудоемкость без учёта SCED
        public static double GetEffortsWithoutSCED(float eafWithoutSCED, float sumOfScaleFactors, float amountProgramCode)
        {
            return eafWithoutSCED * cocomoIIPostArchitectureCoefficientA * Math.Pow(amountProgramCode, ECoefficient(sumOfScaleFactors));
        }

        //Время разработки
        public static double GetTimeToDevelop(float sced, float eafWithoutSCED, float amountProgramCode, float sumOfScaleFactors)
        {
            return sced * cocomoIIPostArchitectureCoefficientC * Math.Pow(GetEffortsWithoutSCED(eafWithoutSCED, sumOfScaleFactors, amountProgramCode), cocomoIIPostArchitectureCoefficientD + 0.2f * (ECoefficient(sumOfScaleFactors) - cocomoIIPostArchitectureCoefficientB));
        }
    }
}
