using System;
using System.Collections;
using System.Data;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using NLog;

namespace wpfStatic
{
    #region ---- Перечисления ----
    /// <summary>Тип шаблона</summary>
    public enum eTipNodes
    {
        /// <summary>Главная ветка</summary>
        Main,
        /// <summary>Информация</summary>
        Inform,
        /// <summary>Редактируемый документ</summary>
        EditDocum,
        /// <summary>Редактируемый шаблон Стационара</summary>
        Stac_Edit,
        /// <summary>Корневая ветка Стационара (только чтение)</summary>
        Stac_Roots,
        /// <summary>Списки Стационара (операции)</summary>
        Stac_RootsList,
        /// <summary>Добавочная ветка Стационара</summary>
        Stac_Add,
        /// <summary>Списки Параклиники</summary>
        Para_RootsList,
        /// <summary>Добавочная ветка Параклиники</summary>
        Para_Add,
        /// <summary>Списки Поликлиники</summary>
        Pol_RootsList,
        /// <summary>Добавочная ветка Поликлиники</summary>
        Pol_Add,
        /// <summary>Списки Kdl</summary>
        Kdl_RootsList,
        /// <summary>Добавочная ветка Kdl</summary>
        Kdl_Add,
        /// <summary>Списки Kdl PDF</summary>
        Kdl_RootsPdf,
        /// <summary>Добавочная ветка Kdl PDF</summary>
        Kdl_AddPdf
    }

    /// <summary>Тип документа</summary>
    public enum eTipDocum
    {
        /// <summary>Без протокола</summary>
        Null,
        /// <summary>Протоколы поликлиники apaN</summary>
        Pol,
        /// <summary>Протоколы стационара ast</summary>
        Stac,
        /// <summary>Протоколы параклиники par</summary>
        Paracl,
        /// <summary>Протоколы исследований КДЛ или другие документы kdl</summary>
        Kdl
    }

    /// <summary>Стадия шаблона</summary>
    public enum eStadNodes
    {
        /// <summary>Новый шаблон</summary>
        New,
        /// <summary>Новый шаблон, с данными предыдущего шаблона</summary>
        NewPerw,
        /// <summary>Старый шаблон</summary>
        Old,
    }

    /// <summary>Модули доступа</summary>
    public enum eModul
    {
        /// <summary>Администратор</summary>
        Admin = 1,
        /// <summary>Поликлиника (Врач)</summary>
        VrPolicl = 4,
        /// <summary>Параклиника (Врач)</summary>
        VrPara = 15,
        /// <summary>Стационар (Врач)</summary>
        VrStac = 16,
        /// <summary>Канцер регистр</summary>
        KancerReg = 21,
        /// <summary>Просмоторщик Истории болезни</summary>
        Viewer = 22,
        /// <summary>Для других ЛПУ</summary>
        OtherLpu = 24,
        /// <summary>Лаборатория</summary>
        Laboratory = 29,
        /// <summary>ЦАОП</summary>
        CAOP = 30
    }

    /// <summary>Тип закладки</summary>
    public enum eVkladki
    {
        /// <summary>Дерево</summary>
        Nodes = 1,
        /// <summary>Отчет</summary>
        Otchet = 2,
        /// <summary>Просмотр печати</summary>
        Print = 3,
        /// <summary>Форма</summary>
        Form = 4,
        /// <summary>Таблица</summary>
        Table = 5,
        /// <summary>Репорты</summary>
        Report = 6,
        /// <summary>PDF</summary>
        PDF = 7
    }

