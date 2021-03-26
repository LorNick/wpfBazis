using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace wpfStatic
{
    ///<summary>Примеры</summary>
    class MyExamples
    {
    }

    // Telerik RadDateTimePicker
    //  <!--<telerik:RadDateTimePicker
    //              Name = "PART_DatePicker"
    //              MaxWidth="110"

    //              HorizontalAlignment="Left"
    //              DisplayFormat="Short"
    //              InputMode="DatePicker"

    //              TodayButtonVisibility="Visible"
    //              TodayButtonContent="Сегодня"
    //              DateTimeWatermarkContent="Введите дату"


    //              SelectedValue="{p:Binding ElementName=UserPole, Path=PROP_Text, Converter={ConvertTextToDateTime}, Mode=TwoWay}"

    //              DisplayDate="{p:Binding ElementName=UserPole, Path=PROP_Date}"
    //              DisplayDateStart="{p:Binding ElementName=UserPole, Path=PROP_ValueMinDate}"
    //              DisplayDateEnd="{p:Binding ElementName=UserPole, Path=PROP_ValueMaxDate}"
    //              SelectionChanged="PART_DatePicker_SelectedDateChanged">
    //</telerik:RadDateTimePicker>-->

    ///<summary>КЛАСС Для того, что бы не делать предварительное объявление в xaml</summary>
    public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter where T : class, new()
    {
        private static T _converter = null;

        ///<summary>КОНСТРУКТОР</summary>
        public ConverterMarkupExtension()
        {
        }

        ///<summary>ProvideValue</summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new T());
        }

        ///<summary>Convert</summary>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        ///<summary>ConvertBack</summary>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }

    ///<summary>КЛАСС Конвертор Возвращаем дату по тексту (dd.MM.yyyy) используя абстрактный класс ConverterMarkupExtension</summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ConvertTextToDateTime : ConverterMarkupExtension<ConvertTextToDateTime>
    {
        ///<summary>КОНСТРУКТОР</summary>
        public ConvertTextToDateTime()
        {
        }

        ///<summary>Convert</summary>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.ToString() == "") return null;
            try
            {
                return DateTime.Parse((string)value);
            }
            catch
            {
                return null;
            }
        }

        ///<summary>ConvertBack</summary>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    ///<summary>КЛАСС Конвертор Возвращаем дату по тексту (dd.MM.yyyy)</summary>
    class ConvertTextToDateTime1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return DateTime.Parse((string)value);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
