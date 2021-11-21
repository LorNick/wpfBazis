using System;
using System.Collections.Generic;
using System.Windows;
using wpfGeneral.UserStruct;
using wpfGeneral.UserFormShablon;
using wpfStatic;
using Neo.IronLua;
using wpfGeneral.UserControls;

namespace wpfGeneral.UserLua
{
    /// <summary>КЛАСС Для программирования элементов Lua</summary>
    public class UserLua_Standart
    {
        private readonly Lua PRI_Lua = new Lua();
        private readonly dynamic PRI_Env;

        /// <summary>Наша форма с шаблоном</summary>
        public UserFormShablon_Standart PUB_FormShablon;

        /// <summary>Текущее поле</summary>
        public VirtualPole PUB_Pole;

        /// <summary>СВОЙСТВО Кусок кода Lua</summary>
        public string PROP_ChankText { get; set; }

        /// <summary>СВОЙСТВО Наличие в кусоке кода Lua функции OnCreat</summary>
        public bool PROP_OnCreat { get; set; }

        /// <summary>СВОЙСТВО Наличие в кусоке кода Lua функции OnSave</summary>
        public bool PROP_OnSave { get; set; }

        /// <summary>СВОЙСТВО Наличие в кусоке кода Lua функции OnBeforeSave</summary>
        public bool PROP_OnBeforeSave { get; set; }

        /// <summary>СВОЙСТВО Наличие в кусоке кода Lua функции OnChange</summary>
        public bool PROP_OnChange { get; set; }

        /// <summary>КОНСТРУКТОР</summary>
        public UserLua_Standart()
        {
            PRI_Env = PRI_Lua.CreateEnvironment();
        }

        /// <summary>КОНСТРУКТОР 2</summary>
        public UserLua_Standart(VirtualPole pPole)
        {
            PRI_Env = PRI_Lua.CreateEnvironment();
            PUB_Pole = pPole;
            MET_RegisterFunction();
        }

        /// <summary>МЕТОД Регистрация функций</summary>
        private void MET_RegisterFunction()
        {
            // Обязательно добавить имя функции в этот файл, для подсветки кода
            // E:\Nick\C#\wpfBazis\wpfGeneral\UserLua\UserWindow_Lua.cs
            // Подключаем функции для считывания глобальных данных
            // Выводим окно сообещиния
            PRI_Env.lMessage = new Action<string, string>(lMessage);
            // Выводим сообещние в лог окно UserWindow_Lua
            PRI_Env.lLog = new Action<string>(lLog);
            // Отображаем поля в шаблоне, указанные в массиве VarId
            PRI_Env.lVisiblOn = new Action<int[]>(lVisiblOn);
            // Скрываем поля в шаблоне, указанные в массиве VarId
            PRI_Env.lVisiblOff = new Action<int[]>(lVisiblOff);
            // Делаем обязательными поля, указанные в массиве VarId
            PRI_Env.lNecesOn = new Action<int[]>(lNecesOn);
            //  Делаем НЕобязательными поля, указанные в массиве VarId
            PRI_Env.lNecesOff = new Action<int[]>(lNecesOff);
            //  Отчистка текста полей, указанные в массиве VarId
            PRI_Env.lTextClear = new Action<int[]>(lTextClear);
            // Добавление/изменение данных в таблицу kbolInfo
            PRI_Env.lKbolInfoAdd = new Func<string, object, string, decimal, string, bool>(lKbolInfoAdd);
            // Удаление данных из таблицы kbolInfo
            PRI_Env.lKbolInfoDel = new Func<string, string, decimal, string, bool>(lKbolInfoDel);
            // Добавление признака ОМС в kbolInfo 0 - не подаем в реестры, 1 - подаем реестры в ОМС
            PRI_Env.lKbolInfoOms = new Func<int, bool>(lKbolInfoOms);
            // Подключаем функцию для Ссылки на поле по VarId
            PRI_Env.lPole = new Func<int, object>(lPole);
            // Подключаем функцию для Ссылки на поле pDate
            PRI_Env.lPolePDate = new Func<object>(lPolePDate);
            // Добавление/изменение данных в поле xInfo, таблицы Oper
            PRI_Env.lOperInfo = new Func<int, string, object, bool>(lOperInfo);
            // Удаление данных из поля xInfo, таблицы Oper
            PRI_Env.lOperInfoDel = new Func<int, string, bool>(lOperInfoDel);
            // Подключаем функцию для считывания глобальных данных
            PRI_Env.lRead = new Func<string, string, object>(lRead);
            // Новый протокол или редактируемый (старый)
            PRI_Env.lNew = new Func<bool>(lNew);
            // Текущее поле
            PRI_Env["Pole"] = PUB_Pole;
            // Сравниваем текстовые 2 даты
            PRI_Env.lDateIf = new Func<string, string, int>(lDateIf);
            // Время госпитализации
            PRI_Env.lTimeGosp = new Func<string>(lTimeGosp);
            // Выполняем SQL запрос, возвращаем текст
            PRI_Env.lSqlToStr = new Func<string, string>(lSqlToStr);

            // Подключаем функцию для Ссылки на поле по VarId
            //PRI_Env.lFocusPole = new Func<int, bool>(lFocusPole);
        }

