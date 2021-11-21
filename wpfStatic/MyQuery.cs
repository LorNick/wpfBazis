using System;
using System.Globalization;

namespace wpfStatic
{
    /// <summary>КЛАСС для вывода SQL Запросов</summary>
    /// <remarks>Файл MyQuery
    /// <para>Класс используется только для запросов SQL</para></remarks>
    public static class MyQuery
    {
        #region ---- Списки шаблонов (ListShablon) ----
        /// <summary>Проверка, есть ли заполненые шаблоны с типом 1 apaNListShablon </summary>
        public static string MET_ListShablon_Select_1()
        {
            string _Query = $@"
                select NumShablon
                from dbo.apaNProtokol       as p
                join dbo.apaNListShablon    as s
                  on NumShablon = s.Cod
                where isnull(s.xDelete, 0) = 0
                    and p.CodApstac = {MyGlo.IND}
                    and s.TipObsled = 1";
             return _Query;
        }

        /// <summary>Выборка шаблонов ListShablon по условию</summary>
        public static string MET_ListShablon_Select_2(string pSqlWhere, string pPrefix, string pxFormat = "")
        {
            string _Query = $@"
                select concat(Cod, ';', Name) as Filter, NameKr, TipObsled, ProfilVr, Icon, Cod, [Name]
                from dbo.{pPrefix}ListShablon
                where isnull(xDelete, 0) = 0
                    and xFormat not like '%\Del%'     -- закрытые шаблоны не отображаем
                    and xFormat like '%{pxFormat}%'   -- показываем шаблоны только с указанным форматом (если есть)
                {pSqlWhere}";
            return _Query;
        }

        /// <summary>Выборка строки шаблона из списка шаблонов ListShablon по коду</summary>
        public static string MET_ListShablon_Select_3(int Cod, string pPrefix)
        {
            string _Query =$@"
                select *
                from dbo.{pPrefix}ListShablon
                where isnull(xDelete, 0) = 0 and Cod = {Cod}";
             return _Query;
        }

        /// <summary>Выборка ВСЕХ строк шаблона ListShablon из заданного списка по Cod</summary>
        public static string MET_ListShablon_Select_4(string pPrefix, string pShablons)
        {
            string _Query = $@"
                select *
                from dbo.{pPrefix}ListShablon
                where isnull(xDelete, 0) = 0 and Cod in ({pShablons})";
             return _Query;
        }

        /// <summary>Выборка ВСЕХ строк шаблона ListShablon</summary>
        public static string MET_ListShablon_Select_5(string pPrefix, DateTime pDateN, DateTime pDateK, string pUserCod, bool? pZeroProtokol)
        {
            string _UserCod = "";
            // Фильтр по UserCod
            if (pUserCod != "")
                _UserCod = $" and xUserUp = {MyMet.MET_ParseInt(pUserCod)}";

            string _ZeroProtokol = "";
            if (pZeroProtokol == true)
                _ZeroProtokol = $" and w.cou > 0";

            string _Query = $@"
                select concat(Cod, ';', Name) as Names
                        ,Icon as Icon
                        ,Cod, TipObsled
                        ,iif(charindex('\Del', s.xFormat) > 0, 'mnDelete', '') as IconDel
                        ,Name, ProfilVr
                        ,isnull(w.cou,0) as Cou
                        ,cast(w.Dmin as date) as Dmin
                        ,cast(w.Dmax as date) as Dmax
                from dbo.{pPrefix}ListShablon as s
                left join (select p.Numshablon, count(*) as cou, min(pDate) as Dmin,  max(pDate) as Dmax
                           from dbo.{pPrefix}Protokol as p
                           where p.xDelete = 0 and p.pDate between '{pDateN:MM.dd.yyyy}' and '{pDateK:MM.dd.yyyy}' {_UserCod}
                           group by p.NumShablon) as w
                  on s.cod=w.NumShablon
                where isnull(s.xDelete, 0) = 0 {_ZeroProtokol}
                order by Cod";
            return _Query;
        }
        #endregion

        #region  ---- Шаблоны (Shablon) ----
        /// <summary>Выборка шаблона Shablon по коду шаблона и имени таблицы</summary>
        public static string MET_Shablon_Select_1(int ID, string pTable)
        {
             string _Query = $@"
                select Cod
                      ,ID
                      ,Nomer
                      ,VarId
                      ,Maska
                      ,Type
                      ,Razdel
                      ,Name
                      ,isnull(ValueStart, '') as ValueStart
                      ,OutText
                      ,InText
                      ,xFormat
                      ,isnull(xLua, '') as xLua
                      ,isnull(xInfo, '') as xInfo
                from dbo.{pTable}
                where ID = {ID}
                order by Nomer";
             return _Query;
        }

        /// <summary>Выборка раздела шаблона Shablon по коду шаблона и наименованию раздела ( или коду вопроса или имени вопроса) и имени таблицы</summary>
        public static string MET_Shablon_Select_2(int ID, string pWhere, string pTable)
        {
            string _Query = $@"
                select Cod
                      ,ID
                      ,Nomer
                      ,VarId
                      ,Maska
                      ,Type
                      ,Razdel
                      ,Name
                      ,isnull(ValueStart, '') as ValueStart
                      ,OutText
                      ,InText
                      ,xFormat
                      ,isnull(xLua, '') as xLua
                      ,isnull(xInfo, '') as xInfo
                from dbo.{pTable}
                where ID = {ID} and {pWhere}
                order by Nomer";
             return _Query;
        }

        /// <summary>Выборка шаблона Shablon по коду шаблона и префиксу</summary>
        public static string MET_Shablon_Select_3(int ID, string pPrefix)
        {
            string _Query = $@"
                select Cod
                      ,ID
                      ,Nomer
                      ,VarId
                      ,Maska
                      ,Type
                      ,Razdel
                      ,Name
                      ,isnull(ValueStart, '') as ValueStart
                      ,isnull(OutText, '') as OutText
                      ,isnull(InText, '') as InText
                      ,isnull(xFormat, '') as xFormat
                      ,isnull(xLua, '') as xLua
                      ,isnull(xInfo, '') as xInfo
                from dbo.{pPrefix}Shablon
                where ID = {ID}
                order by Nomer";
            return _Query;
        }

        /// <summary>Выборка ВСЕХ строк шаблона Shablon из заданного списка</summary>
        public static string MET_Shablon_Select_4(string pPrefix, string pShablons)
        {
            string _Query = $@"
                select Cod
                      ,ID
                      ,Nomer
                      ,VarId
                      ,Maska
                      ,Type
                      ,Razdel
                      ,Name
                      ,isnull(ValueStart, '') as ValueStart
                      ,isnull(OutText, '') as OutText
                      ,isnull(InText, '') as InText
                      ,isnull(xFormat, '') as xFormat
                      ,isnull(xLua, '') as xLua
                      ,isnull(xInfo, '') as xInfo
                from dbo.{pPrefix}Shablon
                where ID in ({pShablons})
                order by Nomer";
            return _Query;
        }

        /// <summary>Изменение шаблона Protokol Shablon по коду</summary>
        public static string MET_Shablon_Update_1(string pTable, int Cod, int ID, byte Nomer, int VarId,
                                                  string Maska, byte Type, string Razdel,
                                                  string Name, string ValueStart, string OutText,
                                                  string InText, string xFormat, string xLua, string xInfo)
        {
            string _Query = $@"
                update dbo.{pTable}
                    set ID = {ID}
                       ,Nomer = {Nomer}
                       ,VarId = {VarId}
                       ,Maska = '{Maska}'
                       ,Type = {Type}
                       ,Razdel = '{Razdel}'
                       ,Name = '{Name}'
                       ,ValueStart = '{ValueStart}'
                       ,OutText = '{OutText}'
                       ,InText = '{InText}'
                       ,xFormat = '{xFormat}'
                       ,xLua = '{xLua.Replace("'", "''")}'
                       ,xInfo = '{xInfo}'
                where Cod = {Cod}";
            return _Query;
        }

        /// <summary>Удаляем шаблон из SQL по типу и коду</summary>
        public static string MET_Shablon_Delete_1(int ID, string pPrefix)
        {
            string _Query = $@"
                delete dbo.{pPrefix}Shablon
                where ID = {ID}";
            return _Query;
        }

