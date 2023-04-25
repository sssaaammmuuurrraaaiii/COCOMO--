using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COCOMO_Калькулятор
{
    public static class CocomoIIPostArchitectureModel
    {
        public static float[][] cocomoIIPostArchitectureScaleFactorsValuesTable = new float[5][];

        static CocomoIIPostArchitectureModel() {
            cocomoIIPostArchitectureScaleFactorsValuesTable[0] = new[] { 6.20f, 4.96f, 3.72f, 2.48f, 1.24f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[1] = new[] { 5.07f, 4.05f, 3.04f, 2.03f, 1.01f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[2] = new[] { 7.07f, 5.65f, 4.24f, 2.83f, 1.41f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[3] = new[] { 5.48f, 4.38f, 3.29f, 2.19f, 1.1f, 0f };
            cocomoIIPostArchitectureScaleFactorsValuesTable[4] = new[] { 7.8f, 6.24f, 4.68f, 3.12f, 1.56f, 0f };
        }
    }
}