        #region ---- Методы скриптов Lua ---
        /// <summary>Lua Функция. Вывод окна сообщения</summary>
        /// <param name="pMessage">Строка сообщения</param>
        /// <param name="pHeader">Заголовок окна</param>
        private void lMessage(string pMessage, string pHeader)
        {
            MessageBox.Show(pMessage, pHeader);
        } // func lMessage

        /// <summary>Lua Функция. Вывод сообщения в лог окно UserWindow_Lua</summary>
        /// <param name="pText">Строка сообщения</param>
        private void lLog(string pText)
        {
            // Если открыто окно UserWindows_Lua, то выводим в логи код VarId вопроса и заданный текст
            MyGlo.Event_sLuaLog?.Invoke($"VarId{PUB_Pole.PROP_VarId}: {pText}");
        } // func lLog

        /// <summary>Lua Функция. Добавление/изменение таблицы kbolInfo</summary>
        /// <param name="pTag">Имя ключа тега</param>
        /// <param name="pValue">Значение</param>
        /// <param name="pRazdKey">Имя ключа раздела (если нет, то не используем раздел)</param>
        /// <param name="pRazdCod">Код раздела (не ставим, если нет раздела)</param>
        /// <param name="pTab">К какой таблицы привязываемся (kbol, pol, stac), по умолчанию привязываем в зависимости от типа шаблона</param>
        /// <returns>Возвращаем успех или не успех добавления/изменения данных</returns>
        /// <remarks>Реализовано только указание параметров pTag, pValue</remarks>
        private bool lKbolInfoAdd(string pTag, object pValue = null, string pRazdKey = "", decimal pRazdCod = 0,  string pTab = "")
        {
            // Значение
            object _Value = pValue ?? PUB_Pole.PROP_Text; // : pValue;
            // Смотрим наличие раздела
            //string _Razdel = pRazdCod == 0 ? "" : $", '{pRazdKey}', {pRazdCod}";
            // Код записи (Cod из Apac, IND  из Apstac, KL из kbol
            decimal _CodZap = MyGlo.IND;
            // Смотрим наличие указанной таблицы pTab
            pTab = pTab == "" ? PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_KbolInfo : pTab;
            UserKbolInfo _KbolInfo = UserKbolInfo.MET_FactoryKbolInfo(pTab, _CodZap, MyGlo.KL);
            _KbolInfo.MET_Change(pTag, _Value);
            //string _Query = $"exec dbo.jsonInfoAdd {_Cod}, '{pTab}', '{pKey}', '{_Value}', '' {_Razdel}";
            //MySql.MET_QueryNo(_Query);
            return true;
        } // func lKbolInfoAdd

