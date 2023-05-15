using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Microsoft.Win32;
using Microsoft.Office.Interop.Excel;

using static COCOMO_Калькулятор.ProjectTypes;
using static COCOMO_Калькулятор.CocomoIntermediateModel;
using static COCOMO_Калькулятор.CocomoIIEarlyDesignModel;
using static COCOMO_Калькулятор.CocomoIIPostArchitectureModel;

using Excel = Microsoft.Office.Interop.Excel;
using Application = System.Windows.Application;
using Window = System.Windows.Window;
using TextBox = System.Windows.Controls.TextBox;

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
        public float cocomoIntermediateEAFValue = 1f;

        public float cocomoIIEarlyDesignPERSValue = 1f;
        public float cocomoIIEarlyDesignPREXValue = 1f;
        public float cocomoIIEarlyDesignRCPXValue = 1f;
        public float cocomoIIEarlyDesignRUSEValue = 1f;
        public float cocomoIIEarlyDesignPDIFValue = 1f;
        public float cocomoIIEarlyDesignFCILValue = 1f;
        public float cocomoIIEarlyDesignSCEDValue = 1f;
        public float cocomoIIEarlyDesignEAFValue = 1f;
        public float cocomoIIEarlyDesignEAFWithoutSCEDValue = 1f;

        public float cocomoIIEarlyDesignPRECValue = 0f;
        public float cocomoIIEarlyDesignFLEXValue = 0f;
        public float cocomoIIEarlyDesignRESLValue = 0f;
        public float cocomoIIEarlyDesignTEAMValue = 0f;
        public float cocomoIIEarlyDesignPMATValue = 0f;
        public float cocomoIIEarlyDesignSumOfScaleFactors = 0f;

        public float cocomoIIPostArchitecturePRECValue = 0f;
        public float cocomoIIPostArchitectureFLEXValue = 0f;
        public float cocomoIIPostArchitectureRESLValue = 0f;
        public float cocomoIIPostArchitectureTEAMValue = 0f;
        public float cocomoIIPostArchitecturePMATValue = 0f;
        public float cocomoIIPostArchitectureSumOfScaleFactors = 0f;

        public float cocomoIIPostArchitectureACAPValue = 1f;
        public float cocomoIIPostArchitectureAEXPValue = 1f;
        public float cocomoIIPostArchitecturePCAPValue = 1f;
        public float cocomoIIPostArchitecturePCONValue = 1f;
        public float cocomoIIPostArchitecturePEXPValue = 1f;
        public float cocomoIIPostArchitectureLTEXValue = 1f;
        public float cocomoIIPostArchitectureRELYValue = 1f;
        public float cocomoIIPostArchitectureDATAValue = 1f;
        public float cocomoIIPostArchitectureCPLXValue = 1f;
        public float cocomoIIPostArchitectureRUSEValue = 1f;
        public float cocomoIIPostArchitectureDOCUValue = 1f;
        public float cocomoIIPostArchitectureTIMEValue = 1f;
        public float cocomoIIPostArchitectureSTORValue = 1f;
        public float cocomoIIPostArchitecturePVOLValue = 1f;
        public float cocomoIIPostArchitectureTOOLValue = 1f;
        public float cocomoIIPostArchitectureSITEValue = 1f;
        public float cocomoIIPostArchitectureSCEDValue = 1f;
        public float cocomoIIPostArchitectureEAFValue = 1f;
        public float cocomoIIPostArchitectureEAFWithoutSCEDValue = 1f;

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

        private void All_TextBoxes_AmountProgramCodeValue_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            Regex onlyDigitsNCommaRegex = new Regex("[^0-9,-]+");
            Regex onlyDigitsRegex = new Regex("[^0-9-]+");

            if (textBox.Text.Contains(",")) {
                e.Handled = onlyDigitsRegex.IsMatch(e.Text);
            } else if (textBox.Text.Equals(",")) {
                //
            } else {
                e.Handled = onlyDigitsNCommaRegex.IsMatch(e.Text);
            }
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
                cocomoIntermediateEAFValue = reguiredSoftwareReliabilityValue * sizeOfApplicationDatabaseValue *
                    complexityOfProductValue * runTimePerformanceConstraintsValue * memoryConstraintsValue *
                    volatalityOfVirtualMachineEnvironmentValue * reguiredTurnaboutTimeValue * analystCapabilityValue *
                    softwareEngineerCapabilityValue * applicationsExperienceValue * virtualMachineExperienceValue * 
                    programmingLanguageExperienceValue * useOfSoftwareToolsValue * applicationOfSoftwareEngineeringMethodsValue * 
                    reguiredDevelopmentScheduleValue;

                CocomoIntermediate_TextBox_LaboriousnessValue.Text = CocomoIntermediateModel.GetEfforts(cocomoIntermediateEAFValue, amountProgramCode, projectType).ToString("F1");
                CocomoIntermediate_TextBox_TimeToDevelopeValue.Text = CocomoIntermediateModel.GetTimeToDevelop(cocomoIntermediateEAFValue, amountProgramCode, projectType).ToString("F1");
                CocomoIntermediate_TextBox_EAFValue.Text = cocomoIntermediateEAFValue.ToString("F2");

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
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 1) {
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][1];
            }  else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 2) {
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 3) {
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 4) {
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 5) {
                cocomoIIEarlyDesignPRECValue = cocomoIIEarlyDesignScaleFactorsValuesTable[0][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 0) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 1) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 2) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 3) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 4) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 5) {
                cocomoIIEarlyDesignFLEXValue = cocomoIIEarlyDesignScaleFactorsValuesTable[1][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 0) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 1) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 2) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 3) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 4) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 5) {
                cocomoIIEarlyDesignRESLValue = cocomoIIEarlyDesignScaleFactorsValuesTable[2][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 0) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 1) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 2) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 3) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 4) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 5) {
                cocomoIIEarlyDesignTEAMValue = cocomoIIEarlyDesignScaleFactorsValuesTable[3][5];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 0) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 1) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 2) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 3) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 4) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 5) {
                cocomoIIEarlyDesignPMATValue = cocomoIIEarlyDesignScaleFactorsValuesTable[4][5];
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
                cocomoIIEarlyDesignSumOfScaleFactors = cocomoIIEarlyDesignPRECValue + cocomoIIEarlyDesignFLEXValue +
                    cocomoIIEarlyDesignRESLValue + cocomoIIEarlyDesignTEAMValue + cocomoIIEarlyDesignPMATValue;
                CocomoIIEarlyDesign_TextBox_SumOfScaleFactorsValue.Text = cocomoIIEarlyDesignSumOfScaleFactors.ToString();
            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignPERSValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[0][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignPREXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[1][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignRCPXValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[2][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignRUSEValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[3][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignPDIFValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[4][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignFCILValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[5][6];
            } else {

            }
        }

        private void CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 0) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][0];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 1) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][1];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 2) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][2];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 3) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][3];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 4) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 5) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][5];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 6) {
                cocomoIIEarlyDesignSCEDValue = cocomoIIEarlyDesignEffortMultipliersValuesTable[6][6];
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
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPERSEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForPREXEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForRCPXEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForRUSEEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForPDIFEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForFCILEffortMultiplier.SelectedItem == null ||
                CocomoIIEarlyDesign_ComboBox_LevelForSCEDEffortMultiplier.SelectedItem == null) {
                MessageBox.Show(
                    "Не для всех множителей трудоёмкости\nвыбран рейтинг!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIEarlyDesign_Button_GetQuote.IsEnabled == true) {
                    CocomoIIEarlyDesign_Button_GetQuote.IsEnabled = false;
                }
            } else {
                float amountProgramCode = float.Parse(CocomoIIEarlyDesign_TextBox_AmountProgramCodeValue.Text.Trim());
                cocomoIIEarlyDesignEAFValue = cocomoIIEarlyDesignPERSValue * 
                    cocomoIIEarlyDesignPREXValue * cocomoIIEarlyDesignRCPXValue * 
                    cocomoIIEarlyDesignRUSEValue * cocomoIIEarlyDesignPDIFValue *
                    cocomoIIEarlyDesignFCILValue * cocomoIIEarlyDesignSCEDValue;

                cocomoIIEarlyDesignEAFWithoutSCEDValue = cocomoIIEarlyDesignPERSValue * 
                    cocomoIIEarlyDesignPREXValue * cocomoIIEarlyDesignRCPXValue * 
                    cocomoIIEarlyDesignRUSEValue * cocomoIIEarlyDesignPDIFValue * 
                    cocomoIIEarlyDesignFCILValue;

                CocomoIIEarlyDesign_TextBox_LaboriousnessValue.Text = CocomoIIEarlyDesignModel.GetEfforts(cocomoIIEarlyDesignEAFValue, cocomoIIEarlyDesignSumOfScaleFactors, amountProgramCode).ToString("F2");
                CocomoIIEarlyDesign_TextBox_TimeToDevelopeValue.Text = CocomoIIEarlyDesignModel.GetTimeToDevelop(cocomoIIEarlyDesignSCEDValue, cocomoIIEarlyDesignEAFWithoutSCEDValue, amountProgramCode, cocomoIIEarlyDesignSumOfScaleFactors).ToString("F2");
                CocomoIIEarlyDesign_TextBox_EAFValue.Text = cocomoIIEarlyDesignEAFValue.ToString("F2");
                CocomoIIEarlyDesign_TextBox_EAFWithoutSCEDValue.Text = cocomoIIEarlyDesignEAFWithoutSCEDValue.ToString("F2");
            }
        }

        private void CocomoIIEarlyDesign_Button_ClearValues_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_LaboriousnessValue.Text) &&
                string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_TimeToDevelopeValue.Text) &&
                string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_EAFValue.Text) &&
                string.IsNullOrEmpty(CocomoIIEarlyDesign_TextBox_EAFWithoutSCEDValue.Text)) {
                MessageBox.Show
                    (
                    "Невозможно очистить пустое поле!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                CocomoIIEarlyDesign_Button_ClearValues.IsEnabled = false;
            } else {
                CocomoIIEarlyDesign_TextBox_LaboriousnessValue.Clear(); 
                CocomoIIEarlyDesign_TextBox_TimeToDevelopeValue.Clear();
                CocomoIIEarlyDesign_TextBox_EAFValue.Clear();
                CocomoIIEarlyDesign_TextBox_EAFWithoutSCEDValue.Clear();
            }
        }

        #endregion

        #region COCOMO II Post Architecture
        private void CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 0) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 1) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 2) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 3) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 4) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPRECScaleFactor.SelectedIndex == 5) {
                cocomoIIPostArchitecturePRECValue = cocomoIIPostArchitectureScaleFactorsValuesTable[0][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 0) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 1) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 2) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 3) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 4) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForFLEXScaleFactor.SelectedIndex == 5) {
                cocomoIIPostArchitectureFLEXValue = cocomoIIPostArchitectureScaleFactorsValuesTable[1][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 0) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 1) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 2) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 3) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 4) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRESLScaleFactor.SelectedIndex == 5) {
                cocomoIIPostArchitectureRESLValue = cocomoIIPostArchitectureScaleFactorsValuesTable[2][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 0) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 1) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 2) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 3) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 4) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTEAMScaleFactor.SelectedIndex == 5) {
                cocomoIIPostArchitectureTEAMValue = cocomoIIPostArchitectureScaleFactorsValuesTable[3][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 0) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 1) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 2) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 3) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 4) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][4];
            } else if (CocomoIIEarlyDesign_ComboBox_LevelForPMATScaleFactor.SelectedIndex == 5) {
                cocomoIIPostArchitecturePMATValue = cocomoIIPostArchitectureScaleFactorsValuesTable[4][5];
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
                cocomoIIPostArchitectureSumOfScaleFactors = cocomoIIPostArchitecturePRECValue + cocomoIIPostArchitectureFLEXValue +
                    cocomoIIPostArchitectureRESLValue + cocomoIIPostArchitectureTEAMValue + cocomoIIPostArchitecturePMATValue;
                CocomoIIPostArchitecture_TextBox_SumOfScaleFactorsValue.Text = cocomoIIPostArchitectureSumOfScaleFactors.ToString();
            }
        }

        private void CocomoIIPostArchitecture_RadioButton_PersonnelFactors_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIIPostArchitecture_TabControl.SelectedIndex = 1;
        }

        private void CocomoIIPostArchitecture_RadioButton_ProductFactors_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIIPostArchitecture_TabControl.SelectedIndex = 2;
        }

        private void CocomoIIPostArchitecture_RadioButton_PlateauFactors_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIIPostArchitecture_TabControl.SelectedIndex = 3;
        }

        private void CocomoIIPostArchitecture_RadioButton_ProjectFactors_Checked(object sender, RoutedEventArgs e)
        {
            CocomoIIPostArchitecture_TabControl.SelectedIndex = 4;
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureACAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[0][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureACAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[0][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureACAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[0][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureACAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[0][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureACAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[0][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureAEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[1][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureAEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[1][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureAEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[1][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureAEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[1][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureAEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[1][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitecturePCAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[2][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitecturePCAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[2][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitecturePCAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[2][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitecturePCAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[2][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitecturePCAPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[2][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitecturePCONValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[3][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitecturePCONValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[3][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitecturePCONValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[3][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitecturePCONValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[3][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitecturePCONValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[3][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitecturePEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[4][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitecturePEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[4][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitecturePEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[4][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitecturePEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[4][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitecturePEXPValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[4][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureLTEXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[5][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureLTEXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[5][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureLTEXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[5][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureLTEXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[5][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureLTEXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[5][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureRELYValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[6][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureRELYValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[6][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureRELYValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[6][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureRELYValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[6][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureRELYValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[6][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureDATAValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[7][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureDATAValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[7][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureDATAValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[7][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureDATAValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[7][3];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedIndex == 5) {
                cocomoIIPostArchitectureCPLXValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[8][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureRUSEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[9][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureRUSEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[9][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureRUSEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[9][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureRUSEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[9][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureRUSEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[9][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureDOCUValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[10][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureDOCUValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[10][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureDOCUValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[10][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureDOCUValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[10][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureDOCUValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[10][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureTIMEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[11][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureTIMEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[11][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureTIMEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[11][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureTIMEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[11][3];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureSTORValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[12][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureSTORValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[12][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureSTORValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[12][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureSTORValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[12][3];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitecturePVOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[13][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitecturePVOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[13][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitecturePVOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[13][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitecturePVOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[13][3];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureTOOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[14][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureTOOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[14][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureTOOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[14][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureTOOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[14][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureTOOLValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[14][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][4];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedIndex == 5) {
                cocomoIIPostArchitectureSITEValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[15][5];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 0) {
                cocomoIIPostArchitectureSCEDValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[16][0];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 1) {
                cocomoIIPostArchitectureSCEDValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[16][1];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 2) {
                cocomoIIPostArchitectureSCEDValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[16][2];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 3) {
                cocomoIIPostArchitectureSCEDValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[16][3];
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedIndex == 4) {
                cocomoIIPostArchitectureSCEDValue = cocomoIIPostArchitectureEffortMultipliersValuesTable[16][4];
            } else {

            }
        }

        private void CocomoIIPostArchitecture_Button_GetQuote_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_SumOfScaleFactorsValue.Text)) {
                MessageBox.Show(
                    "Нет значения для параметра\n\"Сумма факторов масштаба\"!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIPostArchitecture_Button_GetQuote.IsEnabled == true) {
                    CocomoIIPostArchitecture_Button_GetQuote.IsEnabled = false;
                }
            } else if (string.IsNullOrWhiteSpace(CocomoIIPostArchitecture_TextBox_AmountProgramCodeValue.Text) ||
                string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_AmountProgramCodeValue.Text)) {
                MessageBox.Show(
                    "Введена пустая строка или пробел!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIPostArchitecture_Button_GetQuote.IsEnabled == true) {
                    CocomoIIPostArchitecture_Button_GetQuote.IsEnabled = false;
                }
            } else if (CocomoIIPostArchitecture_ComboBox_LevelForACAPEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForAEXPEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForPCAPEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForPCONEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForPEXPEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForLTEXEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForRELYEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForDATAEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForCPLXEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForRUSEEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForDOCUEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForTIMEEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForSTOREffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForPVOLEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForTOOLEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForSITEEffortMultiplier.SelectedItem == null ||
                CocomoIIPostArchitecture_ComboBox_LevelForSCEDEffortMultiplier.SelectedItem == null) {
                MessageBox.Show(
                    "Не для всех множителей трудоёмкости\nвыбран рейтинг!",
                    "Ошибка ввода параметров",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning
                    );

                if (CocomoIIPostArchitecture_Button_GetQuote.IsEnabled == true) {
                    CocomoIIPostArchitecture_Button_GetQuote.IsEnabled = false;
                }
            } else {
                float amountProgramCode = float.Parse(CocomoIIPostArchitecture_TextBox_AmountProgramCodeValue.Text.Trim());

                cocomoIIPostArchitectureEAFValue = cocomoIIPostArchitectureACAPValue * 
                    cocomoIIPostArchitectureAEXPValue * cocomoIIPostArchitecturePCAPValue * 
                    cocomoIIPostArchitecturePCONValue * cocomoIIPostArchitecturePEXPValue * 
                    cocomoIIPostArchitectureLTEXValue * cocomoIIPostArchitectureRELYValue * 
                    cocomoIIPostArchitectureDATAValue * cocomoIIPostArchitectureCPLXValue * 
                    cocomoIIPostArchitectureRUSEValue * cocomoIIPostArchitectureDOCUValue * 
                    cocomoIIPostArchitectureTIMEValue * cocomoIIPostArchitectureSTORValue * 
                    cocomoIIPostArchitecturePVOLValue * cocomoIIPostArchitectureTOOLValue * 
                    cocomoIIPostArchitectureSITEValue * cocomoIIPostArchitectureSCEDValue;

                cocomoIIPostArchitectureEAFWithoutSCEDValue = cocomoIIPostArchitectureACAPValue *
                    cocomoIIPostArchitectureAEXPValue * cocomoIIPostArchitecturePCAPValue *
                    cocomoIIPostArchitecturePCONValue * cocomoIIPostArchitecturePEXPValue *
                    cocomoIIPostArchitectureLTEXValue * cocomoIIPostArchitectureRELYValue *
                    cocomoIIPostArchitectureDATAValue * cocomoIIPostArchitectureCPLXValue *
                    cocomoIIPostArchitectureRUSEValue * cocomoIIPostArchitectureDOCUValue *
                    cocomoIIPostArchitectureTIMEValue * cocomoIIPostArchitectureSTORValue *
                    cocomoIIPostArchitecturePVOLValue * cocomoIIPostArchitectureTOOLValue *
                    cocomoIIPostArchitectureSITEValue;

                CocomoIIPostArchitecture_TextBox_LaboriousnessValue.Text = CocomoIIPostArchitectureModel.GetEfforts(cocomoIIPostArchitectureEAFValue, cocomoIIPostArchitectureSumOfScaleFactors, amountProgramCode).ToString("F2");
                CocomoIIPostArchitecture_TextBox_TimeToDevelopeValue.Text = CocomoIIPostArchitectureModel.GetTimeToDevelop(cocomoIIPostArchitectureSCEDValue, cocomoIIPostArchitectureEAFWithoutSCEDValue, amountProgramCode, cocomoIIPostArchitectureSumOfScaleFactors).ToString("F2");
                CocomoIIPostArchitecture_TextBox_EAFValue.Text = cocomoIIPostArchitectureEAFValue.ToString("F2");
                CocomoIIPostArchitecture_TextBox_EAFWithoutSCEDValue.Text = cocomoIIPostArchitectureEAFWithoutSCEDValue.ToString("F2");
            }
        }

        private void CocomoIIPostArchitecture_Button_ClearValues_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_LaboriousnessValue.Text) &&
                string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_TimeToDevelopeValue.Text) &&
                string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_EAFValue.Text) &&
                string.IsNullOrEmpty(CocomoIIPostArchitecture_TextBox_EAFWithoutSCEDValue.Text))
            {
                MessageBox.Show
                    (
                    "Невозможно очистить пустое поле!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                CocomoIIPostArchitecture_Button_ClearValues.IsEnabled = false;
            }
            else
            {
                CocomoIIPostArchitecture_TextBox_LaboriousnessValue.Clear();
                CocomoIIPostArchitecture_TextBox_TimeToDevelopeValue.Clear();
                CocomoIIPostArchitecture_TextBox_EAFValue.Clear();
                CocomoIIPostArchitecture_TextBox_EAFWithoutSCEDValue.Clear();
            }
        }
        #endregion
    }
}