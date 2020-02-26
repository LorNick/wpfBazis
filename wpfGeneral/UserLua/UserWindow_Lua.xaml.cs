using System;
using System.Linq;
using System.Windows;
using Neo.IronLua;
using wpfGeneral.UserControls;
using wpfGeneral.UserStruct;
using FastColoredTextBoxNS;

namespace wpfGeneral.UserLua
{
    /// <summary>КЛАСС Окно вопросов Shablon с редактором Lua</summary>
    public partial class UserWindow_Lua
    {
        /// <summary>Текущий вопрос шаблона</summary>
        private UserShablon PRI_Shablon;

        /// <summary>Документ шаблона</summary>
        private readonly UserDocument PRI_Document;

        /// <summary>Lua Поле</summary>
        private FastColoredTextBox PART_FCTextBox;

        /// <summary>Стиль моих ключевых Lua слов</summary>
        private FastColoredTextBoxNS.Style PRI_Style = new TextStyle(System.Drawing.Brushes.DarkViolet, null, System.Drawing.FontStyle.Bold);

        /// <summary>Перечень моих ключевых Lua слов (в регулярном выражении)</summary>
        private string PRI_Regular;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Lua()
        {
            InitializeComponent();

            PART_Log.PART_TextBox.MaxHeight = 80;
        }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pPole">Поле с которым работаем</param>
        public UserWindow_Lua(VirtualPole pPole)
        {
            InitializeComponent();

            PART_Log.PART_TextBox.MaxHeight = 80;

            // Родительское окно
            if (Owner == null)
                Owner = Application.Current.MainWindow;

            // Настраиваем положение окна
            Left = 0;
            if (Owner != null)
            {
                Top = Owner.Top - 130;
                Owner.Left = Width - 15;
#if DEBUG
                Left = Owner.Left + Owner.Width + 500;
#endif
            }

            // Находим вопрос шаблона
            PRI_Document = pPole.PROP_Docum;
            PRI_Shablon = PRI_Document.PROP_Shablon.Find(x => x.PROP_VarId == pPole.PROP_VarId);
            this.DataContext = PRI_Shablon;

            // Настраиваем Lua поле
            PRI_Regular = @"(lPole|Pole|lMessage|lKbolInfoAdd|lKbolInfoDel|lPolePDate|";
            PRI_Regular += "lOperInfo|lOperInfoDel|lRead|lNew|lDateIf|lTimeGosp|lSqlToStr|";
            PRI_Regular += "lKbolInfoOms|";
            PRI_Regular += "OnCreat|OnChange|OnBeforeSave|OnSave|";
            PRI_Regular += "PROP_Text|OnChange|OnBeforeSave|OnSave)";

            PART_FCTextBox = new FastColoredTextBox();
            PART_FCTextBox.Language = FastColoredTextBoxNS.Language.Lua;
            PART_FCTextBox.TextChanged += Ts_TextChanged;
            PART_FCTextBox.Text = PRI_Shablon.PROP_xLua;
            PART_FCTextBox.WordWrap = true;
            PART_WindowsFormsHostFCTB.Child = PART_FCTextBox;
        }        

        /// <summary>СОБЫТИЕ Редактируем поле Lua</summary>
        private void Ts_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Отчищаем маркер свертывания кода
            e.ChangedRange.ClearFoldingMarkers();
            // Маркер для свертывания кода
            e.ChangedRange.SetFoldingMarkers("if", "end");
            e.ChangedRange.SetFoldingMarkers(@"function\b", @"end;\b");  
            // Ставим стиль для ключевых слов (не забывать его обновлять, по мере добавления)
            e.ChangedRange.ClearStyle(PRI_Style);
            e.ChangedRange.SetStyle(PRI_Style, PRI_Regular);
            // Меняем высоту окна
            PART_WindowsFormsHostFCTB.Height = PART_FCTextBox.LinesCount * 16;

            PRI_Shablon.PROP_xLua = PART_FCTextBox.Text;
        }

        /// <summary>СОБЫТИЕ Закрыть программу</summary>
        private void PART_ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            // Запрет на проверку правописания
            PART_ValueStart.PART_TextBox.SpellCheck.IsEnabled = false;
            PART_xFormat.PART_TextBox.SpellCheck.IsEnabled = false;
            PART_Log.PART_TextBox.SpellCheck.IsEnabled = false;
        }

        /// <summary>СОБЫТИЕ Изменения статуса окна, если оно свернуто - сворачивает главное оконо</summary>
        private void UserWindows_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                if (Owner != null)
                {
                    Owner.WindowState = WindowState.Minimized;
                }
            }
        }

        /// <summary>СОБЫТИЕ При закрытии окна</summary>
        private void UserWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner?.Activate();
        }

        /// <summary>СОБЫТИЕ Сохранить все изменённые вопросы данного шаблона в SQL</summary>
        private void PART_ButtonReturn_Click(object sender, RoutedEventArgs e)
        {
            PRI_Shablon.MET_SaveSQL();
        }

        /// <summary>СОБЫТИЕ Переходим на нижний вопрос (или на первый, если был последний)</summary>
        private void PART_ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            PRI_Shablon = PRI_Document.PROP_Shablon.Find(x => PRI_Document.PROP_Shablon.Max(i => i.PROP_Nomer) == PRI_Shablon.PROP_Nomer
                ? x.PROP_Nomer == PRI_Document.PROP_Shablon.Min(i => i.PROP_Nomer) 
                    : x.PROP_Nomer == PRI_Shablon.PROP_Nomer + 1);

            this.DataContext = PRI_Shablon;
            PART_FCTextBox.Text = PRI_Shablon.PROP_xLua;
        }

        /// <summary>СОБЫТИЕ Переходим на верхний вопрос (или на последний, если был первый)</summary>
        private void PART_ButtonUp_Click(object sender, RoutedEventArgs e)
        {
            PRI_Shablon = PRI_Document.PROP_Shablon.Find(x => PRI_Document.PROP_Shablon.Min(i => i.PROP_Nomer) == PRI_Shablon.PROP_Nomer 
                ? x.PROP_Nomer == PRI_Document.PROP_Shablon.Max(i => i.PROP_Nomer) 
                    : x.PROP_Nomer == PRI_Shablon.PROP_Nomer - 1);

            this.DataContext = PRI_Shablon;
            PART_FCTextBox.Text = PRI_Shablon.PROP_xLua;
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Тестирование кусков Lua</summary>
        private void PART_ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            Lua _Lua = new Lua();
            
            LuaCompileOptions _Options = new LuaCompileOptions();
            _Options.DebugEngine = new LuaStackTraceDebugger();
            try
            {
                _Lua.CompileChunk(PART_FCTextBox.Text, "Test.lua", _Options);
                MET_LogAdd("Успех");

            }
            catch (LuaParseException ex)
            {
                MET_LogAdd($"Ошибка в строке {ex.Line}: {ex.Message}");
            }
            catch (Exception er)
            {
                MET_LogAdd($"Ошибка: {er.Message}");
            }            
        }

        /// <summary>МЕТОД Добавляем сообщение Лога</summary>
        /// <param name="pText">Сообщение лога</param>
        private void MET_LogAdd(string pText)
        {
            if (PART_Log.PROP_Text == "" || PART_Log.PROP_Text.Length > 1200)
                PART_Log.PROP_Text = DateTime.Now.ToLongTimeString() + " :  " + pText;
            else
                PART_Log.PROP_Text = DateTime.Now.ToLongTimeString() + " :  " + pText + '\n' + PART_Log.PROP_Text;
        }
    }
}