        /// <summary>Lua Функция. Удаление данных из таблицы kbolInfo</summary>
        /// <param name="pTag">Имя ключа тега</param>
        /// <param name="pRazdKey">Имя ключа раздела (если нет, то не используем раздел)</param>
        /// <param name="pRazdCod">Код раздела (не ставим, если нет раздела)</param>
        /// <param name="pTab">К какой таблицы привязываемся (kbol, pol, stac, par), по умолчанию привязываем в зависимости от типа шаблона</param>
        /// <returns>Возвращаем успех или не успех добавления/изменения данных</returns>
        /// <remarks>Реализовано только указание параметров pTag</remarks>
        private bool lKbolInfoDel(string pTag, string pRazdKey = "", decimal pRazdCod = 0, string pTab = "")
        {
            // Смотрим наличие раздела
            //string _Razdel = pRazdCod == 0 ? "" : $", '{pRazdKey}', {pRazdCod}";
            // Код записи (Cod из Apac, IND  из Apstac, KL из kbol
            decimal _CodZap = MyGlo.IND;
            // Смотрим наличие указанной таблицы pTab
            pTab = pTab == "" ? PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_KbolInfo : pTab;
            UserKbolInfo _KbolInfo = UserKbolInfo.MET_FactoryKbolInfo(pTab, _CodZap, MyGlo.KL);
            _KbolInfo.MET_Delete(pTag);
            return true;
        } // func lKbolInfoDel

        /// <summary>Lua Функция. Добавление признака ОМС в kbolInfo</summary>
        /// <param name="pOms">Значения поля ОМС: 0 - не подавать в реестры, 1 - подавать в реестры ОМС</param>
        /// <returns>Возвращаем успех или не успех изменения данных</returns>
        private bool lKbolInfoOms(int pOms = 1)
        {
            // Код записи (Cod из Apac, IND  из Apstac, Cod из parObsledov
            decimal _CodZap = MyGlo.IND;
            // Смотрим наличие указанной таблицы pTab
            string _Tab = PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_KbolInfo;
            UserKbolInfo _KbolInfo = UserKbolInfo.MET_FactoryKbolInfo(_Tab, _CodZap, MyGlo.KL);
            _KbolInfo.MET_ChangeOms(pOms);
            return true;
        } // func lKbolInfoOms

        /// <summary>Lua Функция. Ссылка на поле по VarId</summary>
        /// <param name="pVarId">Код поля</param>
        private VirtualPole lPole(int pVarId)
        {
            return PUB_Pole.PROP_FormShablon.GetPole(pVarId);
        } // func lPole

        /// <summary>Lua Функция. Ссылка на поле pDate</summary>
        private VirtualPole lPolePDate()
        {
            return PUB_Pole.PROP_FormShablon.GetPole("DateOsmotr");
        } // func lPolePDate

        /// <summary>Lua Функция. Добавление/изменение данных в поле xInfo, таблицы Oper</summary>
        /// <param name="pVarId">VarId поля с кодом операции</param>
        /// <param name="pTag">Имя ключа тега</param>
        /// <param name="pValue">Значение, по умолчанию, берем из поля</param>
        /// <returns>Возвращаем успех или не успех добавления/изменения данных</returns>
        private bool lOperInfo(int pVarId, string pTag, object pValue = null)
        {
            // Находим наше поле
            VirtualPole _VirtualPole = PUB_Pole.PROP_FormShablon.GetPole(pVarId);

            // Проверяем на соответствие типу  UserPole_MultyList
            if (_VirtualPole is UserPole_MultyList)
            {
                // Значение
                object _Value = pValue ?? PUB_Pole.PROP_Text;
                UserPole_MultyList _Pole = (UserPole_MultyList) _VirtualPole;
                _Pole.MET_ChangeInfo(pTag, _Value);
                return true;
            }
            return false;
        } // func lOperInfo

        /// <summary>Lua Функция. Удаление данных из поля xInfo, таблицы Oper</summary>
        /// <param name="pVarId">VarId поля с кодом операции</param>
        /// <param name="pTag">Имя ключа тега</param>
        /// <returns>Возвращаем успех или не успех добавления/изменения данных</returns>
        private bool lOperInfoDel(int pVarId, string pTag)
        {
            // Находим наше поле
            VirtualPole _VirtualPole = PUB_Pole.PROP_FormShablon.GetPole(pVarId);
            // Проверяем на соответствие типу  UserPole_MultyList
            if (_VirtualPole is UserPole_MultyList _Pole)
            {
                _Pole.MET_DeleteInfo(pTag);
                return true;
            }
            return false;
        } // func lOperInfo