    /// <summary>Тип Вопросов</summary>
    public enum eVopros
    {
        /// <summary>1. Число UserPole_Number</summary>
        Number = 1,
        /// <summary>2. Десятичное UserPole_Text (поменять)</summary>
        Real = 2,
        /// <summary>3. Дата UserPole_Data</summary>
        Date = 3,
        /// <summary>4. Текст (короткий) UserPole_Text</summary>
        KrString = 4,
        /// <summary>5. Список UserPole_Text</summary>
        List = 5,
        /// <summary>6. Текст (длинный) UserPole_Text</summary>
        LoText = 6,
        /// <summary>7. Список (многовыборочный) UserPole_MultyList (Код операций)</summary>
        MultiList = 7,
        /// <summary>8. Свободный текст (основной) UserPole_Text</summary>
        Text = 8,
        /// <summary>9. Раздел UserPole_Razdel</summary>
        Razdel = 9,
        /// <summary>10. Скрытое поле UserPole_Text</summary>
        Hide = 10,
        /// <summary>11. Время UserPole_Text</summary>
        Time = 11,
        /// <summary>12. Радио кнопки UserPoleRadio_Button</summary>
        Radio = 12,
        /// <summary>13. Изображения UserPole_Image</summary>
        Image = 13,
        /// <summary>14. Таблица UserPole_Grid</summary>
        Grid = 14,
        /// <summary>15. Метка UserPole_Label</summary>
        Label = 15,
        /// <summary>16. Справочники UserPole_Sprav</summary>
        Sprav = 16,
        /// <summary>17. Календарь UserPole_Calendar</summary>
        Calendar = 17
    }

    /// <summary>Тип Тегов (Нерабочие)</summary>
    public enum eTipTegs
    {
        /// <summary>1. Целое число int</summary>
        Int = 1,
        /// <summary>2. Десятичное число real</summary>
        Real = 2,
        /// <summary>3. Дата date</summary>
        Date = 3,
        /// <summary>4. Текст string</summary>
        String = 4,
        /// <summary>5. Календарь calendar (даты через точку с запятой, в конце количество дней)</summary>
        Calendar = 5,
        /// <summary>6. Объект object</summary>
        Object = 6,
        /// <summary>7. Массив array (может включать в себя любой из выше перечисленных элементов)</summary>
        Array = 7
    }
    #endregion
    
    /// <summary>КЛАСС для Глобальных переменных</summary>
    public static class MyGlo
    {
        private static bool PRI_Admin;
        /// <summary>Администратор (берется из реестра)</summary>        
        public static bool PROP_Admin
        {
            get => PRI_Admin;
            set
            {
                PRI_Admin = value;
                MyPdf.MET_SetAccessPdf();
            }
        }

        #region ---- Public ----
        /// <summary>Переменная делегата (вызывает окно ошибки)</summary>
        public static Action<Exception> Event_Error;
        /// <summary>Переменная делегата (сохранть шаблон)</summary>
        public static Action<bool> Event_SaveShablon;
        /// <summary>Переменная делегата (обновляем окно, с новыми данными)</summary>
        public static Action<bool> Event_ReloadWindows;
        /// <summary>Переменная делегата (закрытие вкладки)</summary>
        public static Action<TabItem> Event_CloseTabItem;
        /// <summary>Переменная делегата (вызывает для записи сообщения в Log окно формы UserWindow_Lua)</summary>
        public static Action<string> Event_sLuaLog;
        /// <summary>Переменная делегата (открыли ветку PDF файла)</summary>
        public static Action<TreeViewItem> Event_OpenPdfNode;

        /// <summary>Контекстное меню</summary>
        public static ContextMenu ContextMenu;
        /// <summary>База</summary>
        public static DataSet DataSet = new DataSet();
        /// <summary>Дерево</summary>
        public static TreeView TreeView;

        /// <summary>Тип Модуля</summary>
        public static eModul TypeModul;
        /// <summary>Модуль</summary>
        public static object Modul;
        /// <summary>Максимальный индекс протокола</summary>
        public static int MaxImdex;
        /// <summary>Текущая запись стационара</summary>
        public static Hashtable HashAPSTAC;
        /// <summary>Текущая запись поликлининики</summary>
        public static Hashtable HashAPAC;
        /// <summary>Текущая запись пациента</summary>
        public static Hashtable HashKBOL;
        /// <summary>Последний диагноз пациента</summary>
        public static Hashtable HashLastDiag;
        /// <summary>История болезни</summary>
        public static Hashtable HashOtchet = new Hashtable();
        /// <summary>Цвет отчетов</summary>
        public static Brush BrushesOtchet;

