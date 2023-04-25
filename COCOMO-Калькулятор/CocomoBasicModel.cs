using System;

namespace COCOMO_Калькулятор
{
    public static class CocomoBasicModel
    {
        //Таблица коэффициентов
        static float[][] cocomoBasicModelTable = new float[3][];

        static CocomoBasicModel()
        {
            cocomoBasicModelTable[0] = new[] { 2.4f, 1.05f, 2.5f, 0.38f };
            cocomoBasicModelTable[1] = new[] { 3.6f, 1.20f, 2.5f, 0.32f };
            cocomoBasicModelTable[2] = new[] { 3.0f, 1.12f, 2.5f, 0.35f };
        }

        //Трудоемкость
        public static double GetEfforts(float amountProgramCode, ProjectTypes.ProjectType projectType)
        {
            return cocomoBasicModelTable[(int)projectType][0] * (Math.Pow(amountProgramCode, cocomoBasicModelTable[(int)projectType][1]));
        }

        //Время разработки
        public static double GetTimeToDevelop(float amountProgramCode, ProjectTypes.ProjectType projectType)
        {
            return cocomoBasicModelTable[(int)projectType][2] * (Math.Pow(GetEfforts(amountProgramCode, projectType), cocomoBasicModelTable[(int)projectType][3]));
        }   
    }
}
