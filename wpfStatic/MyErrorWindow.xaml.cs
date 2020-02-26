using System;
using System.Collections;
using System.Windows;

namespace wpfStatic
{
    /// <summary>КЛАСС Окна ошибки</summary>
    public partial class MyErrorWindow : Window
    {
        /// <summary>КОНСТРУКТОР</summary>
        public MyErrorWindow()
        {
            InitializeComponent();

            // Для пользователя просмотра из вне КОДа, убираем контактную информацию
            if (MyGlo.User == 848)
            {
                PART_Label1.Text = "";
                PART_Label2.Text = "";
            }
        }

        /// <summary>СОБЫТИЕ Выводим информацию об ошибке</summary>
        /// <param name="pException">Объект ошибки</param>
        public void MET_ErrorObject(Exception pException)
        {
            // Уменьшаем межстрочное пространство
            PART_ErrorText.Document.Blocks.FirstBlock.LineHeight = 1;
            //  Ошибка
            PART_ErrorText.AppendText("--- Message:\n");
            PART_ErrorText.AppendText(pException.Message);
            PART_ErrorText.AppendText("\n\n--- StackTrace:\n");
            PART_ErrorText.AppendText(pException.StackTrace);
            PART_ErrorText.AppendText("\n\n--- Source:\n");
            PART_ErrorText.AppendText(pException.Source);
            foreach (DictionaryEntry _Data in pException.Data)
            {
                PART_ErrorText.AppendText("\n\n--- " + _Data.Key + ":\n");
                PART_ErrorText.AppendText(_Data.Value.ToString());
            }  
        }

        /// <summary>СОБЫТИЕ Закрыть программу</summary>
        private void PART_ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>СОБЫТИЕ Переподключиться к серверу</summary>
        private void PART_ButtonReturn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
