using System.ComponentModel;

namespace COCOMO_Калькулятор
{
    public class ProjectTypes
    {
        public enum ProjectType
        {
            [Description("Распространённый")]
            Organic = 0,
            [Description("Встроенный")]
            Embedded = 1,
            [Description("Полунезависимый")]
            SemiDetached = 2
        }
    }
}

