using System.Windows;
using wpfStatic;

namespace wpfReestr
{
    /// <summary>КЛАСС Меняем в КЛАДРЕ старый ОКАТО на новый</summary>
    public partial class MyUpdateOKATO : Window
    {
        private string oldOKATO;
        private string newOKATO;

        /// <summary>КОНСТРУКТОР</summary>
        public MyUpdateOKATO()
        {
            InitializeComponent();

            // Родительская форма (что бы окно не пропадало)
            Owner = Application.Current.MainWindow;
        }
                     
        /// <summary>СОБЫТИЕ Нажали на кнопку Проверить старый ОКАТО</summary>
        private void PART_ButtonOldOKATO_Click(object sender, RoutedEventArgs e)
        {
            oldOKATO = PART_TextBoxOldOKATO.Text;
            if (oldOKATO == "")
            {
                PART_RichTextBox.AppendText("Введите в поле Старый номер ОКАТО\n");
                return;
            }

            // Ищем старый ОКАТО
            string _oldAdres = MySql.MET_QueryStr($"select isnull((select SOCR + '. ' + NAME + '; ' from Bazis.dbo.KLADR where  OCATD = {oldOKATO} for xml path('')), 'не нашел')");
            PART_RichTextBox.AppendText($"### Старый ОКАТО: {oldOKATO}\n");
            PART_RichTextBox.AppendText($"Адреса: {_oldAdres}\n");
            if (_oldAdres == "не нашел" || _oldAdres == "")
            {
                PART_ButtonNewOKATO.IsEnabled = false;
                newOKATO = "";
                return;
            }
            PART_ButtonNewOKATO.IsEnabled = true;
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Загрузить справочник врачей</summary>
        private void PART_ButtonNewOKATO_Click(object sender, RoutedEventArgs e)
        {
            newOKATO = PART_TextBoxNewOKATO.Text;
            if (newOKATO == "" || oldOKATO == "")
            {
                PART_RichTextBox.AppendText("Введите в поле правильный Новый номер ОКАТО\n");
                return;
            }
            if (newOKATO.Length != 11)
            {
                PART_RichTextBox.AppendText($"Должно быть 11 символов, а у вас их {newOKATO.Length}\n");
                return;
            }
            if (newOKATO == oldOKATO)
            {
                PART_RichTextBox.AppendText($"Ни чё что они у вас равны!?!\n");
                return;
            }

            bool _isUpdate = MySql.MET_QueryNo($"update Bazis.dbo.KLADR set OCATD = {newOKATO} where OCATD = {oldOKATO}");
            
            if (_isUpdate)
            {
                PART_RichTextBox.AppendText($"Успех, поменял {oldOKATO} нa {newOKATO}\n\n");
                PART_ButtonNewOKATO.IsEnabled = false;
                oldOKATO = "";
                newOKATO = "";
                PART_TextBoxOldOKATO.Text = "";
                PART_TextBoxNewOKATO.Text = "";
            }
            else
            {
                PART_RichTextBox.AppendText($"БЯДА! Не получилось поменять {oldOKATO} нa {newOKATO}\n");
            }
        }
    }
}