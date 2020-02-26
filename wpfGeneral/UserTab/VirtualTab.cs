using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfStatic;

namespace wpfGeneral.UserTab
{
    /// <summary>КЛАСС Виртуальное поле Ветки (владки)</summary>
    public abstract class VirtualTab : StackPanel
    {
        /// <summary>Текст ветки</summary>
        protected TextBlock PRO_TextBlock;

        /// <summary>Текст ветки (нижняя строка)</summary>
        protected TextBlock PRO_TextBlockDown  = new TextBlock();


        /// <summary>СВОЙСТВО Текст ветки</summary>
        public string PROP_Text
        {
            get { return PRO_TextBlock.Text; }
            set { PRO_TextBlock.Text = value; }
        }

        /// <summary>СВОЙСТВО Текст ветки (нижняя строка)</summary>
        public string PROP_TextDown
        {
            get { return PRO_TextBlockDown.Text; }
            set { PRO_TextBlockDown.Text = value; }
        }


        /// <summary>КОНСТРУКТОР (пустой)</summary>
        protected VirtualTab() { }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pImageSource">Иконка</param>
        /// <param name="pText">Текст</param>
        /// <param name="pHeight">Высота иконки</param>
        /// <param name="pDate">Текст даты</param>
        protected VirtualTab(ImageSource pImageSource, string pText, int pHeight, string pDate)
        {
            // Настраиваем StackPanel
            this.Orientation = Orientation.Horizontal;
            this.Margin = new Thickness(0,0,0,0);
            // Настраиваем картинку
            Image _Image = new Image();
            _Image.Height = pHeight;
            _Image.Source = pImageSource;
            this.Children.Add(_Image);
            // Вложенная вертикальная стак-панель
            StackPanel _StackPanel = new StackPanel();
            _StackPanel.Orientation = Orientation.Vertical;
            _StackPanel.Margin = new Thickness(2, 0, 4, 0);
            _StackPanel.VerticalAlignment = VerticalAlignment.Center;
            this.Children.Add(_StackPanel);
            // Настраиваем текст
            PRO_TextBlock = new TextBlock();
            PRO_TextBlock.Text = pText;
            PRO_TextBlock.Padding = new Thickness(0);
            PRO_TextBlock.VerticalAlignment = VerticalAlignment.Center;
            PRO_TextBlock.Margin = new Thickness(2, 0, 4, 0);
            PRO_TextBlock.Padding = new Thickness(0);            
            _StackPanel.Children.Add(PRO_TextBlock);
            // Нижний текст (Дата)
            PRO_TextBlockDown.Text = pDate;
            PRO_TextBlockDown.VerticalAlignment = VerticalAlignment.Bottom;
            PRO_TextBlockDown.Padding = new Thickness(0);
            PRO_TextBlockDown.FontSize = 11;
            PRO_TextBlockDown.Height = 13;
            PRO_TextBlockDown.Foreground = Brushes.BlueViolet;
            _StackPanel.Children.Add(PRO_TextBlockDown);                       
        }

        /// <summary>МЕТОД Признак удаления ветки (зачеркивание текста)</summary>
        /// <param name="pDelete">true - удалено, false - не удалено</param>
        public void MET_Delete(bool pDelete)
        {
            if (pDelete)
                PRO_TextBlock.TextDecorations = TextDecorations.Strikethrough;
            else
                PRO_TextBlock.TextDecorations = null;
        }
    }

    // ========================================================================

    /// <summary>КЛАСС Поле Ветки</summary>
    public class UserTabNod : VirtualTab
    {
        /// <summary>КОНСТРУКТОР (пустой)</summary>
        public UserTabNod() { }

        /// <summary>КОНСТРУКТОР (пустой)</summary>
        /// <param name="pImageSource">Иконка</param>
        /// <param name="pText">Текст</param>
        /// <param name="pDate">Дата данной ветки</param>
        public UserTabNod(ImageSource pImageSource, string pText, string pDate = "")
            : base(pImageSource, pText, 32, pDate) { }
    }

    // ========================================================================

    /// <summary>КЛАСС Поле Закладки</summary>
    public class UserTabVrladka : VirtualTab
    {
        /// <summary>КОНСТРУКТОР (пустой)</summary>
        public UserTabVrladka() { }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pImageSource">Иконка</param>
        /// <param name="pText">Текст</param>
        /// <param name="pParent">Родитель</param>
        /// <param name="pVkladki">Тип закладки</param>
        /// <param name="pClose">Кнопка закрыть закладку</param>
        /// <param name="pDate">Дата</param>
        public UserTabVrladka(ImageSource pImageSource, string pText, object pParent, eVkladki pVkladki, bool pClose, string pDate = "")
            : base(pImageSource, pText, 22, pDate)
        {   
            // В зависимости от вкладки меняем цвет
            switch (pVkladki)
            {
                case eVkladki.Print:
                    this.Background = Brushes.Aqua;
                    break;
                case eVkladki.Form:
                    this.Background = Brushes.Bisque;
                    break;
                case eVkladki.Report:
                    this.Background = Brushes.GreenYellow;
                    break;
            }

            // Настраиваем кнопку "Закрыть"
            if (pClose)
            {
                // Настраиваем кнопку
                Button _ButtonClose = new Button();
                _ButtonClose.Height = 20;
                _ButtonClose.Width = 20;
                _ButtonClose.Focusable = false;
                _ButtonClose.Tag = pParent;
                _ButtonClose.Click += PART_Close_Click;
                this.Children.Add(_ButtonClose);
                // Настраиваем картинку, кнопки Закрыть
                Image _CloseImage = new Image();
                _CloseImage.VerticalAlignment = VerticalAlignment.Stretch;
                _CloseImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                _CloseImage.Source = (BitmapImage)FindResource("btsClose");
                _ButtonClose.Content = _CloseImage;
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Закрыть шаблон"</summary>
        private void PART_Close_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем делегат закрытия вкладки
            MyGlo.callbackEvent_sClose((sender as Button).Tag as TabItem);
        }
    }
}
