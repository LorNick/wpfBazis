using System.Windows.Media;
using wpfStatic;

namespace wpfBazis
{
    /// <summary>КЛАСС Окна "О программе"</summary>
    public partial class About
    {
        private int PRI_Flag;

        /// <summary>КОНСТРУКТОР</summary>
        public About()
        {
            InitializeComponent();
            // IP Сервера
            string _Server = "";
            switch (MyGlo.Server)
            {
                case 1:                                                         // Локальный
                    _Server = "   Server: (1) 127.0.0.1;";
                    break;
                case 2:                                                         // Удаленное подключение к филиалу
                    _Server = "   Server: (2) 10.30.104.5;";
                    break;
                case 3:                                                         // Главный корпус
                    _Server = "   Server: (3) 192.168.0.8;";
                    break;
                case 5:                                                         // Филиал
                    _Server = "   Server: (5) 192.168.0.8;";
                    break;
                case 6:                                                         // Главный из вне
                    _Server = "   Server: (6) 10.30.103.8;";
                    break;
            }
            // Версия программы и IP серевера
            PART_Ver.Text = "Версия " + MyMet.MET_Ver() + _Server;
            // Если включен режим Админа
            if (MyGlo.PROP_Admin)
            {
                PRI_Flag = 3;
                PART_Bazis.Foreground = new SolidColorBrush(Colors.BlueViolet);
            }
        }

        /// <summary>Включаем/Выключаем режим админа 1е действие</summary>
        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PRI_Flag++;
            if (PRI_Flag == 2)
                PART_Bazis.Foreground = new SolidColorBrush(Colors.Red);
            else
                PART_Bazis.Foreground = new SolidColorBrush(Colors.Black);
            if (PRI_Flag > 2)
                PRI_Flag = 0;
            MyGlo.PROP_Admin = false;                                               // режим админа выключен
        }

        /// <summary>Включаем/Выключаем режим админа 2е действие</summary>
        private void TextBlock_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PRI_Flag++;
            if (PRI_Flag == 3)
            {
                PART_Bazis.Foreground = new SolidColorBrush(Colors.BlueViolet);
                MyGlo.PROP_Admin = true;                                             // режим админа включен
            }
            else
            {
                PART_Bazis.Foreground = new SolidColorBrush(Colors.Black);
                MyGlo.PROP_Admin = false;                                            // режим админа выключен
            }
        }
    }
}
