using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для Кнопки</summary>
    public partial class User_Button : Button
    {
        /// <summary>СВОЙСТВО Скрывает, открывает кнопку выбора</summary>
        public string PROP_Image
        {
            get { return (string)GetValue(DEPR_FlagButtonSelectProperty); }
            set { SetValue(DEPR_FlagButtonSelectProperty, value); }
        }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_FlagButtonSelect</summary>
        public static readonly DependencyProperty DEPR_FlagButtonSelectProperty =
            DependencyProperty.Register("PROP_FlagButtonSelect", typeof(string), typeof(User_Button), new PropertyMetadata("mnAnest.ico"));

      
        /// <summary>КОНСТРУКТОР</summary>
        public User_Button()
        {
            InitializeComponent();

       //     PART_TextBox.ContextMenu = MyGlo.ContextMenu;
        }

    }
   
    /// <summary>КЛАСС Конвертр (Пытаемся прицыпить иконку к нопке)</summary>
    public class IconImageConverter : IValueConverter
    {
        /// <summary>МЕТОД Возвращаем иконку</summary>
        public object Convert(object value,
                           Type targetType,
                           object parameter,
                           System.Globalization.CultureInfo culture)
        {
            // if value isn't null, we can safely do the conversion. 
            if (value != null)
            {
                string imageName = value.ToString();
                Uri uri = new Uri(String.Format("pack://application:,,,/wpfResource;component/mnImag/{0}", imageName), UriKind.Relative);
                return new BitmapImage(uri);
            }
            return null;
        }

        /// <summary>МЕТОД ...</summary>
        public object ConvertBack(object value,
                             Type targetType,
                             object parameter,
                             System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