        /// <summary>Lua Функция. Расчет времени госпитализации</summary>
        /// <returns>Возвращаем расчетное время госпитализации</returns>
        private string lTimeGosp()
        {
            // Находим отделение госпитализации
            VirtualPole _PoleOtd = PUB_Pole.PROP_FormShablon.GetPole(3);
            // Находим дату госпитализации
            VirtualPole _PoleDate = PUB_Pole.PROP_FormShablon.GetPole(4);
            // Время по умолчанию
            string _Time = "9:00";
            if (_PoleOtd.PROP_Text != "" && _PoleDate.PROP_Text != "")
            {
                try
                {
                    int _Otd = int.Parse(_PoleOtd.PROP_Text.Split('.')[0]);
                    _Time = MySql.MET_QueryStr(MyQuery.MET_varTimeGosp(_Otd, _PoleDate.PROP_Text));
                }
                catch
                {
                    _Time = "9:00";
                }
            }
            return _Time;
        } // func lTimeGosp

        /// <summary>Lua Функция. Выполняем SQL запрос</summary>
        /// <param name="pSql">Строка SQL запроса</param>
        /// <returns>Возвращаем одно значение, в виде текста (если нет, то пусто)</returns>
        private string lSqlToStr(string pSql)
        {
            // Результат
            string _Value = "";
            if (pSql != "")
            {
                try
                {
                    _Value = MySql.MET_QueryStr(pSql);
                }
                catch
                {
                    _Value = "";
                }
            }
            return _Value;
        } // func lSqlToStr

        /// <summary>Lua Функция. Сравниваем текстовые 2 даты (в Разработке)</summary>
        /// <returns>Возвращаем -1 если первая дата меньше второй, 0 если даты равны, 1 если первая дата больше 2й, 99 если это не даты</returns>
        private int lDateIf(string pDate1, string pDate2)
        {
            int _Rezult;
            if (DateTime.TryParse(pDate1, out DateTime _Date1) && DateTime.TryParse(pDate2, out DateTime _Date2))
                _Rezult = _Date1.CompareTo(_Date2);
            else
                _Rezult = 99;

            return _Rezult;
        } // func lDateIf

        /// <summary>Lua Функция. Отображаем поля в шаблоне, указанные в массиве VarId</summary>
        /// <param name="pVarId">Перечень кодов VarId</param>
        private void lVisiblOn(params int[] pVarId)
        {
            for (int i = 0; i < pVarId.Length; i++)
            {
                PUB_Pole.PROP_FormShablon.GetPole(pVarId[i]).Visibility = Visibility.Visible;
            }
        } // func lVisiblOn

        /// <summary>Lua Функция. Скрываем поля в шаблоне, указанные в массиве VarId</summary>
        /// <param name="pVarId">Перечень кодов VarId</param>
        private void lVisiblOff(params int[] pVarId)
        {
            for (int i = 0; i < pVarId.Length; i++)
            {
                PUB_Pole.PROP_FormShablon.GetPole(pVarId[i]).Visibility = Visibility.Collapsed;
            }
        } // func lVisiblOff

        /// <summary>Lua Функция. Делаем обязательными поля, указанные в массиве VarId</summary>
        /// <param name="pVarId">Перечень кодов VarId</param>
        private void lNecesOn(params int[] pVarId)
        {
            for (int i = 0; i < pVarId.Length; i++)
            {
                PUB_Pole.PROP_FormShablon.GetPole(pVarId[i]).PROP_Necessarily = true;
            }
        } // func lNecesOn

        /// <summary>Lua Функция. Делаем НЕобязательными поля, указанные в массиве VarId</summary>
        /// <param name="pVarId">Перечень кодов VarId</param>
        private void lNecesOff(params int[] pVarId)
        {
            for (int i = 0; i < pVarId.Length; i++)
            {
                PUB_Pole.PROP_FormShablon.GetPole(pVarId[i]).PROP_Necessarily = false;
            }
        } // func lNecesOff

        /// <summary>Lua Функция.  Отчистка текста полей, указанные в массиве VarId</summary>
        /// <param name="pVarId">Перечень кодов VarId</param>
        private void lTextClear(params int[] pVarId)
        {
            for (int i = 0; i < pVarId.Length; i++)
            {
                PUB_Pole.PROP_FormShablon.GetPole(pVarId[i]).PROP_Text = "";
            }
        } // func lTextClear