        /// <summary>Изменение шаблона Protokol Shablon по коду</summary>
        public static string MET_Shablon_Insert_1(string pTable, int Cod, int ID, byte Nomer, int VarId,
                                                  string Maska, byte Type, string Razdel,
                                                  string Name, string ValueStart, string OutText,
                                                  string InText, string xFormat, string xLua, string xInfo)
        {
            string _Query = $@"
                insert Bazis.dbo.{pTable}
                    (Cod, ID, Nomer, VarId, Maska, Type, Razdel, Name, ValueStart, OutText, InText, xFormat, xLua, xInfo)
                values
                    ({Cod}, {ID}, {Nomer}, {VarId}, '{Maska}', {Type}, '{Razdel}', '{Name}', '{ValueStart}', '{OutText}', '{InText}', '{xFormat}', '{xLua.Replace("'", "''")}', '{xInfo}')";
            return _Query;
        }

        /// <summary>Копируем шаблон на филиал</summary>
        public static string MET_Shablon_InsertToFilial_1(int ID, string pPrefix)
        {
            string _Query = $@"
                delete Fil.Bazis.dbo.{pPrefix}Shablon
                where ID = {ID};

                insert Fil.Bazis.dbo.{pPrefix}Shablon
                select *
                from Bazis.dbo.{pPrefix}Shablon
                where ID = {ID}";
            return _Query;
        }
        #endregion

        #region ---- Протоколы (Protokol) ----
        /// <summary>Выборка протоколов Protokol для переменной Docum по типу обследования</summary>
        public static string MET_Protokol_Select_1(decimal KL, decimal CodApstac, int TipObsled, string pPrefix)
        {
            string _Query = $@"
                select p.Protokol, p.NumShablon, p.pDate
                from dbo.{pPrefix}Protokol as p
                left join dbo.{pPrefix}ListShablon as l
                    on p.NumShablon = l.Cod
                where isnull(p.xDelete, 0) = 0 and p.KL = {KL} and p.CodApstac = {CodApstac} and l.TipObsled = {TipObsled}
                order by p.pDate";
             return _Query;
        }

        /// <summary>Выборка протокола Protokol для переменной Docum по коду шаблона</summary>
        public static string MET_Protokol_Select_2(decimal KL, decimal CodApstac, int NumShablon, string pPrefix)
        {
            string _Query = $@"
                select Protokol, NumShablon, pDate
                from dbo.{pPrefix}Protokol
                where isnull(xDelete, 0) = 0 and KL = {KL} and CodApstac = {CodApstac} and NumShablon = {NumShablon}";
             return _Query;
        }

        /// <summary>Выборка протоколов Protokol для переменной Docum по типу обследования (всех протоволов этого пациента)</summary>
        public static string MET_Protokol_Select_3(decimal KL, int TipObsled, string pPrefix)
        {
            string _Query = $@"
                select top(1) p.Protokol, p.NumShablon, p.pDate
                from dbo.{pPrefix}Protokol            as p
                left join dbo.{pPrefix}ListShablon    as l
                  on p.NumShablon = l.Cod
                where isnull(p.xDelete, 0) = 0 and p.KL = {KL} and l.TipObsled = {TipObsled}
                order by p.pDate desc, p.Cod desc";
             return _Query;
        }

        /// <summary>Выборка списка протоколов Protokol</summary>
        public static string MET_Protokol_Select_4(string pPrefix, int pNumSha, DateTime pDateN, DateTime pDateK, string pUserCod)
        {
            string _SqlWhere = "";
            // Фильтр по UserCod
            if (pUserCod != "")
                _SqlWhere += $" and p.xUserUp = {MyMet.MET_ParseInt(pUserCod)}";

            string _Query = $@"
                use Bazis;
                select top 100
                    p.KL
                    ,p.CodApstac
                    ,iif(p.xDelete = 1, 'mnDelete', '') as Icon
                    ,dbo.GetFIO(k.FAM, k.I, k.O) as FIO
                    ,k.DR
                    ,p.pDate
                    ,p.xUserUp
                    ,u.FIO as Users
                from dbo.{pPrefix}Protokol  as p with (nolock)
                left join dbo.kbol          as k with (nolock) on p.KL = k.KL
                left join dbo.s_Users       as u on p.xUserUp = u.Cod
                where NumShablon = {pNumSha}
                    and p.pDate between '{pDateN:MM.dd.yyyy}' and '{pDateK:MM.dd.yyyy}' {_SqlWhere}
                order by pDate desc";
             return _Query;
        }

        /// <summary>Выборка протокола Protokol по 4м параметрам</summary>
        public static string MET_Protokol_Select_5(decimal CodApstac, int NumShablon, int pIndex, string pPrefix)
        {
            string _Query = $@"
                select p.Cod
                    ,cast(p.CodApstac as decimal) as CodApstac
                    ,p.NumShablon
                    ,p.Protokol
                    ,p.KL
                    ,p.xDateUp
                    ,p.xUserUp
                    ,p.xImport
                    ,p.pIndex
                    ,isnull(p.xDelete, 0) as xDelete
                    ,p.pDate
                    ,p.xLog
                    ,p.xInfo
                    ,u.FIO      as UserName
                from dbo.{pPrefix}Protokol as p
                left join dbo.s_Users as u
                  on p.xUserUp = u.Cod
                where p.CodApstac = {CodApstac} and p.NumShablon = {NumShablon} and p.pIndex = {pIndex}";
            return _Query;
        }

        /// <summary>Выборка протокола Protokol по 3м параметрам</summary>
        public static string MET_Protokol_Select_6(decimal CodApstac, int NumShablon, string pPrefix)
        {
             string _Query = $@"
                select top(1) *
                from dbo.{pPrefix}Protokol
                where isnull(xDelete, 0) = 0 and CodApstac = {CodApstac} and NumShablon = {NumShablon}
                order by pDate desc, Cod desc";
             return _Query;
        }

        /// <summary>Выборка последнего протокола Protokol по KL и номеру шаблона</summary>
        public static string MET_Protokol_Select_8(decimal KL, int NumShablon, string pPrefix)
        {
             string _Query = $@"
                select top(1) *
                from dbo.{pPrefix}Protokol
                where isnull(xDelete, 0) = 0 and KL = {KL} and NumShablon = {NumShablon}
                order by pDate desc, Cod desc";
             return _Query;
        }

        /// <summary>Выборка протокола Protokol по коду, для History</summary>
        public static string MET_Protokol_Select_9(int Cod, string pPrefix)
        {
             string _Query = $@"
                select p.Cod
                    ,cast(p.CodApstac as decimal) as CodApstac                    -- в поликлинике стоит int (для DataReader нужен точный тип)
                    ,p.NumShablon
                    ,p.Protokol
                    ,p.KL
                    ,p.xDateUp
                    ,p.xUserUp
                    ,p.xImport
                    ,isnull(p.xDelete, 0) as xDelete
                    ,p.pIndex
                    ,p.pDate
                    ,p.xLog
                    ,p.xInfo
                    ,u.FIO      as UserName
                from dbo.{pPrefix}Protokol as p
                left join dbo.s_Users as u
                  on p.xUserUp = u.Cod
                where p.Cod = {Cod}";
             return _Query;
        }

        /// <summary>Количество протоколов Protokol по 2м параметрам</summary>
        public static string MET_Protokol_Select_11(decimal CodApstac, int NumShablon, string pPrefix)
        {
             string _Query = $@"
                select Cod
                from dbo.{pPrefix}Protokol
                where isnull(xDelete, 0) = 0 and CodApstac = {CodApstac} and NumShablon = {NumShablon}";
             return _Query;
        }

        /// <summary>Выборка ВСЕХ протоколов Protokol по коду посещения/стационара если pTipFind = "CodApstac" или по KL если pTipFind = "KL"</summary>
        public static string MET_Protokol_Select_12(decimal CodApstac, string pPrefix, string pTipFind)
        {
            string _Query = $@"
                select p.Cod
                    ,cast(p.CodApstac as decimal) as CodApstac                    -- в поликлинике стоит int (для DataReader нужен точный тип)
                    ,p.NumShablon
                    ,p.Protokol
                    ,p.KL
                    ,isnull(p.xDateUp, p.pDate) as xDateUp
                    ,p.xUserUp
                    ,p.xImport
                    ,isnull(p.xDelete, 0) as xDelete
                    ,p.pIndex
                    ,p.pDate
                    ,p.xLog
                    ,p.xInfo
                    ,u.FIO      as UserName
                from dbo.{pPrefix}Protokol as p
                left join dbo.s_Users as u          on p.xUserUp = u.Cod
                where p.pDate is not null and p.{pTipFind} = {CodApstac}";
            return _Query;
        }

        /// <summary>Наличие протокола astProtokol с TipObsled = 1 текущего обследования</summary>
        public static string MET_parProtokol_Select_2(decimal CodApstac)
        {
             string _Query = $@"
                select p.Cod
                from dbo.parProtokol as p
                left join dbo.parListShablon as l
                    on p.NumShablon = l.Cod
                where isnull(p.xDelete, 0) = 0 and p.CodApstac = {CodApstac} and l.TipObsled = 1
                order by p.pDate";
             return _Query;
        }

        /// <summary>Изменение протокола Protokol по 7м параметрам</summary>
        public static string MET_Protokol_Update_1(int pCod, string Protokol, DateTime xDateUp, int xUserUp, DateTime pDate, string pPrefix, string pxLog)
        {
             string _Query = $@"
                update dbo.{pPrefix}Protokol
                    set Protokol = '{Protokol}'
                       ,xDateUp = '{xDateUp:MM.dd.yyyy}'
                       ,xUserUp = {xUserUp}
                       ,pDate = '{pDate:MM.dd.yyyy}'
                       ,xLog = '{pxLog}'
                where Cod = {pCod}";
             return _Query;
        }

        /// <summary>Удаляем/Востанавливаем протоколы Protokol по 6м параметрам</summary>
        public static string MET_Protokol_Update_2(int pCod, int pxDelete, DateTime xDateUp, int xUserUp, string pPrefix, string pxLog)
        {
            string _Query = $@"
                update dbo.{pPrefix}Protokol
                    set xDelete = {pxDelete}
                       ,xDateUp = '{xDateUp:MM.dd.yyyy}'
                       ,xUserUp = {xUserUp}
                       ,xLog = '{pxLog}'
                where Cod = {pCod}";

            return _Query;
        }

        /// <summary>Удаляем/Востанавливаем протоколы Protokol по 7м параметрам</summary>
        public static string MET_Protokol_Update_3(int pCod, string Protokol, int pxDelete, DateTime xDateUp, int xUserUp, string pPrefix, string pxLog)
        {
            string _Query = $@"
                update dbo.{pPrefix}Protokol
                    set Protokol = '{Protokol}'
                       ,xDelete = {pxDelete}
                       ,xDateUp = '{xDateUp:MM.dd.yyyy}'
                       ,xUserUp = {xUserUp}
                       ,xLog = '{pxLog}'
                where Cod = {pCod}";

            return _Query;
        }

        /// <summary>Добавление протокола Protokol по 9м параметрам</summary>
        public static string MET_Protokol_Insert_1(int Cod, decimal CodApstac, int NumShablon, string Protokol,
                                                  decimal KL, DateTime xDateUp, int xUserUp, int pIndex, DateTime pDate, string pPrefix,
                                                  string xLog = "", string xInfo = "NULL")
        {
            string _Query = $@"
                insert into dbo.{pPrefix}Protokol
                    (Cod
                    ,CodApstac
                    ,NumShablon
                    ,Protokol
                    ,KL
                    ,xDateUp
                    ,xUserUp
                    ,xDelete
                    ,pIndex
                    ,pDate
                    ,xLog
                    ,xInfo)
                 values({Cod}, {CodApstac}, {NumShablon}, '{Protokol}', {KL}, '{xDateUp:MM.dd.yyyy}', {xUserUp}, 0, {pIndex}, '{pDate:MM.dd.yyyy}', '{xLog}', {xInfo})";
             return _Query;
        }
        #endregion

        #region ---- APAC (поликлиника) ----
        /// <summary>Выборка посещения поликлиники APAC</summary>
        public static string APAC_Select_1(decimal Cod)
        {
             string _Query = $@"
                select *
                from dbo.APAC
                where Cod = {Cod}";
             return _Query;
        }

        /// <summary>Выборка посещений для врача поликлиники</summary>
        public static string APAC_Select_2(string proSqlWhere)
        {
             string _Query = $@"
               select top(200)
                        a.Cod
                        ,k.KL
                        ,dbo.GetFIO(k.FAM, I, O) as FIO
                        ,k.DR
                        ,a.DP
                        ,v.TKOD as Vr
                from dbo.kbol           as k
                join dbo.APAC           as a
                  on  a.KL = k.KL
                join dbo.s_VrachPol     as v
                  on v.KOD = a.KV
                {proSqlWhere}
                    and isnull(a.xDelete, 0) = 0
                order by k.FAM";
             return _Query;
        }

        /// <summary>Считаем время госпитализации для Направлений (для Variables)!!!!</summary>
        /// <param name="pKorpus">Корпус стационара</param>
        /// <param name="pDate">Дата направления в стационар</param>
        public static string APAC_Select_3(int pKorpus, DateTime pDate)
        {
            string _Query = $@"
                -- C 8:30 прибавляем по 5 минут
                -- Если в очереди больше 84 человек, то начинаем отсчет сначала с 8:30 ( до 15:30)
                use Bazis;
                select convert(varchar(5), dateadd(mi, 5 * iif(count(*) > 84, count(*) - 84, count(*)), datetimefromparts(2017, 1, 1, 8, 30, 0, 0)), 108) as times
                      ,count(*) as cou
                from (
                    select KL, dbo.GetPole(4, p.Protokol) as d, try_cast(left(dbo.GetPole(3, p.Protokol), 2) as real) as otd
                    from dbo.apaNProtokol as p
                    where p.NumShablon = 9906 and contains(p.Protokol, '\4#{pDate:dd.MM.yyyy}')
                    union
                    select KL, dbo.GetPole(4, p.Protokol) as d, try_cast(left(dbo.GetPole(3, p.Protokol), 2) as real) as otd
                    from dbo.astProtokol as p
                    where p.NumShablon = 8014 and contains(p.Protokol, '\4#{pDate:dd.MM.yyyy}')
                    ) as p
                join dbo.s_Department as d
                  on p.otd = d.Cod
                where d.Korpus = {pKorpus} and p.otd <> 15";
            return _Query;
        }
        #endregion

        #region ---- APSTAC (стационар) ----
        /// <summary>Выборка записи APSTAC по коду</summary>
        public static string APSTAC_Select_1(decimal IND)
        {
             string _Query = $@"
                select a.*
                      ,'Number' = isnull(p.Number, 0)   -- номер палаты
                from dbo.APSTAC           as a
                left join dbo.astPalat    as p
                  on a.astPalatCod = p.Cod
                where IND = {IND}";
             return _Query;
        }

        /// <summary>Выборка приемное отделение APSTAC по коду </summary>
        public static string APSTAC_Select_2(decimal IND)
        {
              string _Query = $@"
                select  a.*
                        , k.DSmerti
                        ,'User'        = u.FIO
                from dbo.APSTAC             as a
                left join dbo.kbol          as k
                  on a.KL = k.KL
                left join dbo.s_Users       as u
                  on u.Cod = cast(right(cast(k.KL as varchar), 3) as int)
                where a.IND = {IND}";
              return _Query;
        }

        /// <summary>Выборка списка APSTAC по условию</summary>
        public static string APSTAC_Select_3(string proSqlWhere)
        {
             string _Query = $@"
                select top(200)
                     a.IND
                    ,k.KL
                    ,a.NSTAC
                    ,dbo.GetFIO(k.FAM, I, O) as FIO
                    ,k.DR
                    ,'Diag' = a.D
                    ,a.DN
                    ,a.DK
                    ,v.TKOD
                    ,'Number' = isnull(p.Number, 0)
                    ,case
                        when UET3 = 0 then datediff(d, a.DN, GETDATE())
                        else UET3
                     end
                    ,a.Otd
                from dbo.APSTAC             as a
                left join dbo.kbol          as k
                  on a.KL = k.KL
                left join dbo.s_VrachStac   as v
                  on a.KV = v.KOD
                left join dbo.astPalat      as p
                  on a.astPalatCod = p.Cod
                {proSqlWhere}";
             return _Query;
        }

        /// <summary>Выборка списка APSTAC по условию для Канцер регистра</summary>
        public static string APSTAC_Select_4(string proSqlWhere)
        {
             string _Query = $@"
                select top(200)
                     a.IND
                    ,k.KL
                    ,a.NSTAC
                    ,dbo.GetFIO(k.FAM, I, O) as FIO
                    ,'DR' = convert(nvarchar(10),k.DR, 104)
                    ,'Diag' = a.D
                    ,'DN' = convert(nvarchar(10),a.DN, 104)
                    ,'DK' = convert(nvarchar(10),a.DK, 104)
                    ,a.Otd
                    ,'Karta' = case
                                 when p.Cod is NULL then ''
                                 else 'Заполнена'
                               end
                    ,'DZap' = convert(nvarchar(10),a.OtpuskDate, 104)
                from dbo.APSTAC             as a
                left join dbo.kbol          as k
                  on a.KL = k.KL
                left join dbo.astProtokol   as p
                  on a.IND = p.CodApstac and p.NumShablon = 9922
                {proSqlWhere}";
             return _Query;
        }

        /// <summary>Находим код Отделения APSTAC по коду </summary>
        public static string APSTAC_Select_5(decimal IND)
        {
             string _Query = $@"
                select  Otd
                from dbo.APSTAC
                where IND = {IND}";
             return _Query;
        }
        #endregion

        #region ---- History (история болезни) ----
        /// <summary>Выборка истории болезни History</summary>
        public static string MET_History_Select_1(decimal KL)
        {
             string _Query = $@"
                select 1        as Nom
                    ,a.Cod
                    ,a.Kv
                    ,v.TKOD     as Vr
                    ,a.Dp
                    ,0          as Dk
                    ,a.D
                    ,p.Name     as Profil
                    ,(select count(p.Cod) from dbo.apaNProtokol as p where a.Cod = p.CodApstac and isnull(p.xDelete, 0) = 0) as pCount
                    ,a.Cod      as CodApstac
                    ,(select  isnull(max(dbo.GetPole(200, k.Protokol)), '') --count(p.Cod)
                      from dbo.apaNProtokol as p
                      join dbo.kdlProtokol as k
                        on k.pIndex = 1 and p.NumShablon = k.NumShablon and p.Cod = k.CodApstac and isnull(k.xDelete, 0) = 0 and k.pDate is not NULL
                      where p.NumShablon >= 20000 and a.Cod = p.CodApstac and isnull(p.xDelete, 0) = 0
                     ) as kdl
                    ,iif(a.IsCloseTap = 2, iif(a.Cod = a.NumberFirstTap, '(разовое)', '(обращение)'), '') as Metka
                    ,(select top 1 'консилиум'
                      from dbo.apaNProtokol as p
                       where a.Cod = p.CodApstac and p.NumShablon%100 = 11 and p.NumShablon < 3000 and  isnull(p.xDelete, 0) = 0
                     ) as ImageInform
                    ,isnull(a.xDelete, 0) as xDelete
                    ,a.xLog
                from dbo.APAC               as a
                left join dbo.s_VrachPol    as v    on a.Kv = v.KOD
                left join dbo.s_ProfPol     as p    on v.SPCS = p.Cod
                where a.KL = {KL}
                union
                select 2        as Nom
                    ,a.IND      as Cod
                    ,a.Kv
                    ,v.TKOD     as Vr
                    ,a.Dn       as Dp
                    ,a.Dk
                    ,a.D
                    ,o.Names    as Profil
                    ,(select count(p.Cod) from dbo.astProtokol as p where a.IND = p.CodApstac and isnull(p.xDelete, 0) = 0) as pCount
                    ,a.IND      as CodApstac
                    ,(select  isnull(max(dbo.GetPole(200, k.Protokol)), '') --count(p.Cod)
                      from dbo.astProtokol as p
                      join dbo.kdlProtokol as k
                        on k.pIndex = 2 and p.NumShablon = k.NumShablon and p.Cod = k.CodApstac and isnull(k.xDelete, 0) = 0 and k.pDate is not NULL
                      where p.NumShablon >= 20000 and a.IND = p.CodApstac and isnull(p.xDelete, 0) = 0
                     ) as kdl
                    ,'' as Metka
                    ,iif(o.Tip = 1, 'mnStac', 'mnStacDnev') as ImageInform
                    ,isnull(a.xDelete, 0) as xDelete
                    ,a.xLog
                from dbo.APSTAC             as a
                left join dbo.s_VrachStac   as v    on a.Kv = v.KOD
                left join dbo.s_Department  as o    on a.Otd = o.Cod
                where a.KL = {KL}
                union
                select 3        as Nom
                    ,p.Cod
                    ,p.xUserUp  as Kv
                    ,u.FIO      as Vr
                    ,p.pDate    as Dp
                    ,0          as Dk
                    ,''         as D
                    ,l.NameKr   as Profil
                    ,p.NumShablon as pCount
                    ,p.CodApstac
                    ,isnull(cast(k.Cod as nvarchar), '') as kdl
                    ,'' as Metka
                    ,'' as ImageInform
                    ,isnull(p.xDelete, 0) as xDelete
                    ,null as xLog
                from dbo.parProtokol            as p
                left join dbo.parListShablon    as l    on p.NumShablon = l.Cod
                left join dbo.s_Users           as u    on p.xUserUp = u.Cod
                left join dbo.kdlProtokol       as k    on k.pIndex = 3 and p.NumShablon = k.NumShablon and p.Cod = k.CodApstac and isnull(k.xDelete, 0) = 0 and k.pDate is not NULL
                where p.KL = {KL}
                order by Dp desc, Nom desc";
             return _Query;
        }

        /// <summary>Выборка протоколов Стационара History</summary>
        public static string MET_History_Select_2(decimal CodApstac)
        {
             string _Query = $@"
                select p.Cod, p.NumShablon, p.pDate, ls.NameKr, ls.Icon
                      ,iif(charindex('\DostupKR', ls.xFormat) > 0, 1, 0) as Dostup  -- если есть тег Dostup, то отображаем в истории программы КР
                      ,isnull(cast(k.Cod as nvarchar), '') as kdl
                      ,isnull(p.xDelete, 0) as xDelete
                from dbo.astProtokol            as p
                left join dbo.astListShablon    as ls   on p.NumShablon = ls.Cod
                left join dbo.s_Users           as u    on p.xUserUp = u.Cod
                left join dbo.kdlProtokol       as k    on k.pIndex = 2 and p.NumShablon = k.NumShablon and p.Cod = k.CodApstac and isnull(k.xDelete, 0) = 0 and k.pDate is not NULL
                where p.CodApstac = {CodApstac}
                order by p.pDate desc, iif(ls.NameKr = 'Выписка' or ls.NameKr = 'Выписной эпикриз', p.NumShablon * 1000, p.NumShablon) desc";
             return _Query;
        }

        /// <summary>Выборка протоколов поликлиники History</summary>
        public static string MET_History_Select_3(decimal CodApstac)
        {
             string _Query = $@"
                select p.Cod, p.NumShablon, p.pDate, ls.NameKr, ls.Icon
                      ,iif(charindex('\DostupKR', ls.xFormat) > 0, 1, 0) as Dostup  -- если есть тег Dostup, то отображаем в истории программы КР
                      ,isnull(cast(k.Cod as nvarchar), '') as kdl
                      ,isnull(p.xDelete, 0) as xDelete
                from dbo.apaNProtokol           as p
                left join dbo.apaNListShablon   as ls   on p.NumShablon = ls.Cod
                left join dbo.s_Users           as u    on p.xUserUp = u.Cod
                left join dbo.kdlProtokol       as k    on k.pIndex = 1 and p.NumShablon = k.NumShablon and p.Cod = k.CodApstac and isnull(k.xDelete, 0) = 0 and k.pDate is not NULL
                where p.CodApstac = {CodApstac}";
             return _Query;
        }

        /// <summary>Выборка протоколов kdl History</summary>
        public static string MET_History_Select_4(int Cod)
        {
            string _Query = $@"
                select p.Cod, p.NumShablon, ls.NameKr, ls.Icon
                      ,isnull(p.pDate, p.xDateUp) as pDate
                      ,dbo.GetPole(200, p.Protokol) as Indicator
                      ,isnull(p.xDelete, 0) as xDelete
                 from dbo.kdlProtokol           as p
                left join dbo.kdlListShablon    as ls   on p.NumShablon = ls.Cod
                left join dbo.s_Users           as u    on p.xUserUp = u.Cod
                where p.Cod = cast({Cod} as int)";
            return _Query;
        }
        #endregion

        #region ---- kbol (таблица Пациентов) ----
        /// <summary>Выборка записи kbol по коду</summary>
        public static string kbol_Select_1(decimal KL)
        {
             string _Query = $@"
                select *
                       ,dbo.GetFIO(FAM, I, O) as FIO
                from dbo.kbol
                where KL = {KL}";
             return _Query;
        }

        /// <summary>Выборка записи kbol по коду, для паспортных данных</summary>
        public static string kbol_Select_2(decimal KL)
        {
             string _Query = $@"
                select k.*
                        ,'User'         = u.FIO
                        ,'NameSCom'     = sc.TKOD
                        ,'NameReg'      = sc.NameReg
                        ,'DocName'        = d.NAME
                        ,'SocStat'        = s.TKOD
                        ,'LPU'            = l.TKOD
                from dbo.kbol                    as k
                left join dbo.s_StrahComp        as sc    on sc.KOD = k.SCom
                left join dbo.s_Docum            as d    on k.Doc = d.KOD
                left join dbo.s_SocPol            as s    on k.SP = s.KOD
                left join dbo.s_LPU                as l    on k.StrLPU <> '' and k.StrLPU = l.StrLPU
                left join dbo.s_Users            as u    on u.Cod = cast(right(cast(k.KL as varchar), 3) as int)
                where k.KL = {KL}";
             return _Query;
        }

        /// <summary>Выборка записи kbol по условию</summary>
        public static string kbol_Select_3(string proSqlWhere)
        {
             string _Query = $@"
                select top 500 k.KL
                    ,k.NomAK
                    ,dbo.GetFIO(k.FAM, k.I, k.O) as FIO
                    ,k.DR
                    ,k.DSmerti
                from dbo.kbol as k
                {proSqlWhere}";
             return _Query;
        }

        /// <summary>Выборка записи kbol по условию (для онкологов ЛПУ)</summary>
        public static string kbol_Select_4(string proSqlWhere)
        {
            string _Query = $@"
                select top 500 k.KL
                       ,dbo.GetFIO(k.FAM, k.I, k.O) as FIO
                       ,k.DR
                from dbo.kbol as k
                join dbo.ASTORM as a
                  on a.FIO = k.FAM and a.FATHER = k.O and a.Name = k.I and a.DATER = k.DR
                where isnull(k.xDelete, 0) = 0
                    and a.CODE_MO = (select top 1 a.CODE_MO
                                     from dbo.kbol as k
                                     join dbo.ASTORM as a
                                       on a.FIO = k.FAM and a.FATHER = k.O and a.Name = k.I and a.DATER = k.DR
                                     where k.KL = {proSqlWhere}";
            return _Query;
        }

        /// <summary>Выборка записи kbol по условию  (для других ЛПУ)</summary>
        public static string kbol_Select_5(string proSqlWhere, int pLpu, int pOtd)
        {
            int _Modul = (int)MyGlo.TypeModul;

            string _Query = $@"
            use Bazis;

            -- Для химии
            declare @Otd as nvarchar(20) = '\6#{pOtd}';
            declare @Lpu as nvarchar(20) = '\4#{pLpu}';
            declare @ShabNaprPol as int = 9924;        -- шаблон направления из apaNShablon
            declare @ShabNaprStac as int = 9950;    -- шаблон направления из astShablon
            declare @ShabRez as int = 2;            -- шаблон результата из kdlShablon

            -- Для ККД
            if '{pLpu}' = '554505'
            begin
                set @ShabNaprPol = 9944;
                set @ShabNaprStac = 9944;
                set @ShabRez = 3;
            end

            -- Для ЦАОП (пока здесь же)
            if {(int)MyGlo.TypeModul} = 30
            begin
                set @ShabNaprPol = 9958;
                set @ShabNaprStac = 0;
                set @ShabRez = 9;
            end

            select
                k.KL
                ,dbo.GetFIO(k.FAM, k.I, k.O) as FIO
                ,k.DR
                ,u.FIO
                ,p.pDate
                ,iif(p3.pDate >= p.pDate, p3.pDate, null) as Rez        -- результат, только если выписка создана позднее последнего направления
            from dbo.kbol as k
            join (select *
                  from (
                    select row_number() over(partition by KL order by pDate desc, Cod desc) as dd, *
                    from (
                        select * -- max(Cod) as Cod            -- находим последнее направление в ЛПУ
                        from dbo.astProtokol
                        where NumShablon = @ShabNaprStac and contains(Protokol, @Otd) and contains(Protokol, @Lpu) and isnull(xDelete, 0) = 0
                        union
                        select * -- max(Cod) as Cod            -- находим последнее направление в ЛПУ
                        from dbo.apaNProtokol
                        where NumShablon = @ShabNaprPol and contains(Protokol, @Otd) and contains(Protokol, @Lpu) and isnull(xDelete, 0) = 0
                    ) as d
                ) as r
                where dd = 1
            ) as p on k.KL = p.KL
            join dbo.s_Users        as u    on p.xUserUp = u.Cod
            left join (select KL, max(Cod) as Cod        -- находим последнюю выписку
                       from dbo.kdlProtokol
                       where NumShablon = @ShabRez and isnull(xDelete, 0) = 0
                       group by KL
            ) as p2     on k.KL = p2.KL
            left join dbo.kdlProtokol as p3 on p2.Cod = p3.Cod
            {proSqlWhere}";
            return _Query;
        }

        /// <summary>Выборка записи kbol по условию (Лаборатории)</summary>
        public static string kbol_Select_6(int ShabLab, string Dat, string Fio)
        {
            string _Query = $@"
                use Bazis;

                declare @ShabLab as int = {ShabLab}; --1000;
                declare @Dat as date = {Dat}; -- = '07/13/2020'; -- дата проведения исследования
                declare @Fio as nvarchar(50) = '{Fio}'; --'АА ТЕ Т 00';

                -- Удаляем 2е, 3е, 4е пробелы между словами (ну вдруг пользователь разойдется)
                set @Fio = replace(@Fio, '  ', ' ');
                set @Fio = replace(@Fio, '  ', ' ');
                set @Fio = replace(@Fio, '  ', ' ');

                declare @F as nvarchar(50);
                declare @I as nvarchar(50) = '';
                declare @O as nvarchar(50) = '';
                declare @DR as nvarchar(50) = '';
                declare @n as int;

                -- Находим первые символы Фамилии, Имени, Отчества, и цифры даты рождения
                set @n = charindex(' ', @Fio);
                if @n = 0
                    set @F = @Fio;                              -- Если только фамилия
                else begin
                    set @F = left(@Fio, @n - 1);
                    set @Fio = stuff(@Fio, 1, @n, '')           -- Фамилия, но ещё имя есть
                    set @n = charindex(' ', @Fio);
                    if @n = 0
                        set @I = @Fio;                          -- Если только имя
                    else begin
                        set @I = left(@Fio, @n - 1);
                        set @Fio = stuff(@Fio, 1, @n, '')       -- Имя, но ещё отчество есть
                        set @n = charindex(' ', @Fio);
                        if @n = 0
                            set @O = @Fio;                      -- Если только отчество
                        else begin
                            set @O = left(@Fio, @n - 1);
                            set @Fio = stuff(@Fio, 1, @n, '')   -- Отчество, но ещё год рождения есть
                            set @n = charindex(' ', @Fio);
                            if @n = 0
                                set @DR = @Fio;                 -- Всё цифры даты рождения
                            else begin
                                set @DR = left(@Fio, @n - 1);   -- Ан нет, ещё какуют то фигню после даты рождения добавили
                            end
                        end
                    end
                end;

                select top 200
                     d.CodApstac
                    ,k.KL
                    ,dbo.GetFIO(k.FAM, k.I, k.O) as FIO
                    ,k.DR
                    ,p.Lab
                    ,p.pDate
                    ,d.DatNapr
                from dbo.kbol as k with (nolock)
                left join (
                    select r.KL, r.pDate, u.FIO as Lab
                    from (
                        select row_number() over(partition by KL order by pDate desc, Cod desc) as Num, xUserUp, pDate, KL
                        from dbo.kdlProtokol
                        where NumShablon = @ShabLab and isnull(xDelete, 0) = 0
                        ) as r
                    join dbo.s_Users as u on r.xUserUp = u.Cod
                    where Num = 1
                    ) as p on k.KL = p.KL
                join (
                    select * from (
                    select KL, pDate as DatNapr, row_number() over(partition by KL order by pDate desc, Cod desc) as Num, Cod as CodApstac
                    from (
                        select KL, pDate, Cod
                        from dbo.apaNProtokol as p
                        where p.NumShablon = 9957 and xDelete = 0
                        union
                        select KL, pDate, Cod
                        from dbo.astProtokol as p
                        where p.NumShablon = 9957 and xDelete = 0
                    )  as d ) as d
                    where Num = 1
                ) as d on d.KL = k.KL
                where not(k.DSmerti is not null and p.KL is null)                                   -- не показываем новых умерших пациентов без результата
                    and ((len(@F) = 0 and p.KL is not null and p.pDate = @Dat)                      -- если строка ФИО пустая
                        or
                        (len(@F) > 0                                                                -- если строка ФИО НЕ пустая
                        and (k.FAM like @F + '%'
                        and ((@I <> '' and k.I like @I + '%') or @I = '')
                        and ((@O <> '' and k.O like @O + '%') or @O = '')
                        and ((@DR <> '' and cast(year(k.DR) as nvarchar(4)) like '%' + @DR + '%') or @DR = ''))
                        ))";
            return _Query;
        }
        #endregion

        #region ---- kbolInfo (итоговая информация для таблиц  kbol, APAC, APSTAC) ----
        /// <summary>Выборка записи kbolInfo по типу Tab и коду записи CodZap</summary>
        public static string MET_kbolInfo_Select_1(string Tab, decimal CodZap)
        {
            string _Query = $@"
                select *
                from dbo.kbolInfo
                where Tab = '{Tab}' and CodZap = {CodZap}";
            return _Query;
        }

        /// <summary>Выборка записи kbolInfo по коду Cod</summary>
        public static string MET_kbolInfo_Select_2(int Cod)
        {
            string _Query = $@"
                select *
                from dbo.kbolInfo
                where Cod = {Cod}";
            return _Query;
        }

        /// <summary>Выборка записи kbolInfo - последний диагноз пациента по KL</summary>
        public static string MET_kbolInfo_Select_3(decimal KL)
        {
            string _Query = $@"
                select *
                from dbo.GetLastDiag({KL})";
            return _Query;
        }

        /// <summary>Добавляем записи kbolInfo</summary>
        /// <param name="Cod">Код - берем из NextRef</param>
        /// <param name="CodZap">Код записи (KL из kbol, IND из АPSTAC, Cod из APAC ...)</param>
        /// <param name="Tab">Флаг связанной таблицы (kbol, apaN, ast)</param>
        /// <param name="KL">Код пациента</param>
        /// <param name="jTag">Строка данных json</param>
        /// <param name="Oms">Признак подачи в реестр</param>
        public static string MET_kbolInfo_Insert_1(int Cod, decimal CodZap, string Tab, decimal KL, string jTag, int Oms)
        {
            string _Query = $@"
                insert dbo.kbolInfo
                    (Cod
                    ,CodZap
                    ,Tab
                    ,KL
                    ,jTag
                    ,Oms)
                values({Cod}, {CodZap}, '{Tab}', {KL}, '{jTag}', {Oms})";
            return _Query;
        }

        /// <summary>Меняем записи kbolInfo</summary>
        /// <param name="Cod">Код - берем из NextRef</param>
        /// <param name="jTag">Строка данных json</param>
        /// <param name="Oms">Признак подачи в реестр</param>
        public static string MET_kbolInfo_Update_1(int Cod, string jTag, int Oms)
        {
            string _Query = $@"
                update dbo.kbolInfo
                    set jTag = '{jTag}'
                        ,Oms = {Oms}
                where Cod = {Cod}";
            return _Query;
        }
        #endregion

        #region ---- List (списки apaNList, astList, parList, kdlList, s_ListDocum) ----
        /// <summary>Варианты ответа 3м параметрам</summary>
        public static string MET_List_Select_1(string TabName, int ID, int Nomer)
        {
            string _Query = $@"
                select Cod, Value
                from dbo.{TabName}
                where ID = {ID} and Nomer = {Nomer}";
            return _Query;
        }

        /// <summary>Находим максимальный код</summary>
        public static string MET_List_MaxCod_Select_2(string TabName, int ID)
        {
            string _Query = $@"
                select max(Cod)
                from dbo.{TabName}
                where ID = {ID}";
            return _Query;
        }

        /// <summary>Ищем повторы по 4м параметрам</summary>
        public static string MET_List_Select_3(string TabName, int ID, int Nomer, string Value)
        {
            string _Query = $@"
                select Cod
                from dbo.{TabName}
                where ID = {ID} and Nomer = {Nomer} and Value = '{Value}'";
            return _Query;
        }

        /// <summary>Выборка списка по 3м параметрам</summary>
        /// <param name="TabName">Имя таблицы List</param>
        /// <param name="ID">Код шаблона</param>
        /// <param name="VarId">VarId вопроса</param>
        /// <param name="pSortList">Сортировка
        ///         true - сортируем по порядку расположению в таблице List по полю Cod,
        ///         false - сортируем по алфавиту по полю Value (по умолчанию)</param>
        public static string MET_List_Select_4(string TabName, int ID, int VarId, bool pSortList = false)
        {
            string _SortList = pSortList ? "Cod" : "Value";
            string _Query = $@"
                select *
                from dbo.{TabName}
                where ID = {ID} and Nomer = {VarId}
                order by {_SortList}";
            return _Query;
        }

        /// <summary>Варианты ответа Нового справочника s_List</summary>
        /// <param name="ID">Имя индификатора списка</param>
        /// <param name="isDate">Дата действия ответа (как правило сегодняшняя)</param>
        /// <param name="isNumber">Отображать (true) или не отображать (false - по умолчанию)</param>
        /// <param name="sortPole">Имя поля по которому будет сортироваться список по умолчанию</param>
        public static string MET_s_List_Select_5(string ID, DateTime isDate, bool isNumber = false, string sortPole = "Number")
        {
            int _isNumber = isNumber ? 1 : 0;
            string _Query = $@"
                declare @ID as nvarchar(30) = '{ID}'
                declare @isNumber as int = {_isNumber}; -- 0 - скрывать отрицательные значения Number, 1 - показывать все Number
                declare @isDate as date = '{isDate:MM.dd.yyyy}'  -- действуют на момент
                select Cod     
                      ,Number
                      ,Value
                      ,ValueCod
                from dbo.s_List
                where ID = @ID
                    and (@isNumber = 0 and Number > 0 or @isNumber = 1)
                    and @isDate between DateBeg and DateEnd
                    and xDelete = 0
                order by {sortPole}";
            return _Query;
        }

        /// <summary>Удаляем вариант ответа</summary>
        public static string MET_List_Delete_1(string TabName, int Cod)
        {
            string _Query = $@"
                delete dbo.{TabName}
                where Cod = {Cod}";
            return _Query;
        }

        /// <summary>Добавляем вариант ответа</summary>
        public static string MET_List_Insert_1(string TabName, int Cod, int ID, int Nomer, string Value)
        {
            string _Query = $@"
                insert dbo.{TabName}
                    (Cod, ID, Nomer, [Value], Flag)
                values
                    ({Cod}, {ID}, {Nomer}, '{Value}', 1)";
            return _Query;
        }

        /// <summary>Меняем вариант ответа</summary>
        public static string MET_List_Update_1(string TabName, int Cod, string Value)
        {
            string _Query = $@"
                update dbo.{TabName}
                  set Value = '{Value}'
                where Cod = {Cod}";
            return _Query;
        }
        #endregion

        #region ---- lnzVrachLS (таблица Назначения ЛС) ----
        /// <summary>Выборка записи lnzVrachLS по коду</summary>
        public static string lnzVrachLS_Select_1(decimal CodApstac)
        {
             string _Query = $@"
                select ln.*
                    ,ln.DateN + (ln.Kurs - 1) * ln.Period as DateK
                    ,u.Fio             as UserVrachName
                    ,ud.Fio            as DelUserVrachName
                from dbo.lnzVrachLS     as ln
                left join dbo.s_Users   as u   on ln.UserVrach = u.Cod
                left join dbo.s_Users   as ud  on ln.DelUserVrach = ud.Cod
                where isnull(ln.xDelete, 0) = 0 and ln.CodApstac = {CodApstac}";
             return _Query;
        }

        /// <summary>Количество назначений lnzVrachLS по CodApstac</summary>
        public static string lnzVrachLS_Select_2(decimal CodApstac)
        {
             string _Query = $@"
                select Cod
                from dbo.lnzVrachLS
                where isnull(xDelete, 0) = 0 and CodApstac = {CodApstac}";
             return _Query;
        }

        /// <summary>Минимальная дата назначений lnzVrachLS по CodApstac</summary>
        public static string lnzVrachLS_Select_3(decimal CodApstac)
        {
             string _Query = $@"
                select min(pDate)
                from dbo.lnzVrachLS
                where CodApstac = {CodApstac}";
             return _Query;
        }

        /// <summary>Для листа назначения находим аллегию из осмотра при поступлении</summary>
        public static string lnzVrachLS_Select_4(decimal CodApstac)
        {
            string _Query = $@"
                use Bazis;
                select top 1 isnull(dbo.GetPole(s.VarId, Protokol), 'не указано') as al
                from dbo.astProtokol as p
                join dbo.astShablon as s  on p.NumShablon = s.ID and s.Name = 'Аллергия'
                where p.NumShablon%100 = 1
                    and p.NumShablon < 2000
                    and isnull(p.xDelete, 0) = 0
                    and p.CodApstac = {CodApstac}";
            return _Query;
        }

        /// <summary>Удаляем записи lnzVrachLS</summary>
        public static string lnzVrachLS_Delete_1(int Cod)
        {
             string _Query = $@"
                delete dbo.lnzVrachLS
                where Cod = {Cod}";
             return _Query;
        }

        /// <summary>Добавляем записи lnzVrachLS</summary>
        public static string lnzVrachLS_Insert_1(int Cod, decimal KL, decimal CodApstac, DateTime DateN, int Kurs, int Period, string NameLS,
                                                 int Amt, string Route, string Note, int FlagDrug, int FlagPac, int UserVrach, int DelUserVrach,
                                                 DateTime DelDate, string DelNote, DateTime pDate, DateTime xDateUp)
        {
             string _Query = $@"
                insert dbo.lnzVrachLS
                      (Cod
                       ,KL
                       ,CodApstac
                       ,DateN
                       ,Kurs
                       ,Period
                       ,NameLS
                       ,Amt
                       ,Route
                       ,Note
                       ,FlagDrug
                       ,FlagPac
                       ,UserVrach
                       ,DelUserVrach
                       ,DelDate
                       ,DelNote
                       ,pDate
                       ,xDateUp)
               values({Cod}
                     ,{KL}
                     ,{CodApstac}
                     ,'{DateN:MM.dd.yyyy}'
                     ,{Kurs}        -- курс (кол. дней)
                     ,{Period}
                     ,'{NameLS}'    -- сборное наименование
                     ,{Amt}         -- схема приема (раз в день)
                     ,'{Route}'     -- способ приема
                     ,'{Note}'      -- примечание
                     ,{FlagDrug}    -- наркотики
                     ,{FlagPac}     -- л.с пациента
                     ,{UserVrach}   -- пользователь - последний
                     ,{DelUserVrach}            -- пользователь отменивший
                     ,'{DelDate:MM.dd.yyyy}'    -- дата отмены
                     ,'{DelNote}'               -- причина отмены
                     ,'{pDate:MM.dd.yyyy}'      -- дата создания
                     ,'{xDateUp:MM.dd.yyyy}')    -- дата изменения";
             return _Query;
        }

        /// <summary>Меняем записи lnzVrachLS</summary>
        public static string lnzVrachLS_Update_1(int Cod, DateTime DateN, int Kurs, int Period, string NameLS, int Amt, string Route, string Note,
                                                 int FlagDrug, int FlagPac, int UserVrach, int DelUserVrach, DateTime DelDate, string DelNote,
                                                 DateTime xDateUp)
        {
             string _Query = $@"
                update dbo.lnzVrachLS
                  set  DateN        = '{DateN:MM.dd.yyyy}'  -- начальная дата
                       ,Kurs        = {Kurs}                -- курс (кол. дней)
                       ,Period      = {Period}              -- период
                       ,NameLS      = '{NameLS}'            -- сборное наименование
                       ,Amt         = {Amt}                 -- схема приема (раз в день)
                       ,Route       = '{Route}'             -- способ приема
                       ,Note        = '{Note}'              -- примечание
                       ,FlagDrug    = {FlagDrug}            -- наркотики
                       ,FlagPac     = {FlagPac}             -- л.с пациента
                       ,UserVrach   = {UserVrach}           -- пользователь - последний
                       ,DelUserVrach = {DelUserVrach}       -- пользователь отменивший
                       ,DelDate     = '{DelDate:MM.dd.yyyy}' -- дата отмены
                       ,DelNote     = '{DelNote}'            -- причина отмены
                       ,xDateUp     = '{xDateUp:MM.dd.yyyy}' -- дата изменения
                where Cod = {Cod}";
             return _Query;
        }
        #endregion

        #region ---- lnzKompLS (таблица Компоненты ЛС) ----
        /// <summary>Выборка записи lnzKompLS по коду</summary>
        public static string lnzKompLS_Select_1(int CodVrachLS)
        {
             string _Query = $@"
                select k.*, l.BazeMeas
                from dbo.lnzKompLS  as k
                left join dbo.lnzLS as l
                  on k.CodLS = l.Cod
                where k.CodVrachLS = {CodVrachLS}";
             return _Query;
        }

        /// <summary>Удаляем записи lnzKompLS по коду</summary>
        public static string lnzKompLS_Delete_1(int Cod)
        {
             string _Query = $@"
                delete dbo.lnzKompLS
                where Cod = {Cod}";
             return _Query;
        }

        /// <summary>Удаляем записи lnzKompLS всего назначения</summary>
        public static string lnzKompLS_Delete_2(int CodVrachLS)
        {
             string _Query = $@"
                delete dbo.lnzKompLS
                where CodVrachLS = {CodVrachLS}";
             return _Query;
        }

        /// <summary>Добавляем записи lnzKompLS</summary>
        public static string lnzKompLS_Insert_1(int Cod, int CodVrachLS, decimal CodApstac, int CodLS, string NameKomp, double Doza)
        {
            string _Doza = Doza.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
            string _Query = $@"
                insert dbo.lnzKompLS
                    (Cod, CodVrachLS, CodApstac, CodLS, NameKomp, Doza)
                values
                    ({Cod}, {CodVrachLS}, {CodApstac}, {CodLS}, '{NameKomp}', {_Doza})";
             return _Query;
        }

        /// <summary>Меняем записи lnzKompLS</summary>
        public static string lnzKompLS_Update_1(int Cod, int CodVrachLS, int CodLS, string NameKomp, double Doza)
        {
            string _Doza = Doza.ToString("F", CultureInfo.CreateSpecificCulture("en-US"));
            string _Query = $@"
                update dbo.lnzKompLS
                  set  CodVrachLS = {CodVrachLS}
                       ,CodLS     = {CodLS}
                       ,NameKomp  = '{NameKomp}'
                       ,Doza      = {_Doza}
                where Cod = {Cod}";
             return _Query;
        }
        #endregion

        #region ---- lnzLS (таблица Cправочник ЛС) s_Drugs ----
        /// <summary>Cправочник ЛС с группировкой по NameVrach</summary>
        public static string lnzLS_Select_1(string proSqlWhere)
        {
             string _Query = $@"
                select  concat(l.NameVrach, ';', g.Name) as Filter
                        ,min(l.Cod) as Cod, g.Cod as Grou, l.NameVrach, l.BazeMeas, g.Name
                from dbo.lnzLS            as l
                left join dbo.lnzGroup  as g
                  on l.SpecNom = g.Cod
                where isnull(l.xDelete, 0) = 0 and g.Cod <> 0000022 {proSqlWhere}
                group by NameVrach, BazeMeas, g.Name,  g.Cod
                order by NameVrach, BazeMeas";
             return _Query;
        }

        /// <summary>Cправочник ЛС по торговому наименованию NameLS</summary>
        public static string lnzLS_Select_2(string proSqlWhere)
        {
             string _Query = $@"
                select concat(l.NameLS, ';', g.Name) as Filter
                       ,l.Cod, g.Cod as Grou, l.NameLS as NameVrach, l.BazeMeas, g.Name
                from dbo.lnzLS            as l
                left join dbo.lnzGroup    as g
                  on l.SpecNom = g.Cod
                where isnull(l.xDelete, 0) = 0 and g.Cod <> 0000022 {proSqlWhere}
                order by NameLS, BazeMeas";
             return _Query;
        }

        /// <summary>Cправочник ЛС по торговому наименованию NameLS</summary>
        public static string s_Drugs_Select_1(string proSqlWhere)
        {
             string _Query = $@"
                select
                      case
                         when main.DrugType > 2 then 2
                         else main.DrugType
                       end as DrugType
                      ,main.Cod
                      ,main.Descr
                      ,diMain.Descr as MainMeas
                      ,diMNN.Descr as MNN
                from Drugs.dbo.s_Drugs as main
                left join Drugs.dbo.s_Dict as diMain
                  on diMain.InnerCod = main.MainMeas and diMain.DictCod = 1
                left join Drugs.dbo.s_MNN as diMNN
                  on diMNN.Cod = main.MNN
                where diMain.isMark = 0 {proSqlWhere}
                order by main.Descr";
             return _Query;
        }
        #endregion

        #region ---- log_wpfBazis (таблица логов из базы LogBazis) s_Drugs ----
        /// <summary>Логирование wpfBazis</summary>
        public static string log_wpfBazis_Select_1(string proSqlWhere, string pServer)
        {
            string _Query = $@"
                select iif( charindex('SQL:', Data) > 0 and len(Data) > 5,
                            replace(right(Data, len(Data) - charindex('SQL:', Data) - 4),
                            char(13) + char(10) + '                ', char(13) + char(10)), Data) as Data
                      ,CodApstac
                      ,KL
                      ,Cod
                      ,UserCod
                      ,UserName
                      ,CompName
                      ,Ver
                      ,Process
                      ,convert(nchar(19), LogDate , 20) as LogDate
                      ,LogLevel
                      ,Message
                      ,Exception
                      ,StackTrace
                      ,iif(len(Data) > 0,'V','') as Data
                from {pServer}.LogBazis.dbo.log_wpfBazis
                {proSqlWhere}";
            return _Query;
        }

        /// <summary>Удаление записей из логов log_wpfBazis</summary>
        public static string log_wpfBazis_Delete_1(string proSqlWhere, string pServer)
        {
            string _Query = $@"
                delete {pServer}.LogBazis.dbo.log_wpfBazis
                {proSqlWhere}";
            return _Query;
        }

        /// <summary>Поиск пациента из kbol, APAC, APSTAC, parObsledov для перехода к редактированию из окна Логов ошибок</summary>
        public static string log_wpfBazis_Select_2(decimal pKL, decimal pIND)
        {
            string _Query = $@"
                    use Bazis;

                    declare @KL as decimal(18, 0) = '{pKL}';
                    declare @IND as decimal(18, 0) = '{pIND}';

                    select top 1 *
                    from (
                        -- Если поликлиника
                        select k.KL, 'пол' as Tip, @IND as IND, null Otd, 1 as Nom
                        from dbo.kbol as k with (nolock)
                        join dbo.APAC as a with (nolock)  on k.KL = a.KL
                        where Cod = @IND and k.KL = @KL
                        union
                        -- Если стационар
                        select k.KL, 'стац' as Tip, a.IND, a.otd, 2 as Nom
                        from dbo.kbol as k with (nolock)
                        join dbo.APSTAC as a with (nolock)  on k.KL = a.KL
                        where IND = @IND and k.KL = @KL
                        union
                        -- Если параклиника
                        select k.KL, 'пар' as Tip, o.Cod, null Otd, 3 as Nom
                        from dbo.kbol as k with (nolock)
                        join dbo.parObsledov as o with (nolock) on k.KL = o.KL
                        where Cod = @IND and k.KL = @KL
                        union
                        -- Иначе просто пациент
                        select k.KL, 'пациент' as Tip, 0 as IND, null Otd, 4 as Nom
                        from dbo.kbol as k with (nolock)
                        where KL = @KL
                    ) as d
                    order by Nom";
            return _Query;
        }
        #endregion

        #region ---- Oper (таблица Операций) ----
        /// <summary>Выборка записей Oper</summary>
        public static string Oper_Select_1(decimal IND)
        {
             string _Query = $@"
                select Cod
                from dbo.Oper
                where (astProtokol = 0 or astProtokol is null) and IND = {IND} and isnull(xDelete, 0) = 0";
             return _Query;
        }

        /// <summary>Выборка записей Oper</summary>
        public static string Oper_Select_2(int astProtokol)
        {
             string _Query = $@"
                select *
                from dbo.Oper
                where astProtokol = {astProtokol} and isnull(xDelete, 0) = 0
                order by Cod";
             return _Query;
        }

        /// <summary>Удаляем записи Oper, если они не синхронизированные</summary>
        public static string Oper_Delete_1(int astProtokol)
        {
             string _Query = $@"
                delete dbo.Oper
                where astProtokol = {astProtokol} and isnull(xDelete, 0) = 0 and ((xImport = 1 and {MyGlo.Server} in (2, 5)) or (xImport = 3 and {MyGlo.Server} in (3, 6)))";
             return _Query;
        }

        /// <summary>Помечаем для удаления записи Oper, если они синхронизированны</summary>
        public static string Oper_Update_1(int astProtokol)
        {
             string _Query = $@"
                update dbo.Oper
                    set xDelete = 1
                where astProtokol = {astProtokol}";
             return _Query;
        }

        /// <summary>Добавляем записи Oper</summary>
        public static string Oper_Insert_1(int Cod, decimal KL, decimal INT, DateTime Dat, int Otd, string Oper, int astProtokol, string xInfo)
        {
             string _Query = $@"
                 insert dbo.Oper
                    (Cod, KL, IND, Dat, Otd, Oper, Kv, astProtokol, xDelete, xInfo)
                 values
                    ({Cod}, {KL}, {INT}, '{Dat:MM.dd.yyyy}', {Otd}, '{Oper}', 0, {astProtokol}, 0, '{xInfo}')";
             return _Query;
        }
        #endregion

        #region ---- parGroup (группы ЛС) ----
        /// <summary>Выборка групп ЛС parGroup</summary>
        public static string lnzGroup_Select_1()
        {
             string _Query = @"
                select Cod, Name
                from dbo.lnzGroup";
             return _Query;
        }
        #endregion

        #region ---- NextRef (таблица последнего Cod) ----
        /// <summary>Находим Cod NextRef, выбранной таблицы</summary>
        public static string NextRef_Select_1(int TableRef)
        {
             string _Query = $@"
                select NextRef
                from dbo.NextRef
                where TableRef = {TableRef}";
             return _Query;
        }

        /// <summary>Увеличиваем Cod NextRef, выбранной таблицы</summary>
        public static string NextRef_Update_1(int TableRef)
        {
             string _Query = $@"
                update dbo.NextRef
                    set NextRef = NextRef + 1
                where TableRef = {TableRef}";
             return _Query;
        }
        #endregion

        #region ---- parElement (элементы параклиники) ----
        /// <summary>Выборка элементов параклиники parElement</summary>
        public static string parElement_Select_1()
        {
             string _Query = @"
                select Cod, Name
                from dbo.parProfil
                where isnull(xDelete, 0) = 0";
             return _Query;
        }

        /// <summary>Выборка элементов параклиники parElement</summary>
        public static string parElement_Select_2(int pCod)
        {
             string _Query = $@"
                select PROFIL
                from dbo.parElement
                where isnull(xDelete, 0) = 0 and Cod = {pCod}";
             return _Query;
        }
        #endregion

        #region ---- parObsledov (список обследованных пациентов) ----
        /// <summary>Выборка аппарата параклиники parApparat</summary>
        public static string parObsledovt_Select_1(string proSqlWhere)
        {
             string _Query = $@"
                select top(200)
                     o.Cod
                    ,k.KL
                    ,dbo.GetFIO(k.FAM, I, O) as FIO
                    ,k.DR
                    ,o.DP
                    ,i.[Name] as Issl
                from dbo.kbol                 as k
                join dbo.parObsledov          as o
                  on  o.KL = k.KL
                join dbo.parIssledov          as i
                  on i.Cod = o.Issledov
                {proSqlWhere}
                order by k.FAM, k.DR, o.DP desc, i.[Name]";
             return _Query;
        }

        /// <summary>Выборка аппарата параклиники parApparat</summary>
        public static string parObsledovt_Select_2(int Cod)
        {
             string _Query = $@"
                select a.Name
                from dbo.parApparat   as a
                join dbo.parObsledov  as o
                  on a.Cod = o.Apparat
                where o.Cod = {Cod}";
             return _Query;
        }

        /// <summary>Выборка исследования параклиники parIssledov</summary>
        public static string parObsledovt_Select_3(int Cod)
        {
             string _Query = $@"
                select i.Name
                from dbo.parIssledov  as i
                join dbo.parObsledov  as o
                  on i.Cod = o.Issledov
                where isnull(i.xDelete, 0) = 0 and o.Cod = {Cod}";
             return _Query;
        }

        /// <summary>Выборка протоколов Параклиники для ОМС</summary>
        public static string parObsledovt_Select_5(string proSqlWhere)
        {
            string _Query = $@"
                select  o.Cod
                    ,k.KL
                    ,concat(o.Cod, ';', k.KL, ';', dbo.GetFIO(k.FAM, k.I, k.O)) as Filter
                    ,isnull(p.xDelete, 0) as Del
                    ,dbo.GetFIO(k.FAM, k.I, k.O) as FAM
                    ,cast(k.DR as date)    as DR
                    ,u.FIO        as Vrach
                    ,cast(p.pDate as date)    as DP
                    ,s.CODE_USL    as CodUsl
                    ,iif(len(s.Discript) > 35, left(s.Discript, 30) + '  ...', s.Discript) as DiscripUsl
                from dbo.parProtokol    as p
                join dbo.parObsledov    as o    on p.CodApstac = o.Cod
                join dbo.kbol            as k    on p.KL = k.KL
                join dbo.kbolInfo        as i    on i.Tab = 'par' and o.Cod = i.CodZap
                join dbo.s_UsersDostup  as d    on p.xUserUp = d.UserCod and isjson(d.xInfo) > 0 and json_value(d.xInfo, '$.element') = 6 and d.isWork = 0
                join dbo.s_Users        as u    on u.Cod = d.UserCod
                join dbo.StrahStacSv    as s    on s.Flag = 10 and p.pDate between s.DateN and s.DateK and json_value(i.jTag, '$.Usl') = s.CODE_USL
                left join dbo.s_LPU        as l    on k.StrLPU = l.StrLPU
                where p.NumShablon in (518, 519)
                    and isjson(i.jTag) > 0
                    {proSqlWhere}
                order by k.FAM, k.I, k.O, k.DR, p.pDate";
            return _Query;
        }
        #endregion

        #region ---- parTalon (талоны параклиники) ----
        /// <summary>Выборка талонов параклиники parTalon</summary>
        public static string parTalon_Select_1(string proSqlWhere)
        {
             string _Query = $@"
                  select top 200 cast(t.[Date] as date) as DatePos
                        ,t.FIO
                        ,cast(k.DR as date) as DR
                        ,iif(t.Flag = 4, 'принят', '') as Flag
                        ,iif(t.FlagPay = 1, 'платно', '') as Pay
                       ,case
                          when t.WhereCod = 1 then 'Поликлиника - 1'
                          when t.WhereCod = 2 then 'Поликлиника - 2'
                          else o.Names
                        end as Otd
                       ,t.SendName
                       ,e.FIO as Element
                      ,i.Name as Issled
                from Bazis.dbo.parTalon          as t
                join Bazis.dbo.parElement        as e on t.Element = e.COD
                join Bazis.dbo.kbol              as k on t.KL = k.KL
                left join Bazis.dbo.s_Department as o on t.Otdel = o.Cod
                join Bazis.dbo.parProfil         as p on e.PROFIL = p.COD
                join Bazis.dbo.parIssledov       as i on t.TipObsled = i.Cod
                {proSqlWhere}
                order by k.FAM, k.DR, t.Date desc";
             return _Query;
        }
        #endregion

        #region ---- s_Department (справочник подразделений) ----
        /// <summary>Выборка списка отделений s_Department</summary>
        public static string s_Department_Select_1(string pWhere = "")
        {
             string _Query = $@"
                select  concat(Cod, ';', Names) as Filtr, Cod, Names, iif(Korpus = 1, 'главный', 'филиал') as Korpus
                from dbo.s_Department
                where isnull(xDelete, 0) = 0 {pWhere}
                order by Names";
             return _Query;
        }

        /// <summary>Тип отделения стационара s_Department</summary>
        public static string s_Department_Select_2(int pCod)
        {
            string _Query = $@"
                select Tip
                from dbo.s_Department
                where Cod = {pCod}";
            return _Query;
        }
        #endregion

        #region ---- s_Diag (справочник МКБ-10) ----
        /// <summary>Cправочник МКБ-10 s_Diag</summary>
        public static string s_Diag_Select_1(string proSqlWhere)
        {
             string _Query = $@"
                select KOD + ';' + TKOD as Filter, KOD, TKOD
                from dbo.s_Diag
                where isnull(xDelete, 0) = 0 and SCE <> 1 {proSqlWhere}";
             return _Query;
        }
        #endregion

        #region ---- s_List ----
        /// <summary>Варианты ответа</summary>
        public static string MET_s_List_Select_1(string ID)
        {
            string _Query = $@"
                select Cod, Value
                from dbo.s_ListDocum
                where ID = '{ID}' and Nomer > 0";
            return _Query;
        }

        /// <summary>Выборка значения Value по 3м параметрам</summary>
        public static string MET_s_List_Select_2(string ID, int Nomer)
        {
            string _Query = $@"
                select Value
                from dbo.s_ListDocum
                where ID = '{ID}' and Nomer = {Nomer}";
            return _Query;
        }

        /// <summary>Варианты ответа</summary>
        public static string MET_s_List_Select_3(string ID)
        {
            string _Query = $@"
                select Cod, ID, Number Value, ValueCod, DateBeg, DateEnd, xInfo, xLog, xDelete
                from dbo.s_List
                where ID = '{ID}'";
            return _Query;
        }
        #endregion

        #region ---- s_ListImage (список картинок) ----
        /// <summary>Выборка списка рисунков s_ListImage</summary>
        public static string s_ListImage_Select_1()
        {
             string _Query = @"
                select concat(ID, ';', Comment) as Filter, Cod, ID, Nomer, NameImage, Image, Comment
                from dbo.s_ListImage
                where isnull(xDelete, 0) = 0";
             return _Query;
        }

        /// <summary>Выборка списка рисунков s_ListImage</summary>
        public static string s_ListImage_Select_1(int ID, int Nomer)
        {
             string _Query = $@"
                select Cod, ID, Nomer, NameImage, Image, Comment
                from dbo.s_ListImage
                where ID = {ID} and Nomer = {Nomer}";
             return _Query;
        }

        /// <summary>Максимальный код рисунка для этого шаблона и номера s_ListImage</summary>
        public static string s_ListImage_Select_2(int ID, int Nomer)
        {
             string _Query = $@"
                select isnull(max([Cod]), {ID}*100000 + {Nomer}*1000)
                from dbo.s_ListImage
                where ID = {ID} and Nomer = {Nomer}";
             return _Query;
        }

        /// <summary>Вставка рисунка в s_ListImage (не реализован)</summary>
        public static string s_ListImage_Insert_1(int locCod, int locID, int locNomer, string locNameImage, string locComment)
        {
            throw new NotImplementedException();
        }

        /// <summary>Изменение рисунка s_ListImage</summary>
        public static string s_ListImage_Update_1(int Cod, int ID, int Nomer, string NameImage, string Comment)
        {
             string _Query = $@"
                update dbo.s_ListImage
                  set ID = {ID}
                    ,Nomer = {Nomer}
                    ,NameImage = '{NameImage}'
                    ,Image = @Image
                    ,Comment = '{Comment}'
                where Cod = {Cod}";
             return _Query;
        }
        #endregion

        #region ---- s_ListDocum ----
        /// <summary>Варианты ответа 3м параметрам</summary>
        public static string MET_s_ListDocum_Select_1(string ID)
        {
            string _Query = $@"
                select Cod, Value
                from dbo.s_ListDocum
                where ID = '{ID}' and Nomer > 0";
            return _Query;
        }

        /// <summary>Выборка значения Value по 3м параметрам</summary>
        public static string MET_s_ListDocum_Select_2(string ID, int Nomer)
        {
            string _Query = $@"
                select Value
                from dbo.s_ListDocum
                where ID = '{ID}' and Nomer = {Nomer}";
            return _Query;
        }
        #endregion

        #region ---- s_MorfTip (справочник мрфологического типа) ----
        /// <summary>Справочник морфологического типа s_MorfTip</summary>
        public static string s_MorfTip_Select_1()
        {
             string _Query = @"
                use Bazis;
                select MTip + ' - ' + [Name] as Dec
                from dbo.s_MorfTip
                where isnull(xDelete, 0) = 0 and FlagGr = 0";
             return _Query;
        }

        /// <summary>Морфологического типа s_MorfTip (выводим код + наименование)</summary>
        public static string s_MorfTip_Select_2(string pMTip)
        {
             string _Query = $@"
                select MTip + ' ' +[Name]
                from dbo.s_MorfTip
                where isnull(xDelete, 0) = 0 and MTip = '{pMTip}'";
             return _Query;
        }
        #endregion

        #region ---- s_VidOper (справочник операций) ----
        /// <summary>Справочник операций s_VidOper</summary>
        public static string s_VidOper_Select_1(string pWhere)
        {
             string _Query = $@"
                select KOP + ';' + [NAME] as Names
                    -- Считаем количество препаратов по количеству плюсиков
                    ,iif(KOP between 'sh001' and 'sh899', ((datalength([NAME])/2-datalength(replace([NAME],'+',''))/2)/datalength('+')) + 1
                            - iif(charindex('+', left([NAME],charindex(':', [NAME]))) > 0, 1, 0), 0) as CountLS  -- Отнимаем один лишний плюсик, который не относится к разделению лекарств
                    ,KOP
                    ,[NAME]
                from dbo.s_VidOper
                where isnull(xDelete, 0) = 0 {pWhere}
                order by KOP";
            return _Query;
        }

        /// <summary>Справочник операций s_VidOper проверка на наличие записей</summary>
        public static string s_VidOper_Select_2(string pWhere)
        {
            string _Query = $@"
                select 1
                from dbo.s_VidOper
                where isnull(xDelete, 0) = 0 {pWhere}";
            return _Query;
        }
        #endregion

        #region ---- s_VrachStac (справочник врачей стационара) ----
        /// <summary>Cправочник врачей стационара s_VrachStac</summary>
        public static string s_VrachStac_Select_1()
        {
             string _Query = @"
                select concat(v.KOD, ';', v.TKOD, ';', isnull(p.TKOD, ''), ';', isnull(o.Names, '')) as Filter
                    ,v.KOD
                    ,v.TKOD
                    ,'PROF' = isnull(p.TKOD, '')
                    ,'Otd'  = isnull(o.Names, '')
                from dbo.s_VrachStac        as v
                left join dbo.s_ProfOtStac    as p  on v.SPCS = p.KOD
                left join dbo.s_Department    as o  on v.Otd = o.Cod
                where isnull(v.xDelete, 0) = 0";
             return _Query;
        }
        #endregion

        #region ---- s_VrachPol (справочник врачей поликлиники) ----
        /// <summary>Cправочник врачей поликлиники s_VrachPol</summary>
        public static string s_VrachPol_Select_1()
        {
             string _Query = @"
                select concat(v.KOD, ';', v.TKOD, ';', isnull(p.[Name], ''), ';', isnull(s.TKOD, ''), ';', isnull(z.TKOD, '')) as Filter
                    ,v.KOD
                    ,v.TKOD
                    ,'Prof' = isnull(p.[Name], '')
                    ,'Spec' = isnull(s.TKOD, '')
                    ,'Podrazd' = isnull(z.TKOD, '')
                from dbo.s_VrachPol            as v
                left join dbo.s_SpecVrPol    as s
                  on v.SPC = s.KOD
                left join dbo.s_ProfPol        as p
                  on v.SPCS = p.Cod
                left join dbo.z_Podrazd        as z
                  on v.Podrazd = z.Cod
                where isnull(v.xDelete, 0) = 0 and v.NU = 1";
             return _Query;
        }
        #endregion

        #region ---- StrahTarif (Тарифы и связи) ----
        /// <summary>Справочник Тарифы и связей</summary>
        public static string StrahTarif_Select_1()
        {
            string _Return = @"
                use Bazis;

                select concat(Flags, ';', CODE_USL, ';', VID_VME, ';', Discript) as Filter
                    ,Cod
                    ,PRVS
                    ,PROFIL
                    ,PROFIL_K
                    ,CODE_USL
                    ,VID_VME
                    ,Flags as Flag
                    ,VidPom
                    ,Discript
                    ,Tarif
                    ,Child
                    ,DateN
                    ,DateK
                from (
                select *
                        ,case 
                            when Flag = 0 then concat(Flag, ' - кр.стационар')
                            when Flag = 1 then concat(Flag, ' - дн.стационар')
                            when Flag = 2 then concat(Flag, ' - раз.поликл.')
                            when Flag = 8 then concat(Flag, ' - обр.пол.перв')
                            when Flag = 9 then concat(Flag, ' - обр.пол.повт')
                            when Flag = 7 then concat(Flag, ' - ВМП')
                            when Flag = 10 and (left(CODE_USL, 2) = 'P1' or  left(CODE_USL, 6) = '3078.2') then concat(Flag, ' - P1-МРТ')
                            when Flag = 10 and (left(CODE_USL, 2) = 'P2' or  left(CODE_USL, 6) = '3078.3') then concat(Flag, ' - P2-КТ')
                            when Flag = 10 and left(CODE_USL, 2) = 'P3' then concat(Flag, ' - P3-ЭХОЭКГ')
                            when Flag = 10 and left(CODE_USL, 2) = 'P4' then concat(Flag, ' - P4-Эндоскопия')
                            when Flag = 11 and left(CODE_USL, 2) = 'P5' then concat(Flag, ' - P5-Гистология')
                            when Flag = 11 and left(CODE_USL, 2) = 'P6' then concat(Flag, ' - P6-Молек. гинет.')
                            when Flag = 12 then concat(Flag, ' - Телемедицина')
                      end as Flags
                from dbo.StrahTarif) as d";
            return _Return;
        }
        #endregion

        #region ---- StrahVMP (справочник Методы ВМП) ----
        /// <summary>Cправочник Методы ВМП StrahVMP</summary>
        public static string StrahVMP_Select_1(string pWhere)
        {
            string _Query = $@"
                select concat(HVid, ';', IDHM, ';',left(Model, 30), ';', HMName) as Names
                    ,json_value(xInfo, '$.TipVMP') as TipVMP
                    ,HVid, IDHM, left(Model, 30) as Model, HMName
                from dbo.StrahVMP
                where isjson(xInfo) > 0 and xEndDate = '01/01/2222' {pWhere}
                order by TipVMP, Names";
            return _Query;
        }

        /// <summary>Для фильтра Вид ВМП</summary>
        public static string varVidVMP_Select_1()
        {
            string _Query = @"
                select distinct f.TipVMP
                    ,f.Names
                from Bazis.dbo.StrahVMP as v
                left join (values
                    ('1', '1 - Видеоэндоскопич., интервенцион.радиологич., малоинвазивные органосохр.хирург.вмешат'),
                    ('2', '2 - Расширенные хирургические вмешательства'),
                    ('3', '3 - Комбинированное лечение ЗНО (хирургическое и противоопух.лекарственное)'),
                    ('4', '4 - Лучевая терапия'),
                    ('5', '5 - Комплексная и высокодозная химиотерапия (включая таргетную)'),
                    ('6', '6 - Региональное ВМП')) as f(TipVMP, Names)
                  on json_value(v.xInfo, '$.TipVMP') = f.TipVMP
                where v.xEndDate = '01/01/2222' and isjson(v.xInfo) > 0
                order by f.TipVMP";
            return _Query;
        }
        #endregion

        #region ---- z_Constt (константы) ----
        /// <summary>Корпус</summary>
        public static string z_ConsttKorpus_Select_1()
        {
             string _Query = @"
                select Korpus
                from dbo.z_Constt";
             return _Query;
        }
        #endregion

        #region ---- Разное ----
        /// <summary>Выбор квартала</summary>
        public static string MET_varKvartal_Select_1(DateTime pDateN, DateTime pDateK)
        {
             string _Query = $@"
                select top(datediff(month,'{pDateN:MM.dd.yyyy}', '{pDateK:MM.dd.yyyy}')/3+2)
                        r.i
                       ,r.Dat
                       ,(month(r.Dat)-1)/3+1    as Kvar
                       ,year(r.Dat)             as Years
                       ,cast((month(r.Dat)-1)/3+1 as varchar(1)) + ' квартал ' + cast(year(r.Dat) as varchar(4)) + ' года' as Label
                from (select dateadd(month, ((d.i - 1) * 3), '{pDateN:MM.dd.yyyy}') as Dat, d.i
                      from (select row_number() over(order by name) as i
                            from master..spt_values) as d) as r";
             return _Query;
        }

        /// <summary>Меняем поле строковое в какой либо таблице</summary>
        public static string MET_varString_Update_1(string TabName, string NamePoleValue, string Value, string NamePoleCod,  decimal Cod )
        {
             string _Query = $@"
                update dbo.{TabName}
                  set {NamePoleValue} = '{Value}'
                where {NamePoleCod} = {Cod}";
             return _Query;
        }

        /// <summary>Находим отделение, для смены талона</summary>
        public static string MET_varOtd_Select_1(eTipDocum pTip, decimal pCod )
        {
            string _Query = "";
            // В зависимости от типа протокола ищем Отделение
            switch (pTip)
            {
                case eTipDocum.Paracl:
                     _Query = $@"
                        select [Type] as Otd
                        from dbo.parObsledov as o
                        join dbo.parIssledov as i
                          on o.Issledov = i.Cod
                        where o.Cod = {pCod}";
                    break;
                case eTipDocum.Stac:
                     _Query = $@"
                        select Otd
                        from Bazis.dbo.APSTAC
                        where IND = {pCod}";
                    break;
                case eTipDocum.Pol:
                     _Query = $@"
                        select SPCS as Otd
                        from dbo.APAC as a
                        join dbo.s_VrachPol as v
                          on a.KV = v.KOD
                        where a.Cod = {pCod}";
                    break;
            }
            return _Query;
        }

        /// <summary>Расчет Суммы тарифа Поликлиники</summary>
        public static string MET_varSumTarifPol_Select_1(decimal pInd)
        {
            string _Query = $"select cast(other.GetStrahTarifReport({pInd}, 1, 1) as nvarchar) as Summa";
            return _Query;
        }

        /// <summary>Расчет Суммы тарифа Стационара</summary>
        public static string MET_varSumTarifStac_Select_1(decimal pInd)
        {
            string _Query = $"select cast(other.GetStrahTarifReport({ pInd}, 2, 1) as nvarchar) as Summa";
            return _Query;
        }

        /// <summary>Проверяем наличие пациента в Канцер-Регистре</summary>
        public static string MET_varIfRakReg_Select_1(decimal KL)
        {
            string _Query = $@"
                select 1
                from RakReg.dbo.kr_Pasport
                where KL = {KL}";
            return _Query;
        }

        /// <summary>Выстаскиваем Все данный из Канцер Регистра</summary>
        public static string MET_varRakReg_Select_2(decimal KL)
        {
            string _Query = $@"
                use RakReg;
                declare @CD as int = (select CD from dbo.kr_Pasport where KL = {KL});

                -- ПАСПОРТ
                select '1_Паспорт' as Tip, 1 as Cod, NameKey, Value
                from (
                    select  convert(nvarchar(255), D_DeathSDate, 104) as DSmert -- дата смерти
                           ,cast(sik.Text as nvarchar(255)) as Death            -- причина смерти
                           ,cast(Adres as nvarchar(255)) as Adres               -- адрес из канцера
                           ,convert(nvarchar(255), D_RegSDate, 104) as DReg     -- дата взятия на учет в ООД
                           ,convert(nvarchar(255), D_UnRegSDate, 104) as DUnReg -- дата снятия с учета
                           ,cast(svU.Text as nvarchar(255)) as Snjat            -- причина снятия с учета
                           ,cast(lpu.Name as nvarchar(255)) as Lpu              -- ЛПУ приписки
                           ,cast(svK.Text as nvarchar(255)) as KlGr             -- клиническая группа
                           ,convert(nvarchar(255), n.D_ObsSDate, 104) as DNabl  -- дата последнего наблюдения пациента
                           ,cast(n.Sost as nvarchar(255)) as Sost               -- состояние пациента
                    from dbo.kr_Pasport as pas
                    left join dbo.Obsorg    as lpu                                -- справочник ЛПУ
                      on pas.ObserverCD = lpu.CD
                    left join dbo._Sick        as sik                                -- причина смерти
                      on pas.DeathCauseCD = sik.CD and pas.DeathCauseCD > 0
                    left join dbo._SVoc        as svU                                -- причина снятия с учета
                      on pas.UnRegTp = svU.CD and pas.UnRegTp >= 0 and svU.SVocCD = 49
                    left join dbo._SVoc        as svK                                -- клиническая группа
                      on pas.ClinicalGroup = svK.CD and pas.ClinicalGroup >= 0 and svK.SVocCD = 57
                    left join (select top(1) o.PatientCD, o.D_ObsSDate, svV.Text as Sost
                               from dbo.Obs as o
                               left join dbo._SVoc    as svV                    -- состояние пациента
                                 on o.ObsState = svV.CD and o.ObsState >= 0 and svV.SVocCD = 23
                               where o.PatientCD = @CD
                               order by  o.D_ObsSDate desc) as n
                      on n.PatientCD = pas.CD
                    where pas.CD = @CD) as s
                unpivot (Value for NameKey in (DSmert, Death, Adres, DReg, DUnReg, Snjat, Lpu, KlGr, DNabl, Sost)
                ) un
                union  -- ДИАГНОЗЫ
                select '2_Диагноз' as Tip, DsCD as Cod, NameKey, Value
                from (
                    select row_number() over(order by d.DsCD) as DsCD           -- порядковый номер диагноза
                          ,convert(nvarchar(255), d.D_DsSDate, 104) as DDate    -- дата установки диагноза
                          ,cast(iif(Main = 1, '(основной) ', '') + sik.Text as nvarchar(255)) as MKB -- диагноз из МКБ-10
                          ,cast(mor.Text as nvarchar(255)) as Morph             -- морфологический диагноз
                          ,cast(svS.Text as nvarchar(255)) as Stage             -- стадия
                          ,convert(nvarchar(255), d.D_AddSDate, 104) as DUp     -- когда были последние изменения записи
                          ,cast(us.UFio as nvarchar(255)) as Users              -- кто последний менял запись
                    from dbo.DS as d
                    left join dbo._Sick        as sik                                -- МКБ-10
                      on d.[Top] = sik.CD and d.[Top] > 0
                    left join dbo._Morph    as mor                                -- морфологический диагноз
                      on d.Morph = mor.CD and d.Morph > 0
                    left join dbo._SVoc        as svS                                -- стадия
                      on d.Stage = svS.CD and d.Stage >= 0 and svS.SVocCD = 36
                    left join dbo.UserInfo    as us                                -- пользователь
                      on d.UserCD = us.CD and d.UserCD >= 0
                    where d.PatientCD = @CD) as s
                unpivot (Value for NameKey in (DDate, MKB, Morph, Stage, DUp, Users)
                ) un
                union  -- ОПЕРАЦИИ
                select '3_Операции' as Tip, OperCD as Cod, NameKey, Value
                from (
                    select row_number() over(order by o.OperCD) as OperCD       -- порядковый номер операции
                        ,convert(nvarchar(255), o.D_OperSDate, 104) as DDate    -- дата проведения операции
                        ,cast(op.Text as nvarchar(255)) as Oper                 -- код операции
                        ,cast(lpu.Name as nvarchar(255)) as OpLpu               -- ЛПУ где провели операцию
                    from dbo.Oper as o
                    left join dbo._Oper    as op                                    -- справочник операций
                      on o.Oper = op.CD and o.Oper > 0
                    left join dbo.Obsorg as lpu                                    -- ЛПУ (не учитываем наши ЛПУ с кодом 1 и 2)
                      on o.OperWhere = lpu.CD and o.OperWhere > 2
                    where o.PatientCD = @CD) as o
                unpivot (Value for NameKey in (DDate, Oper, OpLpu)
                ) un
                union  -- ЛУЧЕВОЕ ЛЕЧЕНИЕ
                select '4_Лучевое' as Tip, RayCD as Cod, NameKey, Value
                from (
                    select row_number() over(order by RayCD) as RayCD           -- порядковый номер лечения
                        ,convert(nvarchar(255), r.D_RaySDate, 104) as DDate     -- дата начала лечения
                        ,convert(nvarchar(255), r.D_RayEndSDate, 104) as DKRay  -- дата окончания лечения
                        ,cast(svV.Text as nvarchar(255)) as VidRay              -- вид лучевого лечения
                        ,cast(lpu.Name as nvarchar(255)) as RayLpu              -- ЛПУ где провели лучевое лечение
                    from dbo.Ray as r
                    join dbo.kr_Pasport as pas
                      on r.PatientCD = pas.CD
                    left join dbo._SVoc    as svV                                    -- вид лучевого лечения
                      on r.RayKind = svV.CD and r.RayKind >= 0 and svV.SVocCD = 16
                    left join dbo.Obsorg as lpu                                    -- ЛПУ (не учитываем наши ЛПУ с кодом 1 и 2)
                      on r.RayWhere = lpu.CD and r.RayWhere > 2
                    where r.PatientCD = @CD) as r
                unpivot (Value for NameKey in (DDate, DKRay, VidRay, RayLpu)
                ) un
                union  -- ХИМИОТЕРАПИЯ
                select '5_Химия' as Tip, ChemCD as Cod, NameKey, Value
                from (
                    select row_number() over(order by ChemCD) as ChemCD         -- порядковый номер химиотерапии
                        ,convert(nvarchar(255), c.D_ChemSDate, 104) as DDate    -- дата начала лечения
                        ,convert(nvarchar(255), c.D_ChemEndSDate, 104) as DKChem  -- дата окончания лечения
                        ,cast(svV.Text as nvarchar(255)) as VidChem             -- вид
                        ,cast(lpu.Name as nvarchar(255)) as ChemLpu             -- ЛПУ где провели лечение
                    from dbo.Chem as c
                    left join dbo._SVoc    as svV                                    -- вид
                      on c.ChemKind = svV.CD and c.ChemKind >= 0 and svV.SVocCD = 22
                    left join dbo.Obsorg as lpu                                    -- ЛПУ (не учитываем наши ЛПУ с кодом 1 и 2)
                      on c.ChemWhere = lpu.CD and c.ChemWhere > 2
                    where c.PatientCD = @CD) as r
                unpivot (Value for NameKey in (DDate, DKChem, VidChem, ChemLpu)
                ) un
                union  -- ГОРМОНОТЕРАПИЯ
                select '6_Гормоны' as Tip, HormCD as Cod, NameKey, Value
                from (
                    select row_number() over(order by HormCD) as HormCD         -- порядковый номер
                        ,convert(nvarchar(255), h.D_HormSDate, 104) as DDate    -- дата начала лечения
                        ,convert(nvarchar(255), h.D_HormEndSDate, 104) as DKHorm -- дата окончания лечения
                        ,cast(svV.Text as nvarchar(255)) as VidHorm             -- вид
                        ,cast(lpu.Name as nvarchar(255)) as HormLpu             -- ЛПУ где провели лечение
                    from dbo.Horm as h
                    left join dbo._SVoc    as svV                                    -- вид
                      on h.HormKind = svV.CD and h.HormKind >= 0 and svV.SVocCD = 21
                    left join dbo.Obsorg as lpu                                    -- ЛПУ (не учитываем наши ЛПУ с кодом 1 и 2)
                      on h.HormWhere = lpu.CD and h.HormWhere > 2
                    where h.PatientCD = @CD) as r
                unpivot (Value for NameKey in (DDate, DKHorm, VidHorm, HormLpu)
                ) un";
            return _Query;
        }

        /// <summary>Ошибки Стационара</summary>
        public static string MET_varErrorStac_Select_1(int Otd, int User)
        {
            string _Query = $@"
                use Bazis;

                declare @User  as int = {User}; -- 104;
                declare @Otd  as int = {Otd}; -- 5;

                select *
                from (
                select  dbo.jsonValStr(Message, 'FIO') as FIO
                        ,dbo.jsonValStr(Message, 'DR') as DR
                        ,dbo.jsonValStr(Message, 'D') as D
                        ,dbo.jsonValStr(Message, 'DN') as DN
                        ,dbo.jsonValStr(Message, 'DK') as DK
                        ,dbo.jsonValInt(Message, 'KV') as KV
                        ,dbo.jsonValInt(Message, 'UET3') as UET3
                        ,dbo.jsonValStr(Message, 'TKOD') as TKOD
                        ,dbo.jsonValStr(Message, 'Desk') as Desk
                        ,cast(dbo.jsonValStr(Message, 'KL') as decimal(19)) as KL
                        ,Otd
                        ,cast(dbo.jsonValStr(Message, 'IND') as decimal(19)) as IND
                        ,iif(UserCod = @User, 1, 0) as Us -- 1 - текущий пользователь/врач, 0 - остальные врачи этого отделения
                from dbo.MessageHistory as m
                where TypeMessage between 1 and 99 and getdate() between xBeginDate and xEndDate
                    and (Otd = @Otd or UserCod = @User)) as f
                order by Us desc, TKOD, FIO";
            return _Query;
        }

        /// <summary>Справочник телефонов</summary>
        public static string MET_varPhone_Select_1()
        {
            string _Query = @"
                select Find, FIO, Post, Korpus, Division, Cabinet, LocalPhone, CityPhone, IPPhone, MobilPhone
                from (
                    select *
                        ,concat(FIO,'|',Post,'|',Korpus,'|',Cabinet,'|',LocalPhone,'|',CityPhone,'|',IPPhone,'|',MobilPhone) as Find
                    from (
                        select Bazis.dbo.GetFIO(p.FIO, p.Name, p.Subname) as FIO
                              ,isnull(po.Name, '') as Post
                              ,isnull(k.Name, '') as Korpus
                              ,isnull(d.Name, '') as Division
                              ,iif(isnull(p.CabinetNum, '') <> '', '№ ' + p.CabinetNum + ' ', '') + p.CabinetName as Cabinet
                              ,isnull(LocalPhone, '') as LocalPhone
                              ,isnull(CityPhone, '') as CityPhone
                              ,isnull(IPPhone, '') as IPPhone
                              ,isnull(MobilPhone, '') as MobilPhone
                        from Phone.dbo.Phones as p
                        left join Phone.dbo.Korpusa as k
                          on p.KorpusID = k.ID
                        left join Phone.dbo.Divisions as d
                          on p.DivisionID = d.ID
                        left join Phone.dbo.Posts as po
                          on p.PostID = po.ID
                    ) as a
                ) as b
                order by Korpus, Division, FIO";
            return _Query;
        }

        /// <summary>Подразделения телефонов</summary>
        public static string MET_varPhone_Select_2()
        {
            string _Query = @"
                select
                    Name
                from Phone.dbo.Divisions as d
                order by Name";
            return _Query;
        }

        /// <summary>Справочник сотрудников</summary>
        public static string MET_varStaff_Select_1()
        {
            string _Query = @"
                -- Пользователи s_Users
                select FIO
                      ,concat('Cod: ', Cod, char(13),
                              'KL: ', KL, char(13),
                               iif(KL > 0, 'ФИО: ' + dbo.GetFIObyKL(KL) + char(13), ''),
                              'Password: ', Password,
                              replace(cast((
                                  select
                                     char(13) + concat(Cod, '. ',
                                                       Names, char(13),
                                                       '   ', 'xInfo: ', replace(replace(xInfo, char(10), ''), char(13), ''),
                                                       iif(isWork = 1, char(13) + '   ' + 'Запись: удалена', ''))
                                  from dbo.s_UsersDostup as ud
                                  where UserCod = u.Cod
                                  for xml path(''), type) as nvarchar(max)), '&#x0D;', char(13))) as Tegs
                        ,iif(isWork = 1, 'удален', '') as isWork
                        ,'2 - пользователи' as Tips
                from dbo.s_Users as u
                union
                -- Врачи поликлиники s_VrachPol
                select vp.TKOD as FIO
                        ,concat('Cod: ', vp.KOD, char(13),
                            'Подразделение: ', vp.Podrazd, ' - ', dep.TKOD, char(13),
                            'Должность (DOLG): ', DOLG, ' - ', dol.TKOD, char(13),
                            'Специальность (SPC): ', SPC, ' - ', spc.TKOD, char(13),
                            'Профиль кабинета (SPCS): ', SPCS, ' - ', pr.Name, char(13),
                            'Код специальности (PRVS): ', iif(PRVS is not null and PRVS <> '', PRVS, 'пусто'), char(13),
                            'IDDOKT: ', iif(IDDOKT is not null and IDDOKT <> '', IDDOKT, 'пусто') , char(13),
                            'Реестр: ', iif(IsNotReestr = 1,  'не подавать', 'подавать')) as Tegs
                        ,iif(NU = 0, 'удален', '') as isWork   -- 0-уволена, 1-работает
                        ,'3 - врач поликлиники' as Tips
                from dbo.s_VrachPol as vp
                left join dbo.s_SpecVrPol as spc    on vp.SPC = spc.KOD
                left join dbo.s_DolVrach as dol     on vp.DOLG = dol.KOD
                left join dbo.s_ProfPol as pr       on vp.SPCS = pr.Cod
                left join dbo.z_Podrazd as dep      on vp.Podrazd = dep.Cod
                union
                -- Врачи стационара s_VrachStac
                select vs.TKOD as FIO
                        ,concat('Cod: ', vs.KOD, char(13),
                            'Отделение: ', vs.OTD, ' - ', dep.Names, char(13),
                            'Код специальности 0 (PRVS): ', iif(PRVS is not null and PRVS <> '', PRVS, 'пусто'), char(13),
                            iif(PRVS_1 is not null and PRVS_1 <> '', concat('Код специальности 1 (PRVS): ', PRVS_1, char(13)), ''),
                            iif(PRVS_2 is not null and PRVS_2 <> '', concat('Код специальности 2 (PRVS): ', PRVS_2, char(13)), ''),
                            iif(PRVS_3 is not null and PRVS_3 <> '', concat('Код специальности 3 (PRVS): ', PRVS_3, char(13)), ''),
                            iif(PRVS_4 is not null and PRVS_4 <> '', concat('Код специальности 4 (PRVS): ', PRVS_4, char(13)), ''),
                            'IDDOKT: ', iif(IDDOKT is not null and IDDOKT <> '', IDDOKT, 'пусто')) as Tegs
                        ,iif(Dismissed = 1, 'удален', '') as isWork   -- 1-уволена, 0-работает
                        ,'4 - врач стационара' as Tips
                from dbo.s_VrachStac as vs
                left join dbo.s_Department as dep      on vs.OTD = dep.Cod
                union
                -- Медсёстры
                select Name as FIO
                      ,concat('Cod: ', n.Cod, char(13),
                            'Подразделение: ', n.Podrazd, ' - ', dep.TKOD) as Tegs
                      ,'' as isWork
                      ,'5 - медсёстры' as Tips
                from dbo.s_Nurse as n
                left join dbo.z_Podrazd as dep      on n.Podrazd = dep.Cod
                where n.Cod not in (0, 100)
                union
                -- Телефоны
                select p.FIO
                    ,concat(iif(isnull(po.Name, '') = '', '', concat('Должность: ', po.Name, char(13))),
                            'Корпус: ', isnull(k.Name, ''),
                            iif(isnull(d.Name, '') = '', '', concat(char(13), 'Подразделение: ', d.Name)), char(13),
                            'Кабинет: ', iif(isnull(p.CabinetNum, '') <> '', '№ ' + p.CabinetNum + ' ', '') + p.CabinetName,
                            iif(isnull(LocalPhone, '') = '', '', concat(char(13), 'Телефон местный: ', LocalPhone)),
                            iif(isnull(CityPhone, '') = '', '', concat(char(13), 'Телефон городской: ', CityPhone)),
                            iif(isnull(IPPhone, '') = '', '', concat(char(13), 'Телефон IP: ', IPPhone)),
                            iif(isnull(MobilPhone, '') = '', '', concat(char(13), 'Телефон мобильный: ', MobilPhone))) as Tegs
                    ,'' as isWork
                    ,'1 - телефоны' as Tips
                from Phone.dbo.Phones as p
                left join Phone.dbo.Korpusa as k    on p.KorpusID = k.ID
                left join Phone.dbo.Divisions as d  on p.DivisionID = d.ID
                left join Phone.dbo.Posts as po     on p.PostID = po.ID
                where p.FIO <> ''
                order by FIO, Tips";
            return _Query;
        }

        /// <summary>Считаем время госпитализации для Направлений (для Variables)</summary>
        /// <param name="pOtd">Отделение стационара</param>
        /// <param name="pDate">Дата направления в стационар</param>
        public static string MET_varTimeGosp(int pOtd, string pDate)
        {
            string _Query = $@"
                -- C 8:30 прибавляем по 10 минут
                -- Если в очереди больше 42 человек, то начинаем отсчет сначала с 8:30 ( до 15:30)
                use Bazis;
                select convert(varchar(5), dateadd(mi, 10 * (count(*)%42), datetimefromparts(2017, 1, 1, 8, 30, 0, 0)), 108) as times
                      ,count(*) as cou
                from (
                    select KL, dbo.GetPole(4, p.Protokol) as d, try_cast(left(dbo.GetPole(3, p.Protokol), 2) as real) as otd
                    from dbo.apaNProtokol as p
                    where p.NumShablon = 9906 and contains(p.Protokol, '\4#{pDate}') and isnull(p.xDelete, 0) = 0
                    union
                    select KL, dbo.GetPole(4, p.Protokol) as d, try_cast(left(dbo.GetPole(3, p.Protokol), 2) as real) as otd
                    from dbo.astProtokol as p
                    where p.NumShablon = 8014 and contains(p.Protokol, '\4#{pDate}') and isnull(p.xDelete, 0) = 0
                    ) as p
                join dbo.s_Department as d
                  on p.otd = d.Cod
                join dbo.s_Department as d1
                  on d1.Cod = {pOtd} and d.Korpus = d1.Korpus
                where p.otd <> 15";
            return _Query;
        }

        /// <summary>Поиск пациента из kbol, APAC, APSTAC для перехода к редактированию из окна поиска пациентов</summary>
        public static string MET_varFindPac_Select_1(string pFIO, string pCod, string pTip, string pD1, string pD2)
        {

            string _SqlTip;
            string _SqlWhere;

            switch (pTip)
            {
                case "1":   // поликлиника APAC
                    if (pCod != null)
                        _SqlWhere = $"k.KL = {pCod} or a.Cod = {pCod} or a.SN like '{pCod}[нвуэ]' or a.SN = '{pCod}'";
                    else
                        _SqlWhere = $@"
                                len(@F) > 0
                            and ((k.FAM like @F + '%'
                            and ((@I <> '' and k.I like @I + '%') or @I = '')
                            and ((@O <> '' and k.O like @O + '%') or @O = '')
                            and ((@DR <> '' and cast(year(k.DR) as nvarchar(4)) like '%' + @DR + '%') or @DR = '')
                            and a.DP between @D1 and @D2)
                                or a.SN = @F)";

                    _SqlTip = $@"
                        select top 50 k.KL, dbo.GetFIO(k.FAM, k.I, k.O) as Fam, cast(DR as date) as DR, cast(a.DP as date) as D1,
                            cast(null as date) as D2, a.D, v.TKOD as Vr, cast(a.Cod as decimal) as IND, ''  as Otd
                        from dbo.kbol as k with (nolock)
                        join dbo.APAC as a with (nolock)  on k.KL = a.KL
                        left join dbo.s_VrachPol as v     on a.KV = v.KOD
                        where {_SqlWhere}
                            and isnull(k.xDelete, 0) = 0
                            and isnull(a.xDelete, 0) = 0";
                    break;
                case "2":   // стационар APSTAC
                    if (pCod != null)
                        _SqlWhere = $"k.KL = {pCod} or a.IND = {pCod} or a.SNEnd like '{pCod}[нвуэ]' or a.SNEnd = '{pCod}'";
                    else
                        _SqlWhere = $@"
                                len(@F) > 0
                            and ((k.FAM like @F + '%'
                            and ((@I <> '' and k.I like @I + '%') or @I = '')
                            and ((@O <> '' and k.O like @O + '%') or @O = '')
                            and ((@DR <> '' and cast(year(k.DR) as nvarchar(4)) like '%' + @DR + '%') or @DR = '')
                            and (a.DN <= @D2 and (a.DK > @D1 or a.DK is null)))
                                or a.SNEnd = @F) ";

                    _SqlTip = $@"
                        select top 50 k.KL, dbo.GetFIO(k.FAM, k.I, k.O) as Fam, cast(DR as date) as DR, cast(a.DN as date) as D1, cast(a.DK as date) as D2, a.D, v.TKOD as Vr
                            , a.IND, cast(a.otd as nvarchar(200)) as Otd
                        from dbo.kbol as k with (nolock)
                        join dbo.APSTAC as a with (nolock)  on k.KL = a.KL
                        left join dbo.s_VrachStac as v      on a.KV = v.KOD
                        where {_SqlWhere}
                            and isnull(k.xDelete, 0) = 0
                            and isnull(a.xDelete, 0) = 0";
                    break;
                case "3":   // параклиника parObsledov
                    if (pCod != null)
                        _SqlWhere = $"k.KL = {pCod} or p.Cod = {pCod} or k.SN like '{pCod}[нвуэ]' or k.SN = '{pCod}'";
                    else
                        _SqlWhere = $@"
                                len(@F) > 0
                            and ((k.FAM like @F + '%'
                            and ((@I <> '' and k.I like @I + '%') or @I = '')
                            and ((@O <> '' and k.O like @O + '%') or @O = '')
                            and ((@DR <> '' and cast(year(k.DR) as nvarchar(4)) like '%' + @DR + '%') or @DR = ''))
                            and p.pDate between @D1 and @D2
                                or k.SN = @F)";

                    _SqlTip = $@"
                        select top 50 k.KL, dbo.GetFIO(k.FAM, k.I, k.O) as Fam, cast(DR as date) as DR, cast(p.pDate as date) as D1,
                            cast(null as date) as D2, iif(isjson(i.jTag) > 0, json_value(i.jTag, '$.Diag'), '') as D, u.FIO as Vr, cast(o.Cod as decimal) as IND, s.NameKr as Otd
                        from dbo.parProtokol    as p with (nolock)
                        join dbo.parObsledov    as o with (nolock)    on p.CodApstac = o.Cod
                        join dbo.kbol            as k with (nolock)    on p.KL = k.KL
                        left join dbo.kbolInfo    as i with (nolock)    on i.Tab = 'par' and o.Cod = i.CodZap
                        join dbo.parListShablon as s                on p.NumShablon = s.Cod
                        join dbo.s_Users        as u                on u.Cod = p.xUserUp
                        where {_SqlWhere}
                            and isnull(p.xDelete, 0) = 0
                            and isnull(k.xDelete, 0) = 0";
                    break;
                default:   // 0 - пациент kbol
                    if (pCod != null)
                        _SqlWhere = $"k.KL = {pCod} or (k.SN like '{pCod}[нвуэ]') or k.SN = '{pCod}'";
                    else
                        _SqlWhere = $@"
                                len(@F) > 0
                            and ((k.FAM like @F + '%'
                            and ((@I <> '' and k.I like @I + '%') or @I = '')
                            and ((@O <> '' and k.O like @O + '%') or @O = '')
                            and ((@DR <> '' and cast(year(k.DR) as nvarchar(4)) like '%' + @DR + '%') or @DR = ''))
                                or k.SN = @F)";

                    _SqlTip = $@"
                        select top 50 k.KL, dbo.GetFIO(k.FAM, k.I, k.O) as Fam, cast(DR as date) as DR, cast(DataNew as date) as D1,
                            cast(DSmerti as date) as D2, cast(null as nvarchar(10)) as D, cast(null as nvarchar(10)) as Vr, cast(0 as decimal) as IND, '' as Otd
                        from dbo.kbol as k with (nolock)
                        where {_SqlWhere}";
                    break;
            }

            string _Query = $@"
                    use Bazis;

                    declare @Fio as nvarchar(50) = '{pFIO}';

                    -- Удаляем 2е, 3е, 4е пробелы между словами (ну вдруг пользователь разойдется)
                    set @Fio = replace(@Fio, '  ', ' ');
                    set @Fio = replace(@Fio, '  ', ' ');
                    set @Fio = replace(@Fio, '  ', ' ');

                    declare @F as nvarchar(50);
                    declare @I as nvarchar(50) = '';
                    declare @O as nvarchar(50) = '';
                    declare @DR as nvarchar(50) = '';
                    declare @D1 as date = '{pD1}';
                    declare @D2 as date = '{pD2}';
                    declare @n as int;

                    -- Находим первые символы Фамилии, Имени, Отчества, и цифры даты рождения
                    set @n = charindex(' ', @Fio);
                    if @n = 0
                        set @F = @Fio;                              -- Если только фамилия
                    else begin
                        set @F = left(@Fio, @n - 1);
                        set @Fio = stuff(@Fio, 1, @n, '')           -- Фамилия, но ещё имя есть
                        set @n = charindex(' ', @Fio);
                        if @n = 0
                            set @I = @Fio;                          -- Если только имя
                        else begin
                            set @I = left(@Fio, @n - 1);
                            set @Fio = stuff(@Fio, 1, @n, '')       -- Имя, но ещё отчество есть
                            set @n = charindex(' ', @Fio);
                            if @n = 0
                                set @O = @Fio;                      -- Если только отчество
                            else begin
                                set @O = left(@Fio, @n - 1);
                                set @Fio = stuff(@Fio, 1, @n, '')   -- Отчество, но ещё год рождения есть
                                set @n = charindex(' ', @Fio);
                                if @n = 0
                                    set @DR = @Fio;                 -- Всё цифры даты рождения
                                else begin
                                    set @DR = left(@Fio, @n - 1);   -- Ан нет, ещё какуют то фигню после даты рождения добавили
                                end
                            end
                        end
                    end;

                    {_SqlTip}
                    order by FAM, DR, D1 ";
            return _Query;
        }

        /// <summary>Cправочник врачей из МИАЦ (для проверки действующих сертефикатов, только наше ЛПУ 555509)</summary>
        public static string MET_StrahVrachMIAC_Select_1()
        {
            string _Return = $@"
                select distinct concat(family, ';', [name], ';', father, ';', right(year(vozrast), 2), ';',iddokt) as Filter
                    ,iddokt
                    ,dbo.GetFIO(family, [name], father) as FIO
                    ,vozrast
                    ,prvs
                    ,data_n
                    ,data_e
                from dbo.StrahVrachMIAC";
            return _Return;
        }
        #endregion

        #region ---- Регистратура в поликлинике ----
        /// <summary>Выбор Элемента расписания</summary>
        public static string MET_RnElement_Select_1(string pServer, int pDepatment)
        {
             string _Query = $@"
                select * from openquery({pServer},
                    'select   Cod
                            ,Prefix
                            ,ElementName
                    from Bazis.dbo.RnElement
                    where getdate() between xBeginDate and xEndDate and Department = {pDepatment}
                    order by try_cast(Prefix as int)')";
             return _Query;
        }

        /// <summary>Выбор даты расписания</summary>
        public static string MET_RnSetka_Date_Select_1(int Element, string pServer)
        {
             string _Query = $@"
                select * from openquery({pServer},
                    'select distinct s.[Date] as Dat,
                        case datepart(dw, s.[Date])
                            when 1 then ''ВС''
                            when 2 then ''ПН''
                            when 3 then ''ВТ''
                            when 4 then ''СР''
                            when 5 then ''ЧТ''
                            when 6 then ''ПТ''
                            when 7 then ''СБ''
                        end   as DN
                    from Bazis.dbo.RnSetka as s
                    left join Bazis.dbo.RnTalon as t
                        on s.Cod = t.CodSetka and isnull(t.xDelete, 0) = 0
                    where s.CodElement = {Element} and s.[Date] > getdate() and s.xInfo = '''' and isnull(s.xDelete, 0) = 0 and t.Cod is null
                    order by s.[Date]')";
             return _Query;
        }

        /// <summary>Выбор времени расписания</summary>
        public static string MET_RnSetka_Time_Select_1(int Element, DateTime? Date, string pServer = "")
        {
            string _Query = $@"
                select * from openquery({pServer},
                    'select  s.Cod
                            ,cast(s.BeginTime as nvarchar(5)) as Times
                    from Bazis.dbo.RnSetka as s
                    left join Bazis.dbo.RnTalon as t
                      on s.Cod = t.CodSetka and isnull(t.xDelete, 0) = 0
                    where s.CodElement = {Element} and s.[Date] = ''{Date:MM.dd.yyyy}'' and s.xInfo = '''' and isnull(s.xDelete, 0) = 0 and t.Cod is null')";
            return _Query;
        }

        /// <summary>Проверка, свободно ли время расписания</summary>
        public static string MET_RnSetka_Time_Select_2(int Cod, string pServer)
        {
            string _Query = $@"
                select * from openquery({pServer},
                    'select  1
                    from Bazis.dbo.RnSetka as s
                    left join Bazis.dbo.RnTalon as t
                      on s.Cod = t.CodSetka and isnull(t.xDelete, 0) = 0
                    where s.Cod = {Cod} and t.Cod is null')";
            return _Query;
        }

        /// <summary>Проверка, записан ли пациент в этот кабинет</summary>
        public static string MET_RnSetka_Time_Select_3(decimal KL, int Element, string pServer = "")
        {
            string _Query = $@"
                select * from openquery({pServer},
                    'select  1
                    from Bazis.dbo.RnSetka as s
                    left join Bazis.dbo.RnTalon as t
                      on s.Cod = t.CodSetka and isnull(t.xDelete, 0) = 0
                    where t.KL = {KL} and s.CodElement = {Element} and s.[Date] > GETDATE()')";
            return _Query;
        }

        /// <summary>Смотрим куда записан пациент</summary>
        public static string MET_RnSetka_Time_Select_4(decimal KL, string pServer)
        {
            string _Query = $@"
                select * from openquery({pServer},
                    'select  t.Cod, e.Prefix as Kabinet, e.ElementName, s.Date as Dat, cast(s.BeginTime as nvarchar(5)) as Times, t.xLog
                    from Bazis.dbo.RnSetka as s
                    join Bazis.dbo.RnTalon as t
                      on s.Cod = t.CodSetka and isnull(t.xDelete, 0) = 0
                    join Bazis.dbo.RnElement as e
                      on s.CodElement = e.Cod and isnull(e.xDelete, 0) = 0
                    where t.KL = {KL} and s.Date >= getdate() and isnull(s.xDelete, 0) = 0
                    order by try_cast(e.Prefix as int)')";
            return _Query;
        }

        /// <summary>Записываем пациента на прием в RтTalon</summary>
        public static string MET_RnTalon_Insert_1(int Cod, int CodSetka, decimal KL, string FIO, DateTime? DR, string Rai, string xLog, string pServer)
        {
            string _Query = $@"
                insert {pServer}.Bazis.dbo.RnTalon
                    (Cod, CodSetka, CodTabl, KL, FIO, DR, Raion, xLog, xInfo)
                values
                    ({Cod}, {CodSetka}, 0, {KL}, '{FIO}', '{DR:yyyy-MM-dd}', '{Rai}', '{xLog}', '{{ ""wpfdoc"": """" }}')";
            return _Query;
        }
        #endregion

        #region ---- Json ----
        /// <summary>Выборка тегов</summary>
        /// <param name="pTables">Кто создал/изменил протокол</param>
        /// <param name="pDop_Filter">Тип лога Создан, Изменён, Удалён</param>
        public static string MET_s_Tags_Select_1(string pTables, string pDop_Filter = "")
        {
            string _Query = $@"
                use Bazis;

                declare @Tables as nvarchar(50) = '{pTables}';          --'kbolInfo';
                declare @Dop_Filter as nvarchar(50) = '{pDop_Filter}';  --'stac';

                select Tag
                      ,TagName
                      ,Discription
                      ,TypeTag
                      ,xInfo
                      ,xBeginDate
                      ,xEndDate
                      ,xLog
                from dbo.s_Tags
                where [Tables] = @Tables
                   and GetDate() between xBeginDate and xEndDate
                   and (@Dop_Filter = ''
                        or (@Dop_Filter <> '' and isjson(xInfo) > 0) and exists(select * from openjson(xInfo, '$.If_Filter.Dop_Filter') where value = @Dop_Filter)
                        )";
            return _Query;
        }
        #endregion
    }
}