        /// <summary>Код сервера (3- главный, 5 - филиал)</summary>
        public static int Server;
        /// <summary>Корпус (1- главный, 2 - филиал)</summary>
        public static int Korpus;

        /// <summary>Код пациента</summary>
        public static decimal KL;
        /// <summary>Полное ФИО пациента (КРИВКО Николай Васильевич)</summary>
        public static string FIO;
        /// <summary>Дата рождения</summary>
        public static string DR;
        /// <summary>Код пользователя</summary>
        public static int User;
        /// <summary>Имя пользователя</summary>
        public static string UserName;
        /// <summary>Пол (Мужской, Женский)</summary>
        public static string Pol;
        /// <summary>Код ЛПУ</summary>
        public static int Lpu;
        /// <summary>Стационар - Номер отделения (otd), поликлиника - код специальности (SPRS), параклиника - код элемента кабинета (parEL:Cod) (нужно отсюда убрать)</summary>
        public static int Otd;
        /// <summary>Код посещения в стационар (нужно отсюда убрать)</summary>
        public static decimal IND;
        /// <summary>Номер стац. карты (нужно отсюда убрать)</summary>
        public static int NSTAC;
        /// <summary>Дата поступления в стационар (нужно отсюда убрать)</summary>
        public static string DatePriem;
        /// <summary>Полная строка диагноза (из приемного отделения) (нужно отсюда убрать)</summary>
        public static string DiagStac;
        /// <summary>Показать удаленые протоколы</summary>
        public static bool ShowDeletedProtokol;
        /// <summary>Путь к wpfBazis</summary>
        public static string PathExe;
        /// <summary>Доступ на редактирование протоколов (берется из реестра)</summary>
        public static bool FlagEdit;
        /// <summary>Создаем логгер</summary>
        public static Logger PUB_Logger = LogManager.GetCurrentClassLogger();
        /// <summary>Наша база</summary>
        public static BazisDataContext PUB_Context;        
        #endregion

        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public static void MET_ComStr()
        {
            User = 848;    // пользователь "_"
            Server = 3;    // 1 - локальная база 2 - филиал от главного, 3 - главный, 5 - филиал, 6 - главный из вне
            TypeModul = eModul.Viewer;
#if DEBUG
            User = 60; // 5006;
            // Server = 2;
           // TypeModul = eModul.VrPolicl;
            //      TypeModul = eModul.VrStac;
            // TypeModul = eModul.List;
                TypeModul = eModul.Viewer;
            //   TypeModul = eModul.VrPara;
            // TypeModul = eModul.KancerReg;
            //  TypeModul = eModul.OtherLpu;
          // TypeModul = eModul.Laboratory;
            //TypeModul = eModul.CAOP;
#endif

            string[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                if (x == 1)
                    Server = Convert.ToInt32(_mArgs[1]);
                if (x == 2)
                    User = Convert.ToInt32(_mArgs[2]);
                if (x == 3)
                    TypeModul = (eModul)Convert.ToInt32(_mArgs[3]);
            }
            PUB_Context = new BazisDataContext(MySql.MET_ConSql());
            // Заполняем таблицу пользователей s_Users
            MySql.MET_DsAdapterFill("select * from dbo.s_Users", "s_Users");
            // Заполняем таблицу доступа текущего пользователя s_UsersDostup
            MySql.MET_DsAdapterFill($"select * from dbo.s_UsersDostup where UserCod = {User}", "s_UsersDostup");
            // ФИО пользователя
            UserName = MyMet.MET_UserName(User);
            // Поля логирования (есть ещё дубляж в основной форме)
            MyMet.MET_Log();
            // Корпус
            Korpus = MySql.MET_QueryInt(MyQuery.z_ConsttKorpus_Select_1());

            // Уровень доступа
            MyMet.MET_AccessWpf();
        }
    }
}
