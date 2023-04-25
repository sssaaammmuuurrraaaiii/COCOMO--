using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Microsoft.Win32;

using static COCOMO_Калькулятор.ProjectTypes;
using static COCOMO_Калькулятор.CocomoIntermediateModel;
using static COCOMO_Калькулятор.CocomoIIEarlyDesignModel;
using static COCOMO_Калькулятор.CocomoIIPostArchitectureModel;

using Excel = Microsoft.Office.Interop.Excel;
using Application = System.Windows.Application;
using Window = System.Windows.Window;
using Microsoft.Office.Interop.Excel;

namespace COCOMO_Калькулятор
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _writeToExcel = false;

        private string _fileName;

        private Excel.Application _xlExcelApp;
        private Workbook _xlWorkBook;
        private Worksheet _xlWorkSheet;
        private Range _xlRange;

        public float reguiredSoftwareReliabilityValue = 1f;
        public float sizeOfApplicationDatabaseValue = 1f;
        public float complexityOfProductValue = 1f;
        public float runTimePerformanceConstraintsValue = 1f;
        public float memoryConstraintsValue = 1f;
        public float volatalityOfVirtualMachineEnvironmentValue = 1f;
        public float reguiredTurnaboutTimeValue = 1f;
        public float analystCapabilityValue = 1f;
        public float softwareEngineerCapabilityValue = 1f;
        public float applicationsExperienceValue = 1f;
        public float virtualMachineExperienceValue = 1f;
        public float programmingLanguageExperienceValue = 1f;
        public float useOfSoftwareToolsValue = 1f;
        public float applicationOfSoftwareEngineeringMethodsValue = 1f;
        public float reguiredDevelopmentScheduleValue = 1f;
        public float cocomoIntermediateEAF = 1f;

        public float PERSValue = 1f;
        public float PREXValue = 1f;
        public float RCPXValue = 1f;
        public float RUSEValue = 1f;
        public float PDIFValue = 1f;
        public float FCILValue = 1f;
        public float SCEDValue = 1f;
        public float cocomoIIEarlyDesignEAF = 1f;
        public float cocomoIIEarlyDesignEAFWithoutSCED = 1f;

        public float PRECValue = 0f;
        public float FLEXValue = 0f;
        public float RESLValue = 0f;
        public float TEAMValue = 0f;
        public float PMATValue = 0f;
        public float sumOfScaleFactors = 0f;

        public MainWindow()
        {
            InitializeComponent();

            
            MessageBoxResult messageBox_enableWriteToExcelFile = MessageBox.Show
                (
                "Включить запись данных в Excel-файл?",
                "Подтверждения действия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );

            switch (messageBox_enableWriteToExcelFile)
            {
                case MessageBoxResult.Yes:
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = "Выберите файл электронных таблиц Excel для сохранения данных";
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    openFileDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm | All files (*.*)|*.*";
                    openFileDialog.DefaultExt = ".xlsx";
                    openFileDialog.AddExtension = true;
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;

                    Nullable<bool> access = openFileDialog.ShowDialog();
                    if (access == true) {
                        _fileName = openFileDialog.FileName;

                        _xlExcelApp = new Excel.Application();
                        _xlExcelApp.DisplayAlerts = true;
                        _xlWorkBook = _xlExcelApp.Workbooks.Open(_fileName, 0, false, 5, "", "",
                            true, XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                        _writeToExcel = true;
                    } else {
                        _writeToExcel = false;
                    }
                   
                    break;
                case MessageBoxResult.No:
                    _writeToExcel = false;
                    break;
            }

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0);
            dispatcherTimer.Tick += new EventHandler(CheckNUpdateUIStatesNProperties);
            dispatcherTimer.Start();

            CocomoBasic_TextBlock_DescriptionOfModel.Text = "Модель этого уровня - двухпараметрическая. " +
                "В качестве параметров выступают тип проекта и объём программы (число строк кода). " +
                "Модель этого уровня подходит для ранней быстрой приблизительной оценки затрат, но точность её весьма низкая, " +
                "т. к. не учитываются такие факторы, как квалификация персонала, характеристики оборудования, опыт применения " +
                "современных методов разработки программного обеспечения и современных инструментальных средств разработки, и др.";

            CocomoIntermediate_TextBlock_DescriptionOfModel.Text = "На этом уровне базовая модель уточнена за счет ввода дополнительных " +
                "15 «атрибутов стоимости» (или факторов затрат) Cost Drivers, которые сгруппированы по четырем категориям.";

            CocomoIIEarlyDesign_TextBlock_DescriptionOfModel.Text = "Предварительная оценка трудоёмкости программного проекта (Early Design). " +
                "Для этой оценки необходимо оценить для проекта уровень 5-и факторов масштаба (Scale Factors) и 7-и множителей трудоёмкости (Effort Multipliers).";

            CocomoIIPostArchitecture_TextBlock_DescriptionOfModel.Text = "Детальная оценка после проработки архитектуры (Post Archirecture). Для этой оценки необходимо " +
                "оценить для проекта уровень 5-и факторов масштаба (Scale Factors) и 17-и множителей трудоёмкости (Effort Multipliers).";


            CocomoBasic_ComboBox_ProjectTypes.ItemsSource = Enum.GetValues(typeof(ProjectType)).Cast<ProjectType>();
            CocomoIntermediate_ComboBox_ProjectTypes.ItemsSource = Enum.GetValues(typeof(ProjectType)).Cast<ProjectType>();
        }

        private void CheckNUpdateUIStatesNProperties(object sender, EventArgs e)
        {
            #region COCOMO Basic
            if (CocomoBasic_TextBox_AmountProgramCodeValue.Text != string.Empty &&
                CocomoBasic_ComboBox_ProjectTypes.SelectedItem != null &&
                CocomoBasic_Button_GetQuote.IsEnabled == false) {
                CocomoBasic_Button_GetQuote.IsEnabled = true;
            }
            CocomoBasic_Button_GetQuote.Refresh();

            if (CocomoBasic_TextBox_LaboriousnessValue.Text != string.Empty &&
                CocomoBasic_TextBox_TimeToDevelopeValue.Text != string.Empty &&
                CocomoBasic_Button_ClearValues.IsEnabled == false) {
                CocomoBasic_Button_ClearValues.IsEnabled = true;
            }
            CocomoBasic_Button_ClearValues.Refresh();
            #endregion

            #region COCOMO Intermediate
            if (CocomoIntermediate_TextBox_AmountProgramCodeValue.Text != string.Empty &&
                CocomoIntermediate_ComboBox_ProjectTypes.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedItem != null &&
                CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedItem != null &&
                CocomoIntermediate_Button_GetQuote.IsEnabled == false) {
                CocomoIntermediate_Button_GetQuote.IsEnabled = true;
            }
            CocomoIntermediate_Button_GetQuote.Refresh();

            if (CocomoIntermediate_TextBox_LaboriousnessValue.Text != string.Empty &&
                CocomoIntermediate_TextBox_TimeToDevelopeValue.Text != string.Empty &&
                CocomoIntermediate_TextBox_EAFValue.Text != string.Empty &&
                CocomoIntermediate_Button_ClearValues.IsEnabled == false) {
                CocomoIntermediate_Button_ClearValues.IsEnabled = true;
            }
            CocomoIntermediate_Button_ClearValues.Refresh();
            #endregion

            #region COCOMO II Early Design
            if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedItem != null &&
                CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedItem != null &&
                CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedItem != null &&
                CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedItem != null &&
                CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedItem != null &&
                CocomoIIEarlyDesign_Button_GetSumOfScaleFactors.IsEnabled == false) {
                CocomoIIEarlyDesign_Button_GetSumOfScaleFactors.IsEnabled = true;
                #endregion
            }
            CocomoIIEarlyDesign_Button_GetSumOfScaleFactors.Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBox_exitFromApplication = MessageBox.Show
                (
                "Вы уверены, что хотите выйти из программы?",
                "Подтвердение действия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );

            switch (messageBox_exitFromApplication)
            {
                case MessageBoxResult.Yes:
                    if (_writeToExcel == true) {
                        _xlWorkBook.Close(0);
                        _xlExcelApp.Quit();
                    }
                   
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.No:
                    e.Cancel = true;
                    break;
            }
        }

        private void All_TextBoxes_AmountProgramCodeValue_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MenuItem_StartOver_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBox_startOver = MessageBox.Show
                (
                "Начать новую сессию?",
                "Подтверждение действия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
                );

            switch (messageBox_startOver)
            {
                case MessageBoxResult.Yes:
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.Arguments = "/C choice /C Y /N /D Y /T 1 & START \"\" \"" + 
                        Assembly.GetEntryAssembly().Location + "\"";
                    processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    processStartInfo.CreateNoWindow = true;
                    processStartInfo.FileName = "COCOMO-Калькулятор.exe";
                    Process.Start(processStartInfo);
                    Process.GetCurrentProcess().Kill();
                    break;
                case MessageBoxResult.No:
                    e.Handled = true;
                    break;
            }
        }

        private void MenuItem_AboutDeveloper_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_AboutProgram_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region COCOMO Basic
        private void CocomoBasic_ComboBox_ProjectTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoBasic_ComboBox_ProjectTypes.SelectedIndex == 0) {
                CocomoBasic_GroupBox_TextBlock_ProjectsDescription.Text = "Распространенный тип характеризуется тем, что проект выполняется небольшой группой специалистов, " +
                    "имеющих опыт в создании подобных изделий и опыт применения технологических средств. Условия работы стабильны, и изделие имеет относительно " +
                    "невысокую сложность.";

                if (_writeToExcel == true) {
                    _xlWorkSheet = (Excel.Worksheet)_xlExcelApp.Worksheets.get_Item(1);
                    _xlWorkSheet.Name = "Basic Model|" + CocomoBasic_ComboBox_ProjectTypes.SelectedItem.ToString();
                }
            } else if (CocomoBasic_ComboBox_ProjectTypes.SelectedIndex == 1) {
                CocomoBasic_GroupBox_TextBlock_ProjectsDescription.Text = "Встроенный тип характеризуется очень жесткими требованиями на программный продукт, " +
                   "интерфейсы, параметры ЭВМ. Как правило, у таких изделий высокая степень новизны и планирование работ осуществляется при " +
                   "недостаточной информации, как о самом изделии, так и об условиях работы. Встроенный проект требует больших затрат на " +
                   "изменения и исправления.";

                if (_writeToExcel == true) {
                    _xlWorkSheet = (Excel.Worksheet)_xlExcelApp.Worksheets.get_Item(2);
                    _xlWorkSheet.Name = "Basic Model|" + CocomoBasic_ComboBox_ProjectTypes.SelectedItem.ToString();
                }
            } else if (CocomoBasic_ComboBox_ProjectTypes.SelectedIndex == 2) {
                CocomoBasic_GroupBox_TextBlock_ProjectsDescription.Text = "Полунезависимый тип занимает промежуточное положение между распространенным и встроенным – это проекты средней " +
                    "сложности. Исполнители знакомы лишь c некоторыми характеристиками (или компонентами) создаваемой системы, имеют средний опыт работы с подобными изделиями, " +
                    "изделие имеет элемент новизны. Только часть требований к изделию жестко фиксируется, в остальном разработки имеют степени выбора.";

                if (_writeToExcel == true) {
                    _xlWorkSheet = (Excel.Worksheet)_xlExcelApp.Worksheets.get_Item(3);
                    _xlWorkSheet.Name = "Basic Model|" + CocomoBasic_ComboBox_ProjectTypes.SelectedItem.ToString();
                }
            }
        }

        private void CocomoBasic_Button_GetQuote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CocomoBasic_TextBox_AmountProgramCodeValue.Text) ||
                string.IsNullOrEmpty(CocomoBasic_TextBox_AmountProgramCodeValue.Text)) {
                MessageBox.Show
                    (
                    "Введена пустая строка или пробел!",
                     "Предупреждение",
                     MessageBoxButton.OKCancel,
                     MessageBoxImage.Warning
                     );

                if (CocomoBasic_Button_GetQuote.IsEnabled == true) {
                    CocomoBasic_Button_GetQuote.IsEnabled = false;
                } 
            } else if (CocomoBasic_ComboBox_ProjectTypes.SelectedItem == null) {
                MessageBox.Show
                    (
                    "Не выбран тип проекта!",
                    "Предупреждение",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoBasic_Button_GetQuote.IsEnabled == true) {
                    CocomoBasic_Button_GetQuote.IsEnabled = false;
                } 
            } else {
                ProjectTypes.ProjectType projectType = (ProjectTypes.ProjectType)Enum.Parse(typeof(ProjectTypes.ProjectType), CocomoBasic_ComboBox_ProjectTypes.Text);
                float amountOfProgramCode = float.Parse(CocomoBasic_TextBox_AmountProgramCodeValue.Text.Trim());

                CocomoBasic_TextBox_LaboriousnessValue.Text = CocomoBasicModel.GetEfforts(amountOfProgramCode, projectType).ToString("F2");
                CocomoBasic_TextBox_TimeToDevelopeValue.Text = CocomoBasicModel.GetTimeToDevelop(amountOfProgramCode, projectType).ToString("F2");

                MessageBox.Show
                    (
                    "УСПЕШНО!",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );

                if (_writeToExcel == true) {
                    MessageBoxResult messageBox_putToExcelFile = MessageBox.Show
                    (
                    "Сохранить данные в Excel-файл?",
                    "Подтверждения действия",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                    );

                    switch (messageBox_putToExcelFile)
                    {
                        case MessageBoxResult.Yes:
                            _xlRange = _xlWorkSheet.Cells.Find(CocomoBasic_TextBox_AmountProgramCodeValue.Text,
                                Type.Missing, XlFindLookIn.xlValues, XlLookAt.xlPart);

                            if ( _xlRange != null ) {
                                MessageBox.Show
                                    (
                                    "Такие данные уже были сохранены ранее!",
                                    "Предупреждение",
                                    MessageBoxButton.OKCancel,
                                    MessageBoxImage.Warning
                                    );
                            } else {
                                int lastRow = _xlWorkSheet.Range["A" + _xlWorkSheet.Rows.Count].End[XlDirection.xlUp].Row + 1;
                                _xlWorkSheet.Cells[lastRow, 1] = amountOfProgramCode;
                                _xlWorkSheet.Cells[lastRow, 2] = CocomoBasic_TextBox_LaboriousnessValue.Text;
                                _xlWorkSheet.Cells[lastRow, 3] = CocomoBasic_TextBox_TimeToDevelopeValue.Text;

                                dynamic allDataRange = _xlWorkSheet.UsedRange;
                                allDataRange.Sort(allDataRange.Columns[1], XlSortOrder.xlAscending);

                                _xlWorkBook.Save();

                                MessageBox.Show
                                    (
                                    "Данные успешно сохранены в Excel-файле!",
                                    "Информация",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information
                                    );

                                MessageBoxResult messageBox_openExcelFile = MessageBox.Show
                                    (
                                    "Открыть Excel-файл?",
                                    "Подтверждение действия",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question
                                    );
                                
                                switch (messageBox_openExcelFile)
                                {
                                    case MessageBoxResult.Yes:
                                        _xlExcelApp.Visible = true;
                                        break;
                                    case MessageBoxResult.No:
                                        e.Handled = true;
                                        break;
                                }
                            }
                            break;
                        case MessageBoxResult.No:
                            e.Handled = true;
                            break;
                    }
                } else { 
                    e.Handled = true;
                }  
            }
        }

        private void CocomoBasic_Button_ClearValues_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoBasic_TextBox_LaboriousnessValue.Text) &&
                string.IsNullOrEmpty(CocomoBasic_TextBox_TimeToDevelopeValue.Text)) {
                MessageBox.Show
                    (
                    "Невозможно очистить пустое поле!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                CocomoBasic_Button_ClearValues.IsEnabled = false;
            } else {
                CocomoBasic_TextBox_LaboriousnessValue.Clear();
                CocomoBasic_TextBox_TimeToDevelopeValue.Clear();
            }
        }
        #endregion
        
        #region COCOMO Intermediate
        private void CocomoIntermediate_ComboBox_ProjectTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_ProjectTypes.SelectedIndex == 0) {
                CocomoIntermediate_GroupBox_TextBlock_ProjectsDescription.Text = "Распространенный тип характеризуется тем, что проект выполняется небольшой группой специалистов, " +
                    "имеющих опыт в создании подобных изделий и опыт применения технологических средств. Условия работы стабильны, и изделие имеет относительно " +
                    "невысокую сложность.";

            } else if (CocomoIntermediate_ComboBox_ProjectTypes.SelectedIndex == 1) {
                CocomoIntermediate_GroupBox_TextBlock_ProjectsDescription.Text = "Встроенный тип характеризуется очень жесткими требованиями на программный продукт, " +
                   "интерфейсы, параметры ЭВМ. Как правило, у таких изделий высокая степень новизны и планирование работ осуществляется при " +
                   "недостаточной информации, как о самом изделии, так и об условиях работы. Встроенный проект требует больших затрат на " +
                   "изменения и исправления.";

            } else if (CocomoIntermediate_ComboBox_ProjectTypes.SelectedIndex == 2) {
                CocomoIntermediate_GroupBox_TextBlock_ProjectsDescription.Text = "Полунезависимый тип занимает промежуточное положение между распространенным и встроенным – это проекты средней " +
                    "сложности. Исполнители знакомы лишь c некоторыми характеристиками (или компонентами) создаваемой системы, имеют средний опыт работы с подобными изделиями, " +
                    "изделие имеет элемент новизны. Только часть требований к изделию жестко фиксируется, в остальном разработки имеют степени выбора.";
            }
        }

        private void CocomoIntermediate_RadioButton_ProductFeatures_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIntermediate_TabControl.SelectedIndex = 1;

            CocomoIntermediate_Border.Height = 170;

            CocomoIntermediate_GroupBox_Results.Margin = new Thickness(5, 120, 0, 0);
        }

        private void CocomoIntermediate_RadioButton_HardwareSpecifications_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIntermediate_TabControl.SelectedIndex = 2;

            CocomoIntermediate_Border.Height = 235;

            CocomoIntermediate_GroupBox_Results.Margin = new Thickness(5, 55, 0, 0);
        }

        private void CocomoIntermediate_RadioButton_PersonnelCharacteristics_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIntermediate_TabControl.SelectedIndex = 3;

            CocomoIntermediate_Border.Height = 290;

            CocomoIntermediate_GroupBox_Results.Margin = new Thickness(5, 0, 0, 0);
        }

        private void CocomoIntermediate_RadioButton_ProjectCharacteristics_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIntermediate_TabControl.SelectedIndex = 4;

            CocomoIntermediate_Border.Height = 170;

            CocomoIntermediate_GroupBox_Results.Margin = new Thickness(5, 120, 0, 0);
        }

        private void CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedIndex == 0) {
                reguiredSoftwareReliabilityValue = costDriversTable[0][0];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedIndex == 1) {
                reguiredSoftwareReliabilityValue = costDriversTable[0][1];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedIndex == 2) {
                reguiredSoftwareReliabilityValue = costDriversTable[0][2];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedIndex == 3) {
                reguiredSoftwareReliabilityValue = costDriversTable[0][3];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedIndex == 4) {
                reguiredSoftwareReliabilityValue = costDriversTable[0][4];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedIndex == 0) {
                sizeOfApplicationDatabaseValue = costDriversTable[1][0];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedIndex == 1) {
                sizeOfApplicationDatabaseValue = costDriversTable[1][1];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedIndex == 2) {
                sizeOfApplicationDatabaseValue = costDriversTable[1][2];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedIndex == 3) {
                sizeOfApplicationDatabaseValue = costDriversTable[1][3];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForProductComplexity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 0) {
                complexityOfProductValue = costDriversTable[2][0];
            } else if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 1) {
                complexityOfProductValue = costDriversTable[2][1];
            } else if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 2) {
                complexityOfProductValue = costDriversTable[2][2];
            } else if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 3) {
                complexityOfProductValue = costDriversTable[2][3];
            } else if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 4) {
                complexityOfProductValue = costDriversTable[2][4];
            } else if (CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedIndex == 5) {
                complexityOfProductValue = costDriversTable[2][5];
            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedIndex == 0) {
                runTimePerformanceConstraintsValue = costDriversTable[3][0]; 
            } else if (CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedIndex == 1) {
                runTimePerformanceConstraintsValue = costDriversTable[3][1]; 
            } else if (CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedIndex == 2) {
                runTimePerformanceConstraintsValue = costDriversTable[3][2]; 
            }  else if (CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedIndex == 3) {
                runTimePerformanceConstraintsValue = costDriversTable[3][3]; 
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForMemoryConstraints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedIndex == 0) {
                memoryConstraintsValue = costDriversTable[4][0]; 
            } else if (CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedIndex == 1) {
                memoryConstraintsValue = costDriversTable[4][1];
            } else if (CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedIndex == 2) {
                memoryConstraintsValue = costDriversTable[4][2];
            } else if (CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedIndex == 3) {
                memoryConstraintsValue = costDriversTable[4][3];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedIndex == 0) {
                volatalityOfVirtualMachineEnvironmentValue = costDriversTable[5][0];
            } else if (CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedIndex == 1) {
                volatalityOfVirtualMachineEnvironmentValue = costDriversTable[5][1];
            } else if (CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedIndex == 2) {
                volatalityOfVirtualMachineEnvironmentValue = costDriversTable[5][2];
            } else if (CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedIndex == 3) {
                volatalityOfVirtualMachineEnvironmentValue = costDriversTable[5][3];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedIndex == 0) {
                reguiredTurnaboutTimeValue = costDriversTable[6][0];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedIndex == 1) {
                reguiredTurnaboutTimeValue = costDriversTable[6][1];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedIndex == 2) {
                reguiredTurnaboutTimeValue = costDriversTable[6][2];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedIndex == 3) {
                reguiredTurnaboutTimeValue = costDriversTable[6][3];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForAnalyticSkills_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedIndex == 0) {
                analystCapabilityValue = costDriversTable[7][0];
            } else if (CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedIndex == 1) {
                analystCapabilityValue = costDriversTable[7][1];
            } else if (CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedIndex == 2) {
                analystCapabilityValue = costDriversTable[7][2];
            } else if (CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedIndex == 3) {
                analystCapabilityValue = costDriversTable[7][3];
            } else if (CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedIndex == 4) {
                analystCapabilityValue = costDriversTable[7][4];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForApplicationsExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedIndex == 0) {
                applicationsExperienceValue = costDriversTable[8][0];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedIndex == 1) {
                applicationsExperienceValue = costDriversTable[8][1];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedIndex == 2) {
                applicationsExperienceValue = costDriversTable[8][2];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedIndex == 3) {
                applicationsExperienceValue = costDriversTable[8][3];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedIndex == 4) {
                applicationsExperienceValue = costDriversTable[8][4];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedIndex == 0) {
                softwareEngineerCapabilityValue = costDriversTable[9][0];
            } else if (CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedIndex == 1) {
                softwareEngineerCapabilityValue = costDriversTable[9][1];
            } else if (CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedIndex == 2) {
                softwareEngineerCapabilityValue = costDriversTable[9][2];
            } else if (CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedIndex == 3) {
                softwareEngineerCapabilityValue = costDriversTable[9][3];
            } else if (CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedIndex == 4) {
                softwareEngineerCapabilityValue = costDriversTable[9][4];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedIndex == 0) {
                virtualMachineExperienceValue = costDriversTable[10][0];
            } else if (CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedIndex == 1) {
                virtualMachineExperienceValue = costDriversTable[10][1];
            } else if (CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedIndex == 2) {
                virtualMachineExperienceValue = costDriversTable[10][2];
            } else if (CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedIndex == 3) {
                virtualMachineExperienceValue = costDriversTable[10][3];
            } else {

            }
        }
        
        private void CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedIndex == 0) {
                programmingLanguageExperienceValue = costDriversTable[11][0];
            } else if (CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedIndex == 1) {
                programmingLanguageExperienceValue = costDriversTable[11][1];
            } else if (CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedIndex == 2) {
                programmingLanguageExperienceValue = costDriversTable[11][2];
            } else if (CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedIndex == 3) {
                programmingLanguageExperienceValue = costDriversTable[11][3];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedIndex == 0) {
               applicationOfSoftwareEngineeringMethodsValue = costDriversTable[12][0];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedIndex == 1) {
                applicationOfSoftwareEngineeringMethodsValue = costDriversTable[12][1];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedIndex == 2) {
                applicationOfSoftwareEngineeringMethodsValue = costDriversTable[12][2];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedIndex == 3) {
                applicationOfSoftwareEngineeringMethodsValue = costDriversTable[12][3];
            } else if (CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedIndex == 4) {
                applicationOfSoftwareEngineeringMethodsValue = costDriversTable[12][4];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForUseSoftwareTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedIndex == 0) {
                useOfSoftwareToolsValue = costDriversTable[13][0];
            } else if (CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedIndex == 1) {
                useOfSoftwareToolsValue = costDriversTable[13][1];
            } else if (CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedIndex == 2) {
                useOfSoftwareToolsValue = costDriversTable[13][2];
            } else if (CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedIndex == 3) {
                useOfSoftwareToolsValue = costDriversTable[13][3];
            } else if (CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedIndex == 4) {
                useOfSoftwareToolsValue = costDriversTable[13][4];
            } else {

            }
        }

        private void CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedIndex == 0) {
                reguiredDevelopmentScheduleValue = costDriversTable[14][0];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedIndex == 1) {
                reguiredDevelopmentScheduleValue = costDriversTable[14][1];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedIndex == 2) {
                reguiredDevelopmentScheduleValue = costDriversTable[14][2];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedIndex == 3) {
                reguiredDevelopmentScheduleValue = costDriversTable[14][3];
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedIndex == 4) {
                reguiredDevelopmentScheduleValue = costDriversTable[14][4];
            } else {

            }
        }

        private void CocomoIntermediate_Button_GetQuote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CocomoIntermediate_TextBox_AmountProgramCodeValue.Text) ||
                string.IsNullOrEmpty(CocomoIntermediate_TextBox_AmountProgramCodeValue.Text)) {
                MessageBox.Show(
                    "Введена пустая строка или пробел!",
                     "Ошибка ввода параметров",
                     MessageBoxButton.OKCancel,
                     MessageBoxImage.Warning
                     );

                if (CocomoIntermediate_Button_GetQuote.IsEnabled == true) {
                    CocomoIntermediate_Button_GetQuote.IsEnabled = false;
                }
            } else if (CocomoIntermediate_ComboBox_ProjectTypes.SelectedItem == null) {
                MessageBox.Show(
                    "Не выбран тип проекта!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIntermediate_Button_GetQuote.IsEnabled == true) {
                    CocomoIntermediate_Button_GetQuote.IsEnabled = false;
                }
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedItem == null) {
                MessageBox.Show(
                    "Не для всех атрибутов стоимости выбран рейтинг!", 
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIntermediate_Button_GetQuote.IsEnabled == true) {
                    CocomoIntermediate_Button_GetQuote.IsEnabled = false;
                }
            } else {
                ProjectTypes.ProjectType projectType = (ProjectTypes.ProjectType)Enum.Parse(typeof(ProjectTypes.ProjectType), CocomoIntermediate_ComboBox_ProjectTypes.Text);
                float amountProgramCode = float.Parse(CocomoIntermediate_TextBox_AmountProgramCodeValue.Text.Trim());
                cocomoIntermediateEAF = reguiredSoftwareReliabilityValue * sizeOfApplicationDatabaseValue *
                    complexityOfProductValue * runTimePerformanceConstraintsValue * memoryConstraintsValue *
                    volatalityOfVirtualMachineEnvironmentValue * reguiredTurnaboutTimeValue * analystCapabilityValue *
                    softwareEngineerCapabilityValue * applicationsExperienceValue * virtualMachineExperienceValue * 
                    programmingLanguageExperienceValue * useOfSoftwareToolsValue * applicationOfSoftwareEngineeringMethodsValue * 
                    reguiredDevelopmentScheduleValue;

                CocomoIntermediate_TextBox_LaboriousnessValue.Text = CocomoIntermediateModel.GetEfforts(cocomoIntermediateEAF, amountProgramCode, projectType).ToString("F1");
                CocomoIntermediate_TextBox_TimeToDevelopeValue.Text = CocomoIntermediateModel.GetTimeToDevelop(cocomoIntermediateEAF, amountProgramCode, projectType).ToString("F1");
                CocomoIntermediate_TextBox_EAFValue.Text = cocomoIntermediateEAF.ToString("F2");

                MessageBox.Show(
                    "УСПЕШНО!",
                    "Отчет о выполнении операции",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
        }

        private void CocomoIntermediate_Button_ClearValues_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoIntermediate_TextBox_LaboriousnessValue.Text) &&
                string.IsNullOrEmpty(CocomoIntermediate_TextBox_TimeToDevelopeValue.Text) &&
                string.IsNullOrEmpty(CocomoIntermediate_TextBox_EAFValue.Text)) {
                MessageBox.Show(
                    "Невозможно очистить пустое поле!",
                    "Ошибка выполнения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                if (CocomoIntermediate_Button_ClearValues.IsEnabled == true) {
                    CocomoIntermediate_Button_ClearValues.IsEnabled = false;
                }
            } else {
                CocomoIntermediate_TextBox_LaboriousnessValue.Clear();
                CocomoIntermediate_TextBox_TimeToDevelopeValue.Clear();
                CocomoIntermediate_TextBox_EAFValue.Clear();
            }
        }
        #endregion

        #region COCOMO II Early Design
        private void CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 0) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 1) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][1];
            }  else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 2) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 3) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 4) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 5) {
                PRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 0) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 1) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 2) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 3) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 4) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 5) {
                FLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 0) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 1) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 2) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 3) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 4) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 5) {
                RESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 0) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 1) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 2) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 3) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 4) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 5) {
                TEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 0) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 1) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 2) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 3) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 4) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 5) {
                PMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_Button_GetSumOfScaleFactors_Click(object sender, RoutedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedItem == null) {
                MessageBox.Show(
                    "Не для всех факторов масштаба выбран рейтинг!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIEarlyDesign_Button_GetSumOfScaleFactors.IsEnabled == true) {
                    CocomoIIEarlyDesign_Button_GetSumOfScaleFactors.IsEnabled = false;
                }
            } else {
                sumOfScaleFactors = PRECValue + FLEXValue + RESLValue + TEAMValue + PMATValue;
                CocomoIIEarlyDesign_TextBox_SumOfScaleFactorsValue.Text = sumOfScaleFactors.ToString();
            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 0) {
                PERSValue = effortMultipliersValuesTable[0][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 1) {
                PERSValue = effortMultipliersValuesTable[0][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 2) {
                PERSValue = effortMultipliersValuesTable[0][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 3) {
                PERSValue = effortMultipliersValuesTable[0][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 4) {
                PERSValue = effortMultipliersValuesTable[0][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 5) {
                PERSValue = effortMultipliersValuesTable[0][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 6) {
                PERSValue = effortMultipliersValuesTable[0][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 0) {
                PREXValue = effortMultipliersValuesTable[1][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 1) {
                PREXValue = effortMultipliersValuesTable[1][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 2) {
                PREXValue = effortMultipliersValuesTable[1][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 3) {
                PREXValue = effortMultipliersValuesTable[1][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 4) {
                PREXValue = effortMultipliersValuesTable[1][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 5) {
                PREXValue = effortMultipliersValuesTable[1][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 6) {
                PREXValue = effortMultipliersValuesTable[1][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 0) {
                RCPXValue = effortMultipliersValuesTable[2][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 1) {
                RCPXValue = effortMultipliersValuesTable[2][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 2) {
                RCPXValue = effortMultipliersValuesTable[2][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 3) {
                RCPXValue = effortMultipliersValuesTable[2][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 4) {
                RCPXValue = effortMultipliersValuesTable[2][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 5) {
                RCPXValue = effortMultipliersValuesTable[2][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 6) {
                RCPXValue = effortMultipliersValuesTable[2][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 0) {
                RUSEValue = effortMultipliersValuesTable[3][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 1) {
                RUSEValue = effortMultipliersValuesTable[3][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 2) {
                RUSEValue = effortMultipliersValuesTable[3][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 3) {
                RUSEValue = effortMultipliersValuesTable[3][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 4) {
                RUSEValue = effortMultipliersValuesTable[3][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 5) {
                RUSEValue = effortMultipliersValuesTable[3][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 6) {
                RUSEValue = effortMultipliersValuesTable[3][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 0) {
                PDIFValue = effortMultipliersValuesTable[4][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 1) {
                PDIFValue = effortMultipliersValuesTable[4][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 2) {
                PDIFValue = effortMultipliersValuesTable[4][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 3) {
                PDIFValue = effortMultipliersValuesTable[4][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 4) {
                PDIFValue = effortMultipliersValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 5) {
                PDIFValue = effortMultipliersValuesTable[4][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 6) {
                PDIFValue = effortMultipliersValuesTable[4][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 0) {
                FCILValue = effortMultipliersValuesTable[5][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 1) {
                FCILValue = effortMultipliersValuesTable[5][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 2) {
                FCILValue = effortMultipliersValuesTable[5][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 3) {
                FCILValue = effortMultipliersValuesTable[5][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 4) {
                FCILValue = effortMultipliersValuesTable[5][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 5) {
                FCILValue = effortMultipliersValuesTable[5][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 6) {
                FCILValue = effortMultipliersValuesTable[5][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 0) {
                SCEDValue = effortMultipliersValuesTable[6][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 1) {
                SCEDValue = effortMultipliersValuesTable[6][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 2) {
                SCEDValue = effortMultipliersValuesTable[6][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 3) {
                SCEDValue = effortMultipliersValuesTable[6][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 4) {
                SCEDValue = effortMultipliersValuesTable[6][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 5) {
                SCEDValue = effortMultipliersValuesTable[6][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 6) {
                SCEDValue = effortMultipliersValuesTable[6][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_Button_GetQuote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_SumOfScaleFactorsValue.Text)) {
                MessageBox.Show(
                    "Нет значения для параметра\n\"Сумма факторов масштаба\"!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIEarlyDesign_Button_GetQuote.IsEnabled == true) {
                    CocomoIIEarlyDesign_Button_GetQuote.IsEnabled = false;
                }
            } else if (string.IsNullOrWhiteSpace(CocomoIIEarlyDesign_TextBox_AmountProgramCodeValue.Text) ||
                string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_AmountProgramCodeValue.Text)) {
                MessageBox.Show(
                    "Введена пустая строка или пробел!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIEarlyDesign_Button_GetQuote.IsEnabled == true) {
                    CocomoIIEarlyDesign_Button_GetQuote.IsEnabled = false;
                }
            } else if (CocomoIntermediate_ComboBox_LevelForRequiredSoftwareReliability.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationDatabaseSize.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForProductComplexity.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRunTimePerformanceConstraints.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForMemoryConstraints.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForVolatilityOfTheVirtualMachineEnvironment.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRequiredTurnaboutTime.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForAnalyticSkills.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationsExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForSoftwareEngineerCapability.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForVirtualMachineExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForProgrammingLanguageExperience.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForApplicationSoftwareEngineeringMethods.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForUseSoftwareTools.SelectedItem == null ||
                CocomoIntermediate_ComboBox_LevelForRequiredDevelopmentSchedule.SelectedItem == null)
            {
                MessageBox.Show(
                    "Не для всех множителей трудоёмкости\nвыбран рейтинг!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIEarlyDesign_Button_GetQuote.IsEnabled == true) {
                    CocomoIIEarlyDesign_Button_GetQuote.IsEnabled = false;
                }
            }
            else {
                float amountProgramCode = float.Parse(CocomoIIEarlyDesign_TextBox_AmountProgramCodeValue.Text.Trim());
                cocomoIIEarlyDesignEAF = PERSValue * PREXValue * RCPXValue * RUSEValue * PDIFValue *
                    FCILValue * SCEDValue;
                cocomoIIEarlyDesignEAFWithoutSCED = PERSValue * PREXValue * RCPXValue * RUSEValue *
                    PDIFValue * FCILValue;

                CocomoIIEarlyDesign_TextBox_LaboriousnessValue.Text = CocomoIIEarlyDesignModel.GetEfforts(cocomoIIEarlyDesignEAF, sumOfScaleFactors, amountProgramCode).ToString("F2");
                CocomoIIEarlyDesign_TextBox_TimeToDevelopeValue.Text = CocomoIIEarlyDesignModel.GetTimeToDevelop(SCEDValue, cocomoIIEarlyDesignEAFWithoutSCED, amountProgramCode, sumOfScaleFactors).ToString("F2");
                CocomoIIEarlyDesign_TextBox_EAFValue.Text = cocomoIIEarlyDesignEAF.ToString("F2");
                CocomoIIEarlyDesign_TextBox_EAFWithoutSCEDValue.Text = cocomoIIEarlyDesignEAFWithoutSCED.ToString("F2");
            }
        }

        #endregion

        #region COCOMO II Post Architecture
        private void CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 0) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 1) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 2) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 3) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 4) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 5) {
                PRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 0) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 1) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 2) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 3) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 4) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 5) {
                FLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 0) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 1) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 2) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 3) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 4) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 5) {
                RESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 0) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 1) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 2) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 3) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 4) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 5) {
                TEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 0) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 1) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 2) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 3) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 4) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 5) {
                PMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_Button_GetSumOfScaleFactors_Click(object sender, RoutedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedItem == null) {
                MessageBox.Show(
                    "Не для всех факторов масштаба выбран рейтинг!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIPostArchitecture_Button_GetSumOfScaleFactors.IsEnabled == true) {
                    CocomoIIPostArchitecture_Button_GetSumOfScaleFactors.IsEnabled = false;
                }
            }
            else
            {
                sumOfScaleFactors = PRECValue + FLEXValue + RESLValue + TEAMValue + PMATValue;
                CocomoIIPostArchitecture_TextBox_SumOfScaleFactorsValue.Text = sumOfScaleFactors.ToString();
            }
        }
        #endregion
    }
}