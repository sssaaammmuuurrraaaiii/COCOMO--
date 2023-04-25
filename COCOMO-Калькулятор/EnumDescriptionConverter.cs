using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Data;

namespace COCOMO_Калькулятор
{
    public class EnumDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObject)
        {
            FieldInfo fieldInfo = enumObject.GetType().GetField(enumObject.ToString());
            object[] attributeArray = fieldInfo.GetCustomAttributes(false);

            if (attributeArray.Length == 0)
            {
                return enumObject.ToString();
            }  else {
                DescriptionAttribute descriptionAttribute = null;

                foreach (var attribute in attributeArray) {
                    if (attribute is DescriptionAttribute)  {
                        descriptionAttribute = attribute as DescriptionAttribute;
                    }
                }

                if (descriptionAttribute != null) {
                    return descriptionAttribute.Description;
                }

                return enumObject.ToString();
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumObject = (Enum)value;
            string description = GetEnumDescription(enumObject);
            return description;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return String.Empty;
        }
    }
}
