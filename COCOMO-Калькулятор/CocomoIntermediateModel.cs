using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace COCOMO_Калькулятор
{
    public static class CocomoIntermediateModel
    {
        //Таблица коэффициентов
        static float[][] cocomoIntermadiateModelTable = new float[3][];
        public static float[][] costDriversTable = new float[15][];

        static CocomoIntermediateModel()
        {
            cocomoIntermadiateModelTable[0] = new[] { 3.2f, 1.05f, 2.5f, 0.38f };
            cocomoIntermadiateModelTable[1] = new[] { 2.8f, 1.2f, 2.5f, 0.32f };
            cocomoIntermadiateModelTable[2] = new[] { 3.0f, 1.12f, 2.5f, 0.35f };

            //Характеристики продукта
            costDriversTable[0] = new[] { 0.75f, 0.88f, 1f, 1.15f, 1.40f };
            costDriversTable[1] = new[] { 0.94f, 1f, 1.08f, 1.16f };
            costDriversTable[2] = new[] { 0.7f, 0.85f, 1f, 1.15f, 1.3f, 1.65f };

            //Характеристики аппаратного обеспечения
            costDriversTable[3] = new[] { 1f, 1.11f, 1.3f, 1.66f };
            costDriversTable[4] = new[] { 1f, 1.06f, 1.21f, 1.56f };
            costDriversTable[5] = new[] { 0.87f, 1f, 1.15f, 1.3f };
            costDriversTable[6] = new[] { 0.87f, 1f, 1.07f, 1.15f };

            //Характеристики персонала
            costDriversTable[7] = new[] { 1.46f, 1.19f, 1f, 0.86f, 0.71f };
            costDriversTable[8] = new[] { 1.29f, 1.13f, 1f, 0.91f, 0.82f };
            costDriversTable[9] = new[] { 1.42f, 1.17f, 1f, 0.86f, 0.70f };
            costDriversTable[10] = new[] { 1.21f, 1.10f, 1f, 0.9f };
            costDriversTable[11] = new[] { 1.14f, 1.07f, 1f, 0.95f };

            //Характеристики проекта
            costDriversTable[12] = new[] { 1.24f, 1.1f, 1f, 0.91f, 0.82f };
            costDriversTable[13] = new[] { 1.24f, 1.1f, 1f, 0.91f, 0.83f };
            costDriversTable[14] = new[] { 1.23f, 1.08f, 1f, 1.04f, 1.1f };
        }

        //Трудоемкость
        public static double GetEfforts(float eaf, float amountProgramCode, ProjectTypes.ProjectType projectType)
        {
            return eaf * cocomoIntermadiateModelTable[(int)projectType][0] * (Math.Pow(amountProgramCode, cocomoIntermadiateModelTable[(int)projectType][1]));
        }

        //Время разработки
        public static double GetTimeToDevelop(float eaf, float amountProgramCode, ProjectTypes.ProjectType projectType)
        {
            return cocomoIntermadiateModelTable[(int)projectType][2] * (Math.Pow(GetEfforts(eaf, amountProgramCode, projectType), cocomoIntermadiateModelTable[(int)projectType][3]));
        }
    }
}
