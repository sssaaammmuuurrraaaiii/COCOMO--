using System;
using System.Windows.Threading;
using System.Windows;

namespace COCOMO_Калькулятор
{
    public static class RefreshUI
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uIElement)
        {
            uIElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