        //private bool lFocusPole(int pVarId)
        //{
        //    Keyboard.ClearFocus();
        //    bool _result = ((UserPole_Text)PUB_Pole.PROP_FormShablon.GetPole(pVarId)).PART_TextBox.Focus();

        //    return _result;
        //} // func lPole

        /// <summary>Lua Функция. Новый протокол (True), старый протокол (False)</summary>
        private bool lNew()
        {
            return PUB_Pole.PROP_FormShablon.PROP_NewProtokol;
        } // func lNew

        /// <summary>Lua Функция. Выдает значение полей из APSTAC (ast), APAC (apa), Kbol (kbol - по умолчанию), Последний диагноз из поликлиники/стационара(LastDiag)</summary>
        /// <param name="pPole">Имя поля</param>
        /// <param name="pTable">Имя таблицы (по умолчанию "kbol")</param>
        private static object lRead(string pPole, string pTable = "kbol")
        {
            object _Value = null;
            switch (pTable)
            {
                case "ast":
                    _Value = MyGlo.HashAPSTAC[pPole];
                    break;
                case "apa":
                    _Value = MyGlo.HashAPAC[pPole];
                    break;
                case "kbol":
                    _Value = MyGlo.HashKBOL[pPole];
                    break;
                case "LastDiag":
                    _Value = MyGlo.HashLastDiag[pPole];
                    break;
            }
            return _Value ?? "";
        } // func lRead
        #endregion

        /// <summary>МЕТОД Запуск кусков Lua</summary>
        public void MET_StartLua()
        {
            // Смотрим какие функции есть в куске
            PROP_OnCreat = PROP_ChankText.IndexOf("function OnCreat()") > -1;
            PROP_OnChange = PROP_ChankText.IndexOf("function OnChange()") > -1;
            PROP_OnBeforeSave = PROP_ChankText.IndexOf("function OnBeforeSave()") > -1;
            PROP_OnSave = PROP_ChankText.IndexOf("function OnSave()") > -1;
            try
            {
                PRI_Env.dochunk(PROP_ChankText); // execute the chunk
            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка формирования куска Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
            }
        }

        /// <summary>МЕТОД Тестирование кусков Lua</summary>
        /// <param name="pChunk">Код куска Lua</param>
        public void MET_TestLua(string pChunk)
        {
            PRI_Lua.CompileChunk(pChunk, new LuaCompileOptions {DebugEngine = new LuaStackTraceDebugger()});
            try
            {
                PRI_Lua.CompileChunk("return b * 2", new LuaCompileOptions(), new KeyValuePair<string, Type>("b", typeof(int)));

            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка формирования куска Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
            }
        }

        /// <summary>МЕТОД Запускаем Событие OnCreat, после создания формы</summary>
        public void MET_OnCreat()
        {
            if (!PROP_OnCreat) return;
            try
            {
                PRI_Env.OnCreat();
            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
            }
        }

        /// <summary>МЕТОД Запускаем Событие OnChange, после обновления данных поля</summary>
        public void MET_OnChange()
        {
            if (!PROP_OnChange) return;
            try
            {
                PRI_Env.OnChange();
            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
            }
        }

        /// <summary>МЕТОД Запускаем Событие OnBeforeSave, перед cохранением формы</summary>
        public bool MET_OnBeforeSave()
        {
            if (!PROP_OnBeforeSave) return true;
            try
            {
                LuaResult _LuaResult = PRI_Env.OnBeforeSave();
                return (bool) _LuaResult[0];
            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
                return true;
            }
        }

        /// <summary>МЕТОД Запускаем Событие OnSave, после cохранения формы</summary>
        public void MET_OnSave()
        {
            if (!PROP_OnSave) return;
            try
            {
                PRI_Env.OnSave();
            }
            catch (Exception e)
            {
                MyGlo.PUB_Logger.Error(e, $"Ошибка Lua в поле с VarId {PUB_Pole.PROP_VarId}," +
                                          $" шаблона {PUB_Pole.PROP_FormShablon.PROP_Docum.PROP_ListShablon.PROP_Cod}," +
                                          $"\n таблицы {PUB_Pole.PROP_FormShablon.PROP_TipProtokol.PROP_Shablon}");
            }
        }
    }
}
