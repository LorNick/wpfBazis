namespace wpfReestr
{
    /// <summary>КЛАСС для вывода SQL Запросов</summary>
    /// <remarks>Файл MyQuery
    /// <para>Класс используется только для запросов SQL</para></remarks>
    public static class MyQuery
    {
        #region ---- StrahFile (Страховые файлы) ----
        /// <summary>Загрузка Файлов реестров</summary>
        public static string StrahFile_Select_1()
        {
            string _Return = @"
                  select f.Cod 
                        ,f.DateN 
                        ,f.DateK
                        ,f.YEAR
                        ,f.MONTH
                        ,f.Korekt 
                        ,f.NSCHET 
                        ,f.DSCHET
                        ,case f.StrahComp
                            when 0 then 'Все Областные'
                            when 1 then 'Альфа (50)'
                            when 2 then 'Капитал МС (41)'
                            when 3 then 'ВТБ МС (46)'
                            when 4 then 'Иногородние'
                        end as StrahComp 
                        ,case f.VMP
                            when 0 then 'без ВМП'
                            when 1 then 'ВМП'
                            when 2 then 'общий' 
                            when 3 then 'ЗНО' 
                            when 4 then 'без С'                        
                        end as VMP                       
                        ,f.SUMMAV
                        ,pPaket
                        ,'pDate' = convert(nvarchar(11),f.pDate, 104) + ' (' + convert(nvarchar(5),f.pDate, 108) + ')'            
                        ,'pUpDate' = convert(nvarchar(11),f.pUpDate, 104) + ' (' + convert(nvarchar(5),f.pUpDate, 108) + ')'   
                        ,r.cou
                        ,pHide
                        ,pParent
                from dbo.StrahFile as f
                left join (select count(Cod) as cou, CodFile
                           from dbo.StrahReestr
                           group by CodFile) as r
                  on f.Cod = r.CodFile                
                order by f.Cod desc";
            return _Return;
        }

        /// <summary>Обновляем в файле Страховых файлов Сумму</summary>
        public static string StrahFile_Update_1(int Cod)
        {
            string _Return = $@"
                update dbo.StrahFile          
                    set SUMMAV = (select sum(isnull(SUM_LPU, 0) + isnull(DoTarif,0) + isnull(DoSumRea,0))        
                                  from dbo.StrahReestr          
                                  where CodFile ={Cod})
                where Cod = {Cod}";
            return _Return;
        }

        /// <summary>Выбор родителя, для исправленного реестра (для свитка PART_ComboBoxParent)</summary>
        public static string StrahFile_Select_2()
        {
            string _Return = $@"
                use Bazis;
                set language Russian;

                select 'Основной реестр'  as Visual, 99999 as Cod, 0 as StrahComp, 0 as VMP, cast(getdate() as date) as DateN, cast(getdate() as date) as DateK
                union
                select top (40) concat_ws(' - ' 
		                ,Cod                       
                        ,case VMP
                            when 0 then 'без ВМП'
                            when 1 then 'ВМП'
                            when 2 then 'общий' 
                            when 3 then 'ЗНО' 
                            when 4 then 'без С'                        
                        end
                        ,case StrahComp           
                            when 1 then 'Альфа (50)'
                            when 2 then 'Капитал МС (41)'
                            when 3 then 'ВТБ МС (46)'
                            when 4 then 'Иногородние'
                        end
                        ,datename(year, DateN)
		                ,datename(month, DateN)  
		                )  as Visual
		                ,Cod
                        ,StrahComp
		                ,VMP
		                ,DateN
		                ,DateK
                from dbo.StrahFile
                where StrahComp > 0 and NSCHET > 0 and pPaket > 0 and pParent = 0
                order by Cod desc";
            return _Return;
        }
        #endregion

        #region ---- StrahReestr (Страховые реестры) ----
        /// <summary>Загрузка Страховых реестров</summary>
        public static string StrahReestr_Select_1(int CodFile)
        {
            string _Return = $@"
                select [Cod]
                      ,[CodFile]                   
                      ,[PLAT]
                      ,[SMO_OGRN]
                      ,[SMO_OK]
                      ,[SMO_NAM]                        
                      ,[LPU_1]                  
                      ,[ORDER]
                      ,[LPU_ST]
                      ,[VIDPOM]
                      ,[PODR]
                      ,[PROFIL]
                      ,[DET]
                      ,[CODE_USL]
                      ,[PRVS]
                      ,[IDDOKT]
                      ,[ARR_DATE]
                      ,[EX_DATE]                 
                      ,[DS1]
                      ,[DS2]
                      ,[PACIENTID]
                      ,[RES_G]
                      ,[ISHOD]
                      ,[IDSP]
                      ,[KOL_USL]
                      ,[TARIF]
                      ,[DayN]
                      ,[DoTarif]
                      ,[SUM_LPU]
                      ,[DoSumRea]            
                      ,[VPOLIS]
                      ,[SERIA]
                      ,[NUMBER]
                      ,[FAMILY]
                      ,[NAME]
                      ,[FATHER]
                      ,[POL]
                      ,[VOZRAST]
                      ,[SS]                   
                      ,[OS_SLUCH]
                      ,[FAM_P]
                      ,[IM_P]
                      ,[OT_P]
                      ,[W_P]
                      ,[DR_P]
                      ,[MR]
                      ,[DOCTYPE]
                      ,[DOCSER]
                      ,[DOCNUM]
                      ,[OKATOG]                 
                      ,[NOM_ZAP]
                      ,[UKL]                     
                      ,[NOM_USL]
                      ,[ID_PAC]
                      ,[N_ZAP]
                      ,[PR_NOV]
                      ,[xUpdate]
                      ,VID_VME
                      ,[VID_HMP]
                      ,[METOD_HMP]
                      ,Kod_Ksg
                      ,DOCDATE
                      ,DOCORG
                from dbo.StrahReestr
                where CodFile = {CodFile}";
            return _Return;
        }

        /// <summary>Находим сумму поля (например - SUM_LPU) по фильтру в Страховых реестров</summary>
        public static string StrahReestr_Select_2(int CodFile, string pWhere, string pPole)
        {
            if (pWhere.Length > 0)
                pWhere += " and ";

            string _Return = $@"
                select isnull(sum([{pPole}]),0) as summa                                 
                from dbo.StrahReestr
                where {pWhere} CodFile = {CodFile}";
            return _Return;
        }

        /// <summary>Обновляем в файле Страховых реестров поле N_ZAP и NOM_ZAP (Только для ИСПРАВЛЕННЫХ файлов)</summary>
        /// <param name="pCodFile">Код реестра</param>
        /// <param name="pParent">Код основного родительского реестра</param>
        public static string StrahReestr_Update_1(int CodFile, int pParent)
        {
            string _Return = $@"
                use Bazis;

                declare @CodFile as int = {CodFile};
                declare @ParentMain as int = {pParent};
                declare @ParentTable as table (CodFile int);

                -- Определяем Номера реестров, которые могут быть родителями
                insert @ParentTable
                select s.Cod
                from dbo.StrahFile as s
                join (select *
	                  from dbo.StrahFile
	                  where Cod = @ParentMain
	                 ) as f
                  on s.YEAR = f.YEAR and s.MONTH = f.MONTH
                where s.StrahComp > 0 and s.pParent = 0 and s.NSCHET > 0 and s.pPaket > 0
	                and ((f.VMP = 1 and s.VMP = 1) or (f.VMP > 1 and s.VMP > 1))

                update st1        
                    set N_ZAP = st2.N_ZAP
                       ,NOM_ZAP = st2.NOM_ZAP 
                from dbo.StrahReestr              as st1             -- этот правим
                join (select *                                      
                      from dbo.StrahReestr                    
                      where CodFile in (select * from @ParentTable))  as st2          -- родители
                  on st1.PACIENTID = st2.PACIENTID                  
                where st1.CodFile = @CodFile";
            return _Return;
        }

        /// <summary>Обновляем в файле Страховых реестров поле N_ZAP и NOM_ZAP (Только для ОСНОВНОВНЫХ файлов)</summary>
        /// <param name="pCodFile">Код реестра</param>
        public static string StrahReestr_Update_2(int pCodFile)
        {
            string _Return = $@"
                declare @Start as int;
                select  @Start = min(N_ZAP) from dbo.StrahReestr where CodFile = {pCodFile};

                update dbo.StrahReestr        
                    set N_ZAP   = st2.N_ZAP 
                       ,NOM_ZAP = st2.NOM_ZAP                             
                from dbo.StrahReestr    as st1                
                join (select row_number() over (order by FAMILY, [NAME], FATHER, VOZRAST, PLAT, SERIA, NUMBER) + @Start - 1 as NOM_ZAP
                            ,row_number() over (order by FAMILY, [NAME], FATHER, VOZRAST, PLAT, SERIA, NUMBER) + @Start - 1 as N_ZAP   
                         -- потом вполне возможно притдется вернуть эту строку     
                         --   ,dense_rank() over (order by FAMILY, [NAME], FATHER, VOZRAST, PLAT, SERIA, NUMBER) + @Start - 1 as N_ZAP
                            ,Cod 
                      from dbo.StrahReestr                    
                      where CodFile = {pCodFile}) as st2      
                on st1.Cod = st2.Cod";
            return _Return;
        }

        /// <summary>Помечаем/Убираем выбранные записи реестров в поле xUpdate (например для печати)</summary>
        public static string StrahReestr_Update_4(int pUpdate, int CodFile, string pWhere)
        {
            string _Return = $@"
                update dbo.StrahReestr        
                    set xUpdate   = {pUpdate}                             
                where CodFile = {CodFile} {pWhere}";
            return _Return;
        }
        #endregion

        #region ---- XML (Выгрузка в XML) ----
        /// <summary>Выгрузка файла ЗНО ВЕРСИЯ 2019, приказ 285</summary>
        public static string StrahReestrXML_Select_Main_3(int CodFile, string pMainFileName)
        {
            string _Return = $@"
                    use Bazis;

            --- Заголовок ZGLV ---
            select
                '3.1'                                   as 'ZGLV/VERSION'
                ,convert(nvarchar(10), getdate(), 20)   as 'ZGLV/DATA'
                ,'{pMainFileName}'                      as 'ZGLV/FILENAME'					
                ,(select count(*) from dbo.StrahReestr where CodFile = sf.Cod) as 'ZGLV/SD_Z'
            --- Счет SCHET ---
                ,Cod        as 'SCHET/CODE'
                ,'555509'   as 'SCHET/CODE_MO'
                ,[YEAR]     as 'SCHET/YEAR'
                ,[MONTH]    as 'SCHET/MONTH'
                ,NSCHET     as 'SCHET/NSCHET'
                ,convert(nvarchar(10), DSCHET, 20) as 'SCHET/DSCHET'
                ,case StrahComp 
                    when 1 then '55050'
                    when 2 then '55041'
                    when 3 then '55046'
                    else null
                    end         as 'SCHET/PLAT'
                ,SUMMAV         as 'SCHET/SUMMAV'
            --- Запись ZAP ---  
                ,(select      
                        N_ZAP               as 'ZAP/N_ZAP' 
			            ,isnull(PR_NOV, 0)  as 'ZAP/PR_NOV'
            --- Пациент PACIENT ---
                        ,ID_PAC     as 'ZAP/PACIENT/ID_PAC'
                        ,VPOLIS     as 'ZAP/PACIENT/VPOLIS'
                        ,iif(SERIA = '', null, SERIA) as 'ZAP/PACIENT/SPOLIS'
                        ,NUMBER     as 'ZAP/PACIENT/NPOLIS'	
                        ,PLAT       as 'ZAP/PACIENT/SMO'
                        ,SMO_OK     as 'ZAP/PACIENT/SMO_OK'   		  
                        ,0          as 'ZAP/PACIENT/NOVOR'
            -- Законченный случай Z_SL ---
                        ,(select  
                             NOM_ZAP    as 'Z_SL/IDCASE'
                            ,iif(LPU_ST = 4, 3, LPU_ST)     as 'Z_SL/USL_OK'  -- для параклиники меняем 4 на 3
                            ,VIDPOM     as 'Z_SL/VIDPOM'
                            ,3          as 'Z_SL/FOR_POM'
                            ,json_value(NOM_USL, '$.NPR_MO')   as 'Z_SL/NPR_MO'                                 --- !!!
                            ,convert(nvarchar(10), convert(date, json_value(NOM_USL, '$.NPR_DATE'), 104 ), 20)  as 'Z_SL/NPR_DATE'    --- !!!
                            ,'555509'   as 'Z_SL/LPU'
                            ,convert(nvarchar(10), ARR_DATE, 20) as 'Z_SL/DATE_Z_1'
                            ,convert(nvarchar(10), EX_DATE, 20)  as 'Z_SL/DATE_Z_2'
                            ,iif(LPU_ST < 3, cast(KOL_USL as int), null)   as 'Z_SL/KD_Z'  -- только для стационара
                            ,RES_G      as 'Z_SL/RSLT'
                            ,ISHOD      as 'Z_SL/ISHOD'
                            ,iif(OS_SLUCH = '', null, OS_SLUCH) as 'Z_SL/OS_SLUCH'
                            --   ,iif(1 = 2, 1, null) as 'Z_SL/VB_P'                         -- !!! переводы между отделением
            --- Случай SL ---
                            ,(select
                                 1          as 'SL/SL_ID'
                                ,LPU_1      as 'SL/LPU_1'
                                ,PODR       as 'SL/PODR'
                                ,PROFIL     as 'SL/PROFIL'
                                ,json_value(NOM_USL, '$.PROFIL_K') as 'SL/PROFIL_K'        
                                ,DET        as 'SL/DET'
                                ,json_value(NOM_USL, '$.P_Cel') as 'SL/P_CEL'       
                                ,PACIENTID  as 'SL/NHISTORY'
                                ,iif(LPU_ST < 3, 1, null) as 'SL/P_PER'             -- !!! признак поступления/перевода стационара 1 или 4
                                ,convert(nvarchar(10), ARR_DATE, 20) as 'SL/DATE_1'
                                ,convert(nvarchar(10), EX_DATE, 20)  as 'SL/DATE_2'
                                ,iif(LPU_ST < 3, cast(KOL_USL as int), null) as 'SL/KD'
                                ,DS1        as 'SL/DS1'                                        
                                ,iif(DS2 = '', null, DS2)      as 'SL/DS2'
                                ,json_value(NOM_USL, '$.C_ZAB') as 'SL/C_ZAB'
                                ,iif(json_value(NOM_USL, '$.DS_ONK') = 1, 1, 0) as 'SL/DS_ONK'      
                                ,json_value(NOM_USL, '$.DN') as 'SL/DN'             --  диспансерное наблюдение
            --- БЛОК Направления к врачу околога при подозрении NAPR --- Убрал 29/04/2019
                             --   ,convert(nvarchar(10), EX_DATE, 20) as 'SL/NAPR/NAPR_DATE' 
                             --   ,1                                  as 'SL/NAPR/NAPR_V'
                --- БЛОК Направления на исследования добавил 17/12/2019 ---
							    ,(select convert(nvarchar(10), convert(date, NAPR_DATE, 104 ), 20) as NAPR_DATE, NAPR_V, MET_ISSL, NAPR_USL
								  from openjson(NOM_USL, '$.NAPR')
								  with (   
									NAPR_DATE varchar(10)	'$.NAPR_DATE' ,  
									NAPR_V    int			'$.NAPR_V',  
									MET_ISSL  int			'$.MET_ISSL',  
									NAPR_USL  nvarchar(15)  '$.NAPR_USL'  
									) 
								  for xml path('NAPR'), type) as 'SL' 
                --- БЛОК Консилиумы CONS ---  
                                ,(select 
                                        0                                   as 'CONS/PR_CONS'     -- нет консилиума  для параклиники                                                     
                                    where LPU_ST = 4 and dbo.jsonIf(NOM_USL, 'DS1_T') = 1
                                    for xml path(''), type ) as 'SL'
                                ,(select 
                                        0                                   as 'CONS/PR_CONS'     -- нет консилиума                                                       
                                    where dbo.jsonIf(NOM_USL, 'Taktika_1') = 0 and dbo.jsonIf(NOM_USL, 'Taktika_2') = 0 and dbo.jsonIf(NOM_USL, 'Taktika_3') = 0 and LPU_ST <> 4
                                    for xml path(''), type ) as 'SL'
                                ,(select 
                                        1                                   as 'CONS/PR_CONS'     -- противопоказания химии  
                                        ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_1'), 104 ), 20)   as 'CONS/DT_CONS'      
                                    where dbo.jsonIf(NOM_USL, 'Taktika_1') = 1
                                    for xml path(''), type ) as 'SL'
                                ,(select 
                                        2                                   as 'CONS/PR_CONS'     -- противопоказания лучевой  
                                        ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_2'), 104 ), 20)   as 'CONS/DT_CONS'      
                                    where dbo.jsonIf(NOM_USL, 'Taktika_2') = 1
                                    for xml path(''), type ) as 'SL'
                                ,(select 
                                        3                                   as 'CONS/PR_CONS'     -- отказ от хирургии  
                                        ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_3'), 104 ), 20)   as 'CONS/DT_CONS'      
                                    where dbo.jsonIf(NOM_USL, 'Taktika_3') = 1
                                    for xml path(''), type ) as 'SL'            
                --- БЛОК Онкологическое лечение ONK_SL ---
                                ,(select 
                                     iif(dbo.jsonIf(NOM_USL, 'DS1_T') = 1, dbo.jsonValInt(NOM_USL, 'DS1_T'), 0) as 'ONK_SL/DS1_T' 
                                    ,dbo.jsonValInt(NOM_USL, 'STAD')  as 'ONK_SL/STAD' 
                                    ,dbo.jsonValInt(NOM_USL, 'ONK_T') as 'ONK_SL/ONK_T'
                                    ,dbo.jsonValInt(NOM_USL, 'ONK_N') as 'ONK_SL/ONK_N'
                                    ,dbo.jsonValInt(NOM_USL, 'ONK_M') as 'ONK_SL/ONK_M'                      
                                    ,dbo.jsonValInt(NOM_USL, 'MTSTZ') as 'ONK_SL/MTSTZ'
                                    ,try_cast(dbo.jsonValReal(NOM_USL, 'SOD')  as decimal(6,2)) as 'ONK_SL/SOD'
                --- БЛОК Диагноститческий блок B_DIAG --- 
                                    ,(select 
                                            convert(date, DIAG_DATE, 104 )    as 'B_DIAG/DIAG_DATE'
                                        ,DIAG_TIP       as 'B_DIAG/DIAG_TIP' 
                                        ,DIAG_CODE      as 'B_DIAG/DIAG_CODE'
                                        ,DIAG_RSLT      as 'B_DIAG/DIAG_RSLT'
                                        ,1              as 'B_DIAG/REC_RSLT'                                              											
			                        from
			                        (select parent_id
					                        ,name
					                        ,stringvalue
			                        from dbo.jsonParse(NOM_USL)
			                        where object_id is null ) as Piv
			                        pivot
			                        (
			                        max(stringvalue)
			                        for name in (DIAG_DATE, DIAG_TIP, DIAG_CODE, DIAG_RSLT)
			                        ) as dd
			                        where LPU_ST < 3 and DIAG_TIP is not null
			                        for xml path(''), type ) as 'ONK_SL'
                --- БЛОК Противопоказания или отказы B_PROT ---  
                                    ,(select 
                                            1                                   as 'B_PROT/PROT'     -- противопоказания хирургии  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_1'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_1') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                                    ,(select 
                                            2                                   as 'B_PROT/PROT'     -- противопоказания химии  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_2'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_2') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                                    ,(select 
                                            3                                   as 'B_PROT/PROT'     -- противопоказания лучевой  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_3'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_3') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                                    ,(select 
                                            4                                   as 'B_PROT/PROT'     -- отказ от хирургии  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_4'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_4') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                                    ,(select 
                                            5                                   as 'B_PROT/PROT'     -- отказ от химии  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_5'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_5') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                                    ,(select 
                                            6                                   as 'B_PROT/PROT'     -- отказ от лучевой  
                                            ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_6'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                        where dbo.jsonIf(NOM_USL, 'PrOt_6') = 1
                                        for xml path(''), type ) as 'ONK_SL'
                --- БЛОК Онкологическое лечение ONK_USL ---
                                    ,(select
							             json_value(value, '$.USL_TIP') as 'ONK_USL/USL_TIP'
							            ,json_value(value, '$.HIR_TIP') as 'ONK_USL/HIR_TIP'
							            ,json_value(value, '$.LEK_TIP_L') as 'ONK_USL/LEK_TIP_L'
							            ,json_value(value, '$.LEK_TIP_V') as 'ONK_USL/LEK_TIP_V'
							            ,(select 
								            json_value(value, '$.REGNUM') as 'LEK_PR/REGNUM'
								            ,json_value(value, '$.CODE_SH') as 'LEK_PR/CODE_SH'
								            ,(select convert(nvarchar(10), convert(date, value, 104 ), 20) as DATE_INJ 
								             from openjson(value, '$.DATE_INJ')	for xml path(''), type) as 'LEK_PR'
							             from openjson(value, '$.LEK_PR')
							             for xml path(''), type) as 'ONK_USL'
							            ,json_value(value, '$.LUCH_TIP') as 'ONK_USL/LUCH_TIP' 
							            ,json_value(value, '$.PPTR') as 'ONK_USL/PPTR'
						            from openjson(NOM_USL, '$.ONK_SL.ONK_USL')
						            for xml path(''), type) as 'ONK_SL' 
	          --- Продолжаем блок Онкологическое лечение ONK_SL                                    
                                    ,iif(dbo.jsonIf(NOM_USL, 'K_FR') = 1, iif(dbo.jsonValInt(NOM_USL, 'K_FR') = -1, 0, dbo.jsonValInt(NOM_USL, 'K_FR')), null) as 'ONK_SL/K_FR'
                                    ,try_cast(dbo.jsonValReal(NOM_USL, 'WEI')  as decimal(4,1)) as 'ONK_SL/WEI'
                                    ,dbo.jsonValInt(NOM_USL, 'HEI')     as 'ONK_SL/HEI'
                                    ,try_cast(dbo.jsonValReal(NOM_USL, 'BSA')  as decimal(3,2)) as 'ONK_SL/BSA'
                                    where isjson(NOM_USL) > 0
	                                      and (json_value(NOM_USL, '$.ONK_SL.DS1_T') is not null 
                                                or json_value(NOM_USL, '$.ONK_SL.STAD') is not null)
                                    for xml path(''), type ) as 'SL'
            --- Услуга по КСГ для стационара KSG_KPG ---              
                                ,(select 
                                     json_value(NOM_USL, '$.KSG')  as 'KSG_KPG/N_KSG' 
                                    ,[YEAR]     as 'KSG_KPG/VER_KSG' 
                                    ,0          as 'KSG_KPG/KSG_PG'
                                    ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_Z') as decimal(8,5))   as 'KSG_KPG/KOEF_Z'     
                                    ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_UP') as decimal(8,5))  as 'KSG_KPG/KOEF_UP'
                                    ,TARIF      as 'KSG_KPG/BZTSZ'
                                    ,1.10500    as 'KSG_KPG/KOEF_D'     
                                    ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_U') as decimal(8,5))           as 'KSG_KPG/KOEF_U' 
						            ,(select value as CRIT from openjson(NOM_USL, '$.CRIT') for xml path(''), type) as 'KSG_KPG'                      
                                    ,iif(dbo.jsonValReal(NOM_USL, 'IT_SL') is not null, 1, 0)           as 'KSG_KPG/SL_K' 
                                    ,cast(dbo.jsonValReal(NOM_USL, 'IT_SL') as decimal(8,5))            as 'KSG_KPG/IT_SL'     
            --- КСЛП для стационара SL_KOEF ---
                                    ,(select 
                                                10      as 'SL_KOEF/IDSL'       
                                            ,cast(dbo.jsonValReal(NOM_USL, 'Sl10') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                        where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl10') = 1
                                        for xml path(''), type ) as 'KSG_KPG' 
                                    ,(select 
                                                13      as 'SL_KOEF/IDSL'       
                                            ,cast(dbo.jsonValReal(NOM_USL, 'Sl13') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                        where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl13') = 1
                                        for xml path(''), type ) as 'KSG_KPG' 
                                    ,(select 
                                                14      as 'SL_KOEF/IDSL'       
                                            ,cast(dbo.jsonValReal(NOM_USL, 'Sl14') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                        where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl14') = 1
                                        for xml path(''), type ) as 'KSG_KPG'                                                                                                                                                                                                                                
                                    where LPU_ST in (1, 2)
                                    for xml path(''), type ) as 'SL'
            --- продолжаем Случай SL ---
                                ,PRVS       as 'SL/PRVS'
                                ,'V021'     as 'SL/VERS_SPEC'
                                ,IDDOKT     as 'SL/IDDOKT'
                                ,1.00       as 'SL/ED_COL'
                                ,SUM_LPU    as 'SL/TARIF'
                                ,SUM_LPU    as 'SL/SUM_M'
            --- Услуга Поликлиники USL ---
                                ,(select 
                                    row_number() over (order by Cod) as 'USL/IDSERV' 
                                    ,'555509'   as 'USL/LPU' 
                                    ,LPU_1      as 'USL/LPU_1'
                                    ,PODR       as 'USL/PODR'
                                    ,PROFIL     as 'USL/PROFIL'
                                    ,Usl        as 'USL/VID_VME'
                                    ,DET        as 'USL/DET'
                                    ,convert(nvarchar(10), convert(date, DatN, 104 ), 20) as 'USL/DATE_IN'
                                    ,convert(nvarchar(10), convert(date, DatN, 104 ), 20)  as 'USL/DATE_OUT'            
                                    ,D          as 'USL/DS' 
                                    ,Code_Usl   as 'USL/CODE_USL'
                                    ,1.00       as 'USL/KOL_USL' 
                                    ,0.00       as 'USL/TARIF'   
                                    ,0.00       as 'USL/SUMV_USL'                   
                                    ,PRVS_Usl   as 'USL/PRVS' 
                                    ,MD         as 'USL/CODE_MD'                     											
			                    from
			                    (select parent_id
					                    ,name
					                    ,stringvalue
			                    from dbo.jsonParse(NOM_USL)
			                    where object_id is null ) as Piv
			                    pivot
			                    (
			                    max(stringvalue)
			                    for name in (Nom, Usl, PRVS_Usl, DatN, Code_Usl, MD, D)
			                    ) as dd
			                    where LPU_ST = 3 and Usl is not null
			                    for xml path(''), type ) as 'SL'
                    
            --- Услуга Стационара USL ---
                                ,(select
                                     1  as 'USL/IDSERV' 
                                    ,'555509'   as 'USL/LPU' 
                                    ,LPU_1      as 'USL/LPU_1'
                                    ,PODR       as 'USL/PODR'
                                    ,PROFIL     as 'USL/PROFIL'
                                    ,VID_VME    as 'USL/VID_VME'
                                    ,DET        as 'USL/DET'
                                    ,convert(nvarchar(10), ARR_DATE, 20) as 'USL/DATE_IN'
                                    ,convert(nvarchar(10), EX_DATE, 20)  as 'USL/DATE_OUT'            
                                    ,DS1        as 'USL/DS' 
                                    ,CODE_USL   as 'USL/CODE_USL'
                                    ,cast(KOL_USL as decimal(5,2))   as 'USL/KOL_USL'
                                    ,0.00       as 'USL/TARIF'
                                    ,0.00       as 'USL/SUMV_USL'
                                    ,PRVS       as 'USL/PRVS' 
                                    ,IDDOKT     as 'USL/CODE_MD' 									          
                                where LPU_ST in (1, 2)
                                for xml path(''), type ) as 'SL'
               
            --- Услуга лечения операции USL ---
                                ,(select 
                                    row_number() over (order by convert(date, DatN, 104), Cod) + 1 as 'USL/IDSERV' 
                                    ,'555509'   as 'USL/LPU' 
                                    ,LPU_1      as 'USL/LPU_1'
                                    ,PODR       as 'USL/PODR'
                                    ,PROFIL     as 'USL/PROFIL'
                                    ,iif(left(Usl, 2) = 'sh', null, Usl)        as 'USL/VID_VME'
                                    ,DET        as 'USL/DET'
                                    ,convert(nvarchar(10), convert(date, DatN, 104 ), 20) as 'USL/DATE_IN'
                                    ,convert(nvarchar(10), convert(date, DatK, 104 ), 20)  as 'USL/DATE_OUT' -- Для схем дату окончания ставим равной дате выписки           
                                    ,DS1        as 'USL/DS' 
                                    ,iif(left(DopUsl, 2) = 'mt', DopUsl, left(Usl, charindex('.', Usl, 6) - 1))   as 'USL/CODE_USL'
                                    ,cast(isnull(Frakc, 1) as decimal(5,2))   as 'USL/KOL_USL'
                                    ,0.00       as 'USL/TARIF'                       
                                    ,0.00       as 'USL/SUMV_USL'
                                    ,PRVS       as 'USL/PRVS' 
                                    ,IDDOKT     as 'USL/CODE_MD' 
			                    from
			                    (select parent_id
					                    ,name
					                    ,stringvalue
			                    from dbo.jsonParse(NOM_USL)
			                    where object_id is null ) as Piv
			                    pivot
			                    (
			                    max(stringvalue)
			                    for name in (Usl, Tip, DatN, DatK, DopUsl, Frakc)
			                    ) as dd
			                    where LPU_ST in (1, 2) and Usl is not null and Tip <> 'диаг' and left(Usl, 2) <> 'sh'
                                order by convert(date, DatN, 104), Cod -- Сортируем услуги по дате
			                    for xml path(''), type ) as 'SL'
            --- Услуга Параклиники USL ---
                                ,(select
                                     1  as 'USL/IDSERV' 
                                    ,'555509'   as 'USL/LPU' 
                                    ,LPU_1      as 'USL/LPU_1'
                                    ,PODR       as 'USL/PODR'
                                    ,PROFIL     as 'USL/PROFIL'
                                    ,VID_VME    as 'USL/VID_VME'
                                    ,DET        as 'USL/DET'
                                    ,convert(nvarchar(10), ARR_DATE, 20) as 'USL/DATE_IN'
                                    ,convert(nvarchar(10), EX_DATE, 20)  as 'USL/DATE_OUT'            
                                    ,DS1        as 'USL/DS' 
                                    ,CODE_USL   as 'USL/CODE_USL'
                                    ,cast(KOL_USL as decimal(5,2))    as 'USL/KOL_USL'
                                    ,SUM_LPU    as 'USL/TARIF'
                                    ,SUM_LPU    as 'USL/SUMV_USL'
                                    ,PRVS       as 'USL/PRVS' 
                                    ,IDDOKT     as 'USL/CODE_MD' 									          
                                where LPU_ST in (3, 4)
                                for xml path(''), type ) as 'SL'
            --- продолжаем Законченный Случай Z_SL ---
                            ,IDSP       as 'IDSP'
                            ,SUM_LPU    as 'SUMV'               
                        for xml path(''), type) as 'Z_SL'       
                    for xml path(''), type) as 'ZAP'
                from dbo.StrahReestr as b
                where CodFile = sf.Cod
                order by N_ZAP, NOM_ZAP   
                for xml path(''),  type)
            from dbo.StrahFile as sf
            where Cod = {CodFile}
            for xml path(''), root('ZL_LIST'),  type";
            return _Return;
        }
        
        /// <summary>Выгрузка ВМП Файла ВЕРСИЯ 2019, приказ 285</summary>
        public static string StrahReestrXML_Select_VMP_2(int CodFile, string pMainFileName)
        {
            string _Return = $@"
                    use Bazis;

                     --- Заголовок ZGLV ---
                    select
                        '3.1'                                   as 'ZGLV/VERSION'
                        ,convert(nvarchar(10), getdate(), 20)   as 'ZGLV/DATA'
                        ,'{pMainFileName}'                      as 'ZGLV/FILENAME'					
                        ,(select count(*) from dbo.StrahReestr where CodFile = sf.Cod) as 'ZGLV/SD_Z'
                    --- Счет SCHET ---
                        ,Cod        as 'SCHET/CODE'
                        ,'555509'   as 'SCHET/CODE_MO'
                        ,[YEAR]     as 'SCHET/YEAR'
                        ,[MONTH]    as 'SCHET/MONTH'
                        ,NSCHET     as 'SCHET/NSCHET'
                        ,convert(nvarchar(10), DSCHET, 20) as 'SCHET/DSCHET'
                        ,case StrahComp 
                            when 1 then '55050'
                            when 2 then '55041'
                            when 3 then '55046'
                            else null
                         end        as 'SCHET/PLAT'
                        ,SUMMAV     as 'SCHET/SUMMAV'
                    --- Запись ZAP ---  
                        ,(select      
                                N_ZAP               as 'ZAP/N_ZAP' 
			                    ,isnull(PR_NOV, 0)  as 'ZAP/PR_NOV'
                    --- Пациент PACIENT ---
                                ,ID_PAC     as 'ZAP/PACIENT/ID_PAC'
                                ,VPOLIS     as 'ZAP/PACIENT/VPOLIS'
                                ,iif(SERIA = '', null, SERIA) as 'ZAP/PACIENT/SPOLIS'
                                ,NUMBER     as 'ZAP/PACIENT/NPOLIS'	
                                ,PLAT       as 'ZAP/PACIENT/SMO'
                                ,SMO_OK     as 'ZAP/PACIENT/SMO_OK'   		  
                                ,0          as 'ZAP/PACIENT/NOVOR'
                    -- Законченный случай Z_SL ---
                                ,(select  
                                     NOM_ZAP    as 'Z_SL/IDCASE'
                                    ,LPU_ST     as 'Z_SL/USL_OK'
                                    ,VIDPOM     as 'Z_SL/VIDPOM'
                                    ,3          as 'Z_SL/FOR_POM'
                                    ,json_value(NOM_USL, '$.NPR_MO')   as 'Z_SL/NPR_MO'                                 --- !!!
                                    ,convert(nvarchar(10), convert(date, json_value(NOM_USL, '$.NPR_DATE'), 104 ), 20)  as 'Z_SL/NPR_DATE'    --- !!!
                                    ,'555509'   as 'Z_SL/LPU'
                                    ,convert(nvarchar(10), ARR_DATE, 20) as 'Z_SL/DATE_Z_1'
                                    ,convert(nvarchar(10), EX_DATE, 20)  as 'Z_SL/DATE_Z_2'
                                    ,cast(KOL_USL as int)   as 'Z_SL/KD_Z'
                                    ,RES_G      as 'Z_SL/RSLT'
                                    ,ISHOD      as 'Z_SL/ISHOD'
                                    ,iif(OS_SLUCH = '', null, OS_SLUCH) as 'Z_SL/OS_SLUCH'             
                    --- Случай SL ---
                                    ,(select
                                         1          as 'SL/SL_ID'                    
                                        ,VID_HMP    as 'SL/VID_HMP'
                                        ,METOD_HMP  as 'SL/METOD_HMP'
                                        ,LPU_1      as 'SL/LPU_1'
                                        ,PODR       as 'SL/PODR'
                                        ,PROFIL     as 'SL/PROFIL'
                                        ,json_value(NOM_USL, '$.PROFIL_K') as 'SL/PROFIL_K'                          
                                        ,DET        as 'SL/DET'
                                        ,convert(nvarchar(10), convert(date, json_value(NOM_USL, '$.TAL_D'), 104 ), 20)  as 'SL/TAL_D' 
                                        ,replace(json_value(NOM_USL, '$.TAL_NUM'), '.', '/') as 'SL/TAL_NUM' 
                                        ,convert(nvarchar(10), ARR_DATE, 20) as 'SL/TAL_P'      
                                        ,PACIENTID  as 'SL/NHISTORY'
                                        ,convert(nvarchar(10), ARR_DATE, 20) as 'SL/DATE_1'
                                        ,convert(nvarchar(10), EX_DATE, 20)  as 'SL/DATE_2'
                                        ,DS1        as 'SL/DS1'
                                        ,iif(DS2 = '', null, DS2)      as 'SL/DS2'
                                        ,json_value(NOM_USL, '$.C_ZAB') as 'SL/C_ZAB'
                                        ,iif(json_value(NOM_USL, '$.DS_ONK') = 1, 1, 0) as 'SL/DS_ONK'
                        --- БЛОК Направления к врачу околога при подозрении NAPR --- Убрал 29/04/2019
                                      --  ,convert(nvarchar(10), EX_DATE, 20) as 'SL/NAPR/NAPR_DATE' 
                                      --  ,1                                  as 'SL/NAPR/NAPR_V'
                         --- БЛОК Направления на исследования добавил 17/12/2019 ---
							            ,(select convert(nvarchar(10), convert(date, NAPR_DATE, 104 ), 20) as NAPR_DATE, NAPR_V, MET_ISSL, NAPR_USL
								          from openjson(NOM_USL, '$.NAPR')
								          with (   
									        NAPR_DATE varchar(10)	'$.NAPR_DATE' ,  
									        NAPR_V    int			'$.NAPR_V',  
									        MET_ISSL  int			'$.MET_ISSL',  
									        NAPR_USL  nvarchar(15)  '$.NAPR_USL'  
									        ) 
								          for xml path('NAPR'), type) as 'SL' 
                        --- БЛОК Консилиумы CONS ---  
                                        ,(select 
                                                0                                   as 'CONS/PR_CONS'     -- нет консилиума                                                       
                                            where dbo.jsonIf(NOM_USL, 'Taktika_1') = 0 and dbo.jsonIf(NOM_USL, 'Taktika_2') = 0 and dbo.jsonIf(NOM_USL, 'Taktika_3') = 0
                                            for xml path(''), type ) as 'SL'
                                        ,(select 
                                                1                                   as 'CONS/PR_CONS'     -- противопоказания химии  
                                                ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_1'), 104 ), 20)   as 'CONS/DT_CONS'      
                                            where dbo.jsonIf(NOM_USL, 'Taktika_1') = 1
                                            for xml path(''), type ) as 'SL'
                                        ,(select 
                                                2                                   as 'CONS/PR_CONS'     -- противопоказания лучевой  
                                                ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_2'), 104 ), 20)   as 'CONS/DT_CONS'      
                                            where dbo.jsonIf(NOM_USL, 'Taktika_2') = 1
                                            for xml path(''), type ) as 'SL'
                                        ,(select 
                                                3                                   as 'CONS/PR_CONS'     -- отказ от хирургии  
                                                ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'Taktika_3'), 104 ), 20)   as 'CONS/DT_CONS'      
                                            where dbo.jsonIf(NOM_USL, 'Taktika_3') = 1
                                            for xml path(''), type ) as 'SL'  
                    --- БЛОК Онкологическое лечение ONK_SL ---
                                        ,(select 
                                             iif(dbo.jsonIf(NOM_USL, 'DS1_T') = 1, dbo.jsonValInt(NOM_USL, 'DS1_T'), 0) as 'ONK_SL/DS1_T' 
                                            ,dbo.jsonValInt(NOM_USL, 'STAD')  as 'ONK_SL/STAD' 
                                            ,dbo.jsonValInt(NOM_USL, 'ONK_T') as 'ONK_SL/ONK_T'
                                            ,dbo.jsonValInt(NOM_USL, 'ONK_N') as 'ONK_SL/ONK_N'
                                            ,dbo.jsonValInt(NOM_USL, 'ONK_M') as 'ONK_SL/ONK_M'                      
                                            ,dbo.jsonValInt(NOM_USL, 'MTSTZ') as 'ONK_SL/MTSTZ'                     
                        --- БЛОК Диагноститческий блок B_DIAG --- 
                                            ,(select 
                                                 convert(date, DIAG_DATE, 104 )    as 'B_DIAG/DIAG_DATE'
                                                ,DIAG_TIP       as 'B_DIAG/DIAG_TIP' 
                                                ,DIAG_CODE      as 'B_DIAG/DIAG_CODE'
                                                ,DIAG_RSLT      as 'B_DIAG/DIAG_RSLT'
                                                ,1              as 'B_DIAG/REC_RSLT'                                              											
			                                from
			                                (select parent_id
					                                ,name
					                                ,stringvalue
			                                from dbo.jsonParse(NOM_USL)
			                                where object_id is null ) as Piv
			                                pivot
			                                (
			                                max(stringvalue)
			                                for name in (DIAG_DATE, DIAG_TIP, DIAG_CODE, DIAG_RSLT)
			                                ) as dd
			                                where LPU_ST < 3 and DIAG_TIP is not null
			                                for xml path(''), type ) as 'ONK_SL'
                        --- БЛОК Противопоказания или отказы B_PROT ---  
                                            ,(select 
                                                    1                                   as 'B_PROT/PROT'     -- противопоказания хирургии  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_1'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_1') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                                            ,(select 
                                                    2                                   as 'B_PROT/PROT'     -- противопоказания химии  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_2'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_2') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                                            ,(select 
                                                    3                                   as 'B_PROT/PROT'     -- противопоказания лучевой  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_3'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_3') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                                            ,(select 
                                                    4                                   as 'B_PROT/PROT'     -- отказ от хирургии  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_4'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_4') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                                            ,(select 
                                                    5                                   as 'B_PROT/PROT'     -- отказ от химии  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_5'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_5') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                                            ,(select 
                                                    6                                   as 'B_PROT/PROT'     -- отказ от лучевой  
                                                    ,convert(nvarchar(10), convert(date, dbo.jsonValStr(NOM_USL, 'PrOt_6'), 104 ), 20)   as 'B_PROT/D_PROT'      
                                                where dbo.jsonIf(NOM_USL, 'PrOt_6') = 1
                                                for xml path(''), type ) as 'ONK_SL'
                        --- БЛОК Онкологическое лечение ONK_USL ---
                                            ,(select
							                     json_value(value, '$.USL_TIP') as 'ONK_USL/USL_TIP'
							                    ,json_value(value, '$.HIR_TIP') as 'ONK_USL/HIR_TIP'
							                    ,json_value(value, '$.LEK_TIP_L') as 'ONK_USL/LEK_TIP_L'
							                    ,json_value(value, '$.LEK_TIP_V') as 'ONK_USL/LEK_TIP_V'
							                    ,(select 
								                     json_value(value, '$.REGNUM') as 'LEK_PR/REGNUM'
								                    ,json_value(value, '$.CODE_SH') as 'LEK_PR/CODE_SH'
								                    ,(select convert(nvarchar(10), convert(date, value, 104 ), 20) as DATE_INJ 
								                      from openjson(value, '$.DATE_INJ')	for xml path(''), type) as 'LEK_PR'
							                     from openjson(value, '$.LEK_PR')
							                     for xml path(''), type) as 'ONK_USL'
							                    ,json_value(value, '$.LUCH_TIP') as 'ONK_USL/LUCH_TIP' 
							                    ,json_value(value, '$.PPTR') as 'ONK_USL/PPTR'
						                    from openjson(NOM_USL, '$.ONK_SL.ONK_USL')
						                    for xml path(''), type) as 'ONK_SL' 
	                    --- Продолжаем блок Онкологическое лечение ONK_SL
                                            ,try_cast(dbo.jsonValReal(NOM_USL, 'SOD')  as decimal(6,2)) as 'ONK_SL/SOD'
                                            ,iif(dbo.jsonIf(NOM_USL, 'K_FR') = 1, iif(dbo.jsonValInt(NOM_USL, 'K_FR') = -1, 0, dbo.jsonValInt(NOM_USL, 'K_FR')), null) as 'ONK_SL/K_FR'
                                            ,try_cast(dbo.jsonValReal(NOM_USL, 'WEI')  as decimal(4,1)) as 'ONK_SL/WEI'
                                            ,dbo.jsonValInt(NOM_USL, 'HEI')     as 'ONK_SL/HEI'
                                            ,try_cast(dbo.jsonValReal(NOM_USL, 'BSA')  as decimal(3,2)) as 'ONK_SL/BSA'
                                            where isjson(NOM_USL) > 0
	                                              and (json_value(NOM_USL, '$.ONK_SL.DS1_T') is not null 
                                                       or json_value(NOM_USL, '$.ONK_SL.STAD') is not null)
                                            for xml path(''), type ) as 'SL'
                    --- Продолжаем блок случая SL
                                        ,PRVS       as 'SL/PRVS'
                                        ,'V021'     as 'SL/VERS_SPEC'
                                        ,IDDOKT     as 'SL/IDDOKT'
                                        ,1.00       as 'SL/ED_COL'
                                        ,SUM_LPU    as 'SL/TARIF'                                   
                                        ,SUM_LPU    as 'SL/SUM_M'
                    --- Услуга Стационара USL ---
                                        ,(select
                                                1  as 'USL/IDSERV' 
                                            ,'555509'   as 'USL/LPU' 
                                            ,LPU_1      as 'USL/LPU_1'
                                            ,PODR       as 'USL/PODR'
                                            ,PROFIL     as 'USL/PROFIL'
                                            ,VID_VME    as 'USL/VID_VME'
                                            ,DET        as 'USL/DET'
                                            ,convert(nvarchar(10), ARR_DATE, 20) as 'USL/DATE_IN'
                                            ,convert(nvarchar(10), EX_DATE, 20)  as 'USL/DATE_OUT'            
                                            ,DS1        as 'USL/DS' 
                                            ,CODE_USL   as 'USL/CODE_USL'
                                            ,cast(KOL_USL as decimal(5,2))    as 'USL/KOL_USL'
                                            ,0.00       as 'USL/TARIF'                         
                                            ,0.00       as 'USL/SUMV_USL'
                                            ,PRVS       as 'USL/PRVS' 
                                            ,IDDOKT     as 'USL/CODE_MD' 	                    
                                        where LPU_ST in (1, 2)
                                        for xml path(''), type ) as 'SL'
               
                    --- Услуга лечения операции USL --- (пока не подаем 03/02/2020)
                      --                  ,(select 
                      --                      row_number() over (order by convert(date, DatN, 104), Cod) + 1 as 'USL/IDSERV' 
                      --                      ,'555509'   as 'USL/LPU' 
                      --                      ,LPU_1      as 'USL/LPU_1'
                      --                      ,PODR       as 'USL/PODR'
                      --                      ,PROFIL     as 'USL/PROFIL'
                      --                      ,iif(left(Usl, 2) = 'sh', null, Usl)        as 'USL/VID_VME'
                      --                      ,DET        as 'USL/DET'
                      --                      ,convert(nvarchar(10), convert(date, DatN, 104 ), 20) as 'USL/DATE_IN'
                      --                      ,convert(nvarchar(10), convert(date, DatK, 104 ), 20)  as 'USL/DATE_OUT' -- Для схем дату окончания ставим равной дате выписки           
                      --                      ,DS1        as 'USL/DS' 
                      --                      ,iif(left(DopUsl, 2) = 'mt', DopUsl, left(Usl, charindex('.', Usl, 6) - 1))   as 'USL/CODE_USL'
                      --                      ,cast(isnull(Frakc, 1) as decimal(5,2))  as 'USL/KOL_USL'
                      --                      ,0.00       as 'USL/TARIF'                       
                      --                      ,0.00       as 'USL/SUMV_USL'
                      --                      ,PRVS       as 'USL/PRVS' 
                      --                      ,IDDOKT     as 'USL/CODE_MD' 
			          --                  from
			          --                  (select parent_id
					  --                          ,name
					  --                          ,stringvalue
			          --                  from dbo.jsonParse(NOM_USL)
			          --                  where object_id is null ) as Piv
			          --                  pivot
			          --                  (
			          --                  max(stringvalue)
			          --                  for name in (Usl, Tip, DatN, DatK, DopUsl, Frakc)
			          --                  ) as dd
			          --                  where LPU_ST in (1, 2) and Usl is not null and Tip <> 'диаг' and left(Usl, 2) <> 'sh'
                      --                  order by convert(date, DatN, 104), Cod -- Сортируем услуги по дате
			          --              for xml path(''), type ) as 'SL'
                    --- продолжаем Законченный Случай Z_SL ---
                                    ,IDSP       as 'IDSP'
                                    ,SUM_LPU    as 'SUMV'               
                                for xml path(''), type) as 'Z_SL'       
                            for xml path(''), type) as 'ZAP'
                        from dbo.StrahReestr as b
                        where CodFile = sf.Cod
                        order by N_ZAP, NOM_ZAP   
                        for xml path(''),  type)
                    from dbo.StrahFile as sf
                    where Cod = {CodFile}
                    for xml path(''), root('ZL_LIST'),  type";
            return _Return;
        }

        /// <summary>Выгрузка файла НЕ зно ВЕРСИЯ 2019, приказ 285</summary>
        public static string StrahReestrXML_Select_Main_4(int CodFile, string pMainFileName)
        {
            string _Return = $@"
                    use Bazis;

                    --- Заголовок ZGLV ---
                    select
                        '3.1'                                   as 'ZGLV/VERSION'
                        ,convert(nvarchar(10), getdate(), 20)   as 'ZGLV/DATA'
                        ,'{pMainFileName}'                      as 'ZGLV/FILENAME'					
                        ,(select count(*) from dbo.StrahReestr where CodFile = sf.Cod) as 'ZGLV/SD_Z'
                    --- Счет SCHET ---
                        ,Cod        as 'SCHET/CODE'
                        ,'555509'   as 'SCHET/CODE_MO'
                        ,[YEAR]     as 'SCHET/YEAR'
                        ,[MONTH]    as 'SCHET/MONTH'
                        ,NSCHET     as 'SCHET/NSCHET'
                        ,convert(nvarchar(10), DSCHET, 20) as 'SCHET/DSCHET'
                        ,case StrahComp 
                            when 1 then '55050'
                            when 2 then '55041'
                            when 3 then '55046'
                            else null
                         end        as 'SCHET/PLAT'
                        ,SUMMAV     as 'SCHET/SUMMAV'
                    --- Запись ZAP ---  
                        ,(select      
                                 N_ZAP              as 'ZAP/N_ZAP' 
			                    ,isnull(PR_NOV, 0)  as 'ZAP/PR_NOV'
                    --- Пациент PACIENT ---
                                ,ID_PAC     as 'ZAP/PACIENT/ID_PAC'
                                ,VPOLIS     as 'ZAP/PACIENT/VPOLIS'
                                ,iif(SERIA = '', null, SERIA) as 'ZAP/PACIENT/SPOLIS'
                                ,NUMBER     as 'ZAP/PACIENT/NPOLIS'	
                                ,PLAT       as 'ZAP/PACIENT/SMO'
                                ,SMO_OK     as 'ZAP/PACIENT/SMO_OK'
                                ,0          as 'ZAP/PACIENT/NOVOR'
                    -- Законченный случай Z_SL ---
                                ,(select  
                                     NOM_ZAP    as 'Z_SL/IDCASE'
                                    ,LPU_ST     as 'Z_SL/USL_OK'
                                    ,VIDPOM     as 'Z_SL/VIDPOM'
                                    ,3          as 'Z_SL/FOR_POM'
                                    ,json_value(NOM_USL, '$.NPR_MO')   as 'Z_SL/NPR_MO'                                 --- !!!
                                    ,convert(nvarchar(10), convert(date, json_value(NOM_USL, '$.NPR_DATE'), 104 ), 20)  as 'Z_SL/NPR_DATE'    --- !!!
                                    ,'555509'   as 'Z_SL/LPU'
                                    ,convert(nvarchar(10), ARR_DATE, 20) as 'Z_SL/DATE_Z_1'
                                    ,convert(nvarchar(10), EX_DATE, 20)  as 'Z_SL/DATE_Z_2'
                                    ,iif(LPU_ST = 3, null, cast(KOL_USL as int))   as 'Z_SL/KD_Z'  -- только для стационара
                                    ,RES_G      as 'Z_SL/RSLT'
                                    ,ISHOD      as 'Z_SL/ISHOD'
                                    ,iif(OS_SLUCH = '', null, OS_SLUCH) as 'Z_SL/OS_SLUCH'
                                    --   ,iif(1 = 2, 1, null) as 'Z_SL/VB_P'                         -- !!! переводы между отделением
                    --- Случай SL ---
                                    ,(select
                                         1          as 'SL/SL_ID'
                                        ,LPU_1      as 'SL/LPU_1'
                                        ,PODR       as 'SL/PODR'
                                        ,PROFIL     as 'SL/PROFIL'
                                        ,json_value(NOM_USL, '$.PROFIL_K') as 'SL/PROFIL_K'       
                                        ,DET        as 'SL/DET'
                                        ,iif(LPU_ST = 3, json_value(NOM_USL, '$.P_Cel'), null) as 'SL/P_CEL'        
                                        ,PACIENTID  as 'SL/NHISTORY'
                                        ,iif(LPU_ST < 3, 1, null) as 'SL/P_PER'             -- !!! признак поступления/перевода стационара 1 или 4
                                        ,convert(nvarchar(10), ARR_DATE, 20) as 'SL/DATE_1'
                                        ,convert(nvarchar(10), EX_DATE, 20)  as 'SL/DATE_2'
                                        ,iif(LPU_ST < 3, cast(KOL_USL as int), null) as 'SL/KD'
                                        ,DS1        as 'SL/DS1'                                        
                                        ,iif(DS2 = '', null, DS2)			as 'SL/DS2'
                                        ,json_value(NOM_USL, '$.C_ZAB')     as 'SL/C_ZAB'
                                        ,iif(LPU_ST = 3 and json_value(NOM_USL, '$.P_CEL') = '1.3', 1, null) as 'SL/DN'  -- если в поликлинике цель посещение 1.3 диспансерное наблюдение
                    --- Услуга по КСГ для стационара KSG_KPG ---              
                                        ,(select 
                                             json_value(NOM_USL, '$.KSG')    as 'KSG_KPG/N_KSG' 
                                            ,[YEAR]     as 'KSG_KPG/VER_KSG' 
                                            ,0          as 'KSG_KPG/KSG_PG'
                                            ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_Z') as decimal(8,5))   as 'KSG_KPG/KOEF_Z'     
                                            ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_UP') as decimal(8,5))  as 'KSG_KPG/KOEF_UP'
                                            ,TARIF      as 'KSG_KPG/BZTSZ'
                                            ,1.10500    as 'KSG_KPG/KOEF_D'     
                                            ,cast(dbo.jsonValReal(NOM_USL, 'KOEF_U') as decimal(8,5))           as 'KSG_KPG/KOEF_U'     
                                            ,(select value as CRIT from openjson(NOM_USL, '$.CRIT') for xml path(''), type) as 'KSG_KPG'     
                                            ,iif(dbo.jsonValReal(NOM_USL, 'IT_SL') is not null, 1, 0)           as 'KSG_KPG/SL_K' 
                                            ,cast(dbo.jsonValReal(NOM_USL, 'IT_SL') as decimal(8,5))            as 'KSG_KPG/IT_SL'     
                    --- КСЛП для стационара SL_KOEF ---
                                            ,(select 
                                                     10      as 'SL_KOEF/IDSL'       
                                                    ,cast(dbo.jsonValReal(NOM_USL, 'Sl10') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                                where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl10') = 1
                                                for xml path(''), type ) as 'KSG_KPG'  
						                    ,(select 
                                                     13      as 'SL_KOEF/IDSL'       
                                                    ,cast(dbo.jsonValReal(NOM_USL, 'Sl13') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                                where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl13') = 1
                                                for xml path(''), type ) as 'KSG_KPG' 
                                            ,(select 
                                                     14      as 'SL_KOEF/IDSL'       
                                                    ,cast(dbo.jsonValReal(NOM_USL, 'Sl14') + 1 as decimal(8,5))  as 'SL_KOEF/Z_SL'      
                                                where LPU_ST = 1 and dbo.jsonIf(NOM_USL, 'Sl14') = 1
                                                for xml path(''), type ) as 'KSG_KPG'                                                                                                                                                                                                                            
                                            where LPU_ST in (1, 2)
                                            for xml path(''), type ) as 'SL'
                    --- продолжаем Случай SL ---
                                        ,PRVS       as 'SL/PRVS'
                                        ,'V021'     as 'SL/VERS_SPEC'
                                        ,IDDOKT     as 'SL/IDDOKT'
                                        ,1.00       as 'SL/ED_COL'
                                        ,SUM_LPU    as 'SL/TARIF' --iif(LPU_ST in (1, 2), SUM_LPU, null)    as 'SL/TARIF'
                                        ,SUM_LPU    as 'SL/SUM_M'
                    --- Услуга Поликлиники USL ---
                                        ,(select 
                                            row_number() over (order by Cod) as 'USL/IDSERV' 
                                            ,'555509'   as 'USL/LPU' 
                                            ,LPU_1      as 'USL/LPU_1'
                                            ,PODR       as 'USL/PODR'
                                            ,PROFIL     as 'USL/PROFIL'
                                            ,Usl        as 'USL/VID_VME'
                                            ,DET        as 'USL/DET'
                                            ,convert(nvarchar(10), convert(date, DatN, 104 ), 20) as 'USL/DATE_IN'
                                            ,convert(nvarchar(10), convert(date, DatN, 104 ), 20)  as 'USL/DATE_OUT'            
                                            ,D          as 'USL/DS' 
                                            ,Code_Usl   as 'USL/CODE_USL'
                                            ,1.00       as 'USL/KOL_USL' 
                                            ,0.00          as 'USL/TARIF'   
                                            ,0.00          as 'USL/SUMV_USL'                   
                                            ,PRVS_Usl   as 'USL/PRVS' 
                                            ,MD         as 'USL/CODE_MD'                     											
			                            from
			                            (select parent_id
					                            ,name
					                            ,stringvalue
			                            from dbo.jsonParse(NOM_USL)
			                            where object_id is null ) as Piv
			                            pivot
			                            (
			                            max(stringvalue)
			                            for name in (Nom, Usl, PRVS_Usl, DatN, Code_Usl, MD, D)
			                            ) as dd
			                            where LPU_ST = 3 and Usl is not null
			                            for xml path(''), type ) as 'SL'
                    
                    --- Услуга Стационара USL ---
                                        ,(select
                                             1  as 'USL/IDSERV' 
                                            ,'555509'   as 'USL/LPU' 
                                            ,LPU_1      as 'USL/LPU_1'
                                            ,PODR       as 'USL/PODR'
                                            ,PROFIL     as 'USL/PROFIL'
                                            ,VID_VME    as 'USL/VID_VME'
                                            ,DET        as 'USL/DET'
                                            ,convert(nvarchar(10), ARR_DATE, 20) as 'USL/DATE_IN'
                                            ,convert(nvarchar(10), EX_DATE, 20)  as 'USL/DATE_OUT'            
                                            ,DS1        as 'USL/DS' 
                                            ,CODE_USL   as 'USL/CODE_USL'
                                            ,cast(KOL_USL as decimal(5,2))    as 'USL/KOL_USL'
                                            ,0.00          as 'USL/TARIF'
                                            ,0.00          as 'USL/SUMV_USL'
                                            ,PRVS       as 'USL/PRVS' 
                                            ,IDDOKT     as 'USL/CODE_MD' 									          
                                        where LPU_ST in (1, 2)
                                        for xml path(''), type ) as 'SL'
               
                    --- Услуга лечения операции USL ---
                                        ,(select 
                                            row_number() over (order by convert(date, DatN, 104), Cod) + 1 as 'USL/IDSERV' 
                                            ,'555509'   as 'USL/LPU' 
                                            ,LPU_1      as 'USL/LPU_1'
                                            ,PODR       as 'USL/PODR'
                                            ,PROFIL     as 'USL/PROFIL'
                                            ,iif(left(Usl, 2) = 'sh', null, Usl)        as 'USL/VID_VME'
                                            ,DET        as 'USL/DET'
                                            ,convert(nvarchar(10), convert(date, DatN, 104 ), 20) as 'USL/DATE_IN'
                                            ,convert(nvarchar(10), convert(date, DatK, 104 ), 20)  as 'USL/DATE_OUT'  -- Для схем дату окончания ставим равной дате выписки           
                                            ,DS1        as 'USL/DS' 
                                            ,iif(left(DopUsl, 2) = 'mt', DopUsl, left(Usl, charindex('.', Usl, 6) - 1))   as 'USL/CODE_USL'
                                            ,cast(isnull(Frakc, 1) as decimal(5,2))    as 'USL/KOL_USL'
                                            ,0.00          as 'USL/TARIF'                       
                                            ,0.00          as 'USL/SUMV_USL'
                                            ,PRVS       as 'USL/PRVS' 
                                            ,IDDOKT     as 'USL/CODE_MD' 
			                            from
			                            (select parent_id
					                            ,name
					                            ,stringvalue
			                            from dbo.jsonParse(NOM_USL)
			                            where object_id is null ) as Piv
			                            pivot
			                            (
			                            max(stringvalue)
			                            for name in (Usl, Tip, DatN, DatK, DopUsl, Frakc)
			                            ) as dd
			                            where LPU_ST in (1, 2) and Usl is not null and Tip <> 'диаг' and left(Usl, 2) <> 'sh'
                                        order by convert(date, DatN, 104), Cod -- Сортируем услуги по дате
			                        for xml path(''), type ) as 'SL'
                    --- продолжаем Законченный Случай Z_SL ---
                                    ,IDSP       as 'IDSP'
                                    ,SUM_LPU    as 'SUMV'               
                                for xml path(''), type) as 'Z_SL'       
                            for xml path(''), type) as 'ZAP'
                        from dbo.StrahReestr as b
                        where CodFile = sf.Cod
                        order by N_ZAP, NOM_ZAP   
                        for xml path(''),  type)
                    from dbo.StrahFile as sf
                    where Cod = {CodFile}
                    for xml path(''), root('ZL_LIST'),  type";
            return _Return;
        }

        /// <summary>Выгрузка Файла Пациентов</summary>
        public static string StrahReestrXMLPerson_Select_1(int CodFile, string pMainFileName, string pPacFileName)
        {
            string _Return = $@"
                 select
                    '3.2'                                   as 'ZGLV/VERSION'
                    ,convert(nvarchar(10), getdate(), 20)   as 'ZGLV/DATA'
                    ,'{pPacFileName}'                       as 'ZGLV/FILENAME'
                    ,'{pMainFileName}'                      as 'ZGLV/FILENAME1'
                    ,(select
                --- Данные ---
                         distinct(ID_PAC)                   as 'PERS/ID_PAC'
                        ,FAMILY                             as 'PERS/FAM'
                        ,[NAME]                             as 'PERS/IM'
                        ,iif(FATHER = '', null, FATHER)     as 'PERS/OT'
                        ,POL                                as 'PERS/W'
                        ,convert(nvarchar(10), VOZRAST, 20) as 'PERS/DR'
                        ,iif(OS_SLUCH = '2', '1', null)     as 'PERS/DOST'                        
                        ,iif(FAM_P = '', null, FAM_P)       as 'PERS/FAM_P'
                        ,iif(IM_P = '', null, IM_P)         as 'PERS/IM_P'
                        ,iif(OT_P = '', null, OT_P)         as 'PERS/OT_P'
                        ,iif(W_P = 0, null, cast(W_P as char(1))) as 'PERS/W_P'
                        ,iif(isnull(DR_P, '') = '', null, convert(nvarchar(10), DR_P, 20)) as 'PERS/DR_P'
                        ,iif(OT_P = 'НЕТ', '1', null)       as 'PERS/DOST_P'
                        ,replace(MR, '""', '&quot;')        as 'PERS/MR'
                        ,iif(DOCTYPE = 0, null, cast(DOCTYPE as char(2))) as 'PERS/DOCTYPE'                        
                        ,iif(DOCSER = '', null, DOCSER)     as 'PERS/DOCSER'
                        ,iif(DOCNUM = '', null, DOCNUM)     as 'PERS/DOCNUM'
                        ,iif(DOCDATE is null, null, convert(nvarchar(10), DOCDATE, 20))  as 'PERS/DOCDATE'
                        ,iif(DOCORG = '', null, DOCORG)     as 'PERS/DOCORG'
                        ,iif(SS = '', null, SS)             as 'PERS/SNILS'
                        ,OKATOG                             as 'PERS/OKATOG'
                        ,OKATOG                             as 'PERS/OKATOP'                     
                      from dbo.StrahReestr as b
                      where CodFile = sf.Cod
                      order by FAMILY, [NAME], iif(FATHER = '', null, FATHER)
                      for xml path(''), elements, type)
                from dbo.StrahFile as sf
                where Cod = {CodFile} 
                for xml path(''), root('PERS_LIST')";
            return _Return;
        }
        #endregion

        #region ---- Общие (без наименования таблиц) ----
        /// <summary>Удаляем вариант ответа</summary>
        public static string Table_Delete_1(string TabName, int Cod)
        {
            string _Return = $@"
                delete dbo.{TabName}
                where Cod = {Cod}";
            return _Return;
        }

        /// <summary>Находим код отделения из стационара Apstac</summary>
        public static string Table_Select_1(string pIND)
        {
            string _Return = $@"
                select otd
                from dbo.APSTAC
                where IND = {pIND}";
            return _Return;
        }

        /// <summary>Смотрим сколько записей будет удалены и тип реестров (основные или исправленные) (из Excel)</summary>
        public static string DeleteFromExcel_Select_1()
        {
            string _Return = $@"
                select count(*) as Cou, min(f.pParent) as Par, count(distinct z.Reestr) as Rees
                from Bazis.dbo.StrahReestr	as r
                join Bazis.dbo.StrahZero	as z  on r.CodFile = z.Reestr and r.NOM_ZAP = z.IDCase
                join Bazis.dbo.StrahFile	as f  on z.Reestr = f.Cod";
            return _Return;
        }

        /// <summary>Удаление записей из реестров (из Excel)</summary>
        public static string DeleteFromExcel_Delete_1()
        {
            string _Return = $@"
                delete r
                from Bazis.dbo.StrahReestr as r
                join Bazis.dbo.StrahZero as t
                  on r.CodFile = t.Reestr and r.NOM_ZAP = t.IDCase";
            return _Return;
        }

        /// <summary>Смотрим сколько записей будет обнулено и тип реестров (основные или исправленные) (из Excel)</summary>
        public static string ZeroFromExcel_Select_1()
        {
            string _Return = $@"
                select count(*) as Cou, max(f.pParent) as Par, count(distinct z.Reestr) as Rees
                from Bazis.dbo.StrahReestr	as r
                join Bazis.dbo.StrahZero	as z  on r.CodFile = z.Reestr and r.NOM_ZAP = z.IDCase
                join Bazis.dbo.StrahFile	as f  on z.Reestr = f.Cod";
            return _Return;
        }

        /// <summary>Обнуление записей из реестров (из Excel)</summary>
        public static string ZeroFromExcel_Update_1()
        {
            string _Return = $@"
                update r
                    set r.SUM_LPU = 0.01
                from Bazis.dbo.StrahReestr as r
                join Bazis.dbo.StrahZero as t
                  on r.CodFile = t.Reestr and r.NOM_ZAP = t.IDCase";
            return _Return;
        }

        #endregion

        #region ---- Отчеты ----
        /// <summary>Формируем отчет для экономистов для вывода в Excel</summary>
        /// <param name="pCod">Код реестра StrahFile</param>
        public static string ReestrEco_Select_1(int pCod)
        {
            string _Query = $@"
              	use Bazis;
				declare @CodFile as int = {pCod};

                declare @Reestr as table
                (   
	                PACIENTID nvarchar(50),
	                IDSP int,					
	                PODR int,
	                LPU_1 numeric(8,0),
	                LPU_ST int,
	                FAMILY nvarchar(40),
	                [NAME] nvarchar(40),
	                FATHER nvarchar(40),
	                POL int,
	                VOZRAST date,
	                SERIA nvarchar(10),
	                NUMBER nvarchar(20),
	                PLAT nvarchar(5),
	                ARR_DATE date,
	                EX_DATE date,
	                KOL_USL int,
	                DS1 nvarchar(10),
	                PRVS int,
	                PROFIL int,
	                SUM_LPU numeric(15,2),
	                METOD_HMP nvarchar(5),
	                NOM_USL nvarchar(max),					
	                NOM_ZAP int,
	                Cod int primary key,
                    CodFile int
                );

                insert @Reestr
                select
                     PACIENTID
                    ,IDSP
                    ,PODR 
	                ,LPU_1
	                ,LPU_ST
	                ,FAMILY
	                ,[NAME]
	                ,FATHER
	                ,POL
	                ,VOZRAST
	                ,SERIA
	                ,NUMBER
	                ,PLAT
	                ,ARR_DATE
	                ,EX_DATE
	                ,KOL_USL
	                ,DS1
	                ,PRVS
	                ,PROFIL
	                ,SUM_LPU
	                ,METOD_HMP
	                ,NOM_USL					
	                ,NOM_ZAP
	                ,Cod
                    ,CodFile 
                from dbo.StrahReestr
                where CodFile = @CodFile

                select 
	                r.NOM_ZAP              as IDCASE
	                ,r.LPU_ST
                    ,choose(r.LPU_ST, s.Podr, s.Podr, p.Podr, o.Podr)   as Podr					
                    ,choose(r.LPU_ST, s.Vrach, s.Vrach, p.Vrach, o.Vrach) as Vrach
                    ,isnull(p.Nurs, '')       as Nurse
                    ,dbo.GetFIO(r.FAMILY, r.[NAME], r.FATHER) as FIO
                    ,r.POL
                    ,r.VOZRAST   
                    ,datediff(year, r.VOZRAST, r.EX_DATE) as Age
                    ,ltrim(r.SERIA + ' ' + r.NUMBER) as Polis
                    ,r.PLAT
                    ,r.ARR_DATE as d1
                    ,r.EX_DATE  as d2
                    ,r.KOL_USL  
                    ,r.DS1 
                    ,r.SUM_LPU                   
                    ,iif(r.LPU_ST = 3, iif(json_value(r.NOM_USL, '$.P_Cel') = '3.0', 'Обращение', 'Разовое'), '') as Polik
	                ,r.PRVS
	                ,r.PROFIL					
                    ,r.METOD_HMP                   
                    ,isnull(json_value(r.NOM_USL, '$.KSG'), '') as KSG
                    ,r.PODR as Cod_Otd
                    ,iif(er.IdCase is not null, 'не оплачен', '') as ErrorPay
	                ,r.CodFile
	                ,iif(r.LPU_ST < 3, isnull(json_value(r.NOM_USL, '$.USL[0].Usl'), ''), iif(r.LPU_ST = 4, isnull(json_value(r.NOM_USL, '$.USL[0].Code_Usl'), ''), '')) as Usl1
	                ,iif(r.LPU_ST < 3, isnull(json_value(r.NOM_USL, '$.USL[1].Usl'), ''), '') as Usl2
	                ,iif(r.LPU_ST < 3, isnull(json_value(r.NOM_USL, '$.USL[2].Usl'), ''), '') as Usl3
	                ,iif(r.LPU_ST < 3, isnull(json_value(r.NOM_USL, '$.USL[3].Usl'), ''), '') as Usl4
	                ,iif(r.LPU_ST < 3, isnull(json_value(r.NOM_USL, '$.USL[4].Usl'), ''), '') as Usl5
                    ,iif(s.Arhiv = 1, 'не в архиве', '') as Arhiv
                    
                from @Reestr as r
                left join (select r.Cod    -- Поликлиника
                                 ,iif(r.LPU_1 = 55550900, '1я поликлиника', '2я поликлиника') as Podr
                                 ,v.TKOD    as Vrach
                                 ,n.[Name]  as Nurs 
                            from @Reestr			as r
                            join dbo.APAC			as ac	on ac.Cod= r.PACIENTID
                            join dbo.s_VrachPol		as v	on ac.KV = v.KOD 
                            left join dbo.s_Nurse	as n    on ac.Nurse = n.Cod 
                            where r.LPU_ST = 3 and isnull(ac.xDelete, 0) = 0)     as p
                    on  p.Cod = r.Cod
                left join (select r.Cod    -- Стационар
                                 ,ot.TKOD     as Podr
                                 ,v.TKOD      as Vrach 
                                 ,a.FlagClose as Arhiv
                            from @Reestr			as r
                            join dbo.APSTAC			as a	on a.IND = r.PACIENTID 
                            join dbo.s_VrachStac	as v	on a.KV = v.KOD    
                            join dbo.s_Otdel		as ot   on a.otd = ot.KOD 
                            where r.LPU_ST < 3 and isnull(a.xDelete, 0) = 0)    as s
                    on s.Cod = r.Cod
                left join (select r.Cod    -- Параклиника
                                 ,'Параклиника'     as Podr
                                 ,u.FIO      as Vrach                  
                            from @Reestr			as r
                            join dbo.parObsledov	as o	on o.Cod = r.PACIENTID 
                            join dbo.parProtokol	as p	on p.CodApstac = o.Cod    
                            join dbo.s_UsersDostup  as d	on p.xUserUp = d.UserCod and isjson(d.xInfo) > 0 and json_value(d.xInfo, '$.element') = 6 
			                join dbo.s_Users		as u	on u.Cod = d.UserCod
                            where r.LPU_ST = 4 and isnull(o.xDelete, 0) = 0)    as o
                    on o.Cod = r.Cod
                left join (select distinct IdCase
                           from dbo.StrahError 
                           where CodFiles = @CodFile) as er
                    on r.NOM_ZAP = er.Idcase                
                order by LPU_ST, Podr, Vrach, Nurse, FIO, VOZRAST, d2";
            return _Query;
        }
        #endregion

        #region ---- Для формирования реестра ----
        /// <summary>Заготовка таблицы StrahReestr</summary>
        /// <param name="pSelectAll">Фрагмент sql общих полей для вывода</param>
        /// <param name="pWerePol">Фрагмента sql для поликлинники</param>
        /// <param name="pWereStac">Фрагмента sql для стационара </param>
        /// <param name="pWerePar">Фрагмента sql для параклиники </param>
        public static string ReStrahReestr_Select_1(string pSelectAll, string pWerePol, string pWereStac, string pWerePar)
        {
            string _Return = $@"
                use Bazis;
                select 3     as LPU_ST                                        -- тип подразделения (3 - поликлиника)
                      ,cast(a.Cod as decimal) as Cod 
                      ,a.DP                     
                      {pSelectAll}                                              
                from dbo.APAC as a                                              -- поликлиника
                left join dbo.ErrorASU  as e    on a.PD = e.Cod                 -- справочник ошибок                      
                join dbo.kbol           as k    on a.KL = k.KL 
                left join dbo.s_Diag    as d    on d.KOD=a.D   
                left join dbo.s_Docum   as do   on k.Doc = do.KOD  
                left join dbo.KLADR  as kl
                  on concat(right(concat('00', k.Obl),2), 
			            right(concat('000', k.KR), 3),
			            right(concat('000', k.Gorod), 3), 
			            right(concat('000', k.NasP), 3), '00') = kl.CODE  
                where {pWerePol}                                                -- условия поликлиники
                union all                                                           
                select iif(o.Tip = 1, 1, 2) as LPU_ST                           -- 1 кр. стационар, 2 дн. стационар 
                      ,a.IND as Cod
                      ,a.DN as DP
                      {pSelectAll}  
                from dbo.APSTAC as a                                            -- стационар
                left join dbo.ErrorASU  as e    on a.PD = e.Cod                 -- справочник ошибок
                join dbo.s_Otdel        as o    on o.KOD = a.otd                -- отделения                  
                join dbo.kbol           as k    on a.KL = k.KL 
                left join dbo.s_Diag    as d    on d.KOD = a.D 
                left join dbo.s_Docum   as do   on k.Doc = do.KOD
                left join dbo.KLADR  as kl
                  on concat(right(concat('00', k.Obl),2), 
			            right(concat('000', k.KR), 3),
			            right(concat('000', k.Gorod), 3), 
			            right(concat('000', k.NasP), 3), '00') = kl.CODE
                left join dbo.astProtokol   as p                       -- находим протокол Метод ВМП (если есть)
                  on p.CodApstac = a.IND and p.NumShablon = 8013 and isnull(p.xDelete,0) = 0                                        
                {pWereStac}                                                             -- условия стационара
                union all
                select 4 as LPU_ST		-- тип подразделения (4 - параклиника, при выгрузке в XML превратить в 3)
	                ,cast(i.Cod as decimal) as Cod
	                ,p.pDate as DP
	                {pSelectAll}
                from dbo.parProtokol	as p
                join dbo.parObsledov	as o	on p.CodApstac = o.Cod
                join dbo.kbol			as k	on p.KL = k.KL
                join dbo.kbolInfo		as i	on i.Tab = 'par' and o.Cod = i.CodZap 
                join dbo.s_Diag			as d	on d.KOD = json_value(i.jTag, '$.Diag')
                join dbo.s_Docum		as do   on k.Doc = do.KOD 
                left join dbo.KLADR		as kl
                    on concat(right(concat('00', k.Obl),2), 
			                right(concat('000', k.KR), 3),
			                right(concat('000', k.Gorod), 3), 
			                right(concat('000', k.NasP), 3), '00') = kl.CODE
                left join dbo.ErrorASU as e on json_value(i.jTag, '$.Error') = e.Cod
                where NumShablon in (515, 518, 519, 101, 102, 103, 1301)
                {pWerePar}                                                               -- условия параклиника
                union all
                select 5 as LPU_ST		-- тип подразделения (5 - гистология, при выгрузке в XML превратить в 3)
	                ,cast(i.Cod as decimal) as Cod
	                ,p.pDate as DP
	                {pSelectAll}
				from dbo.kdlProtokol	as p
				join dbo.kbol			as k	on p.KL = k.KL
				join dbo.kbolInfo		as i	on i.Tab = 'kdl' and i.CodZap = p.Cod
                join dbo.s_Diag			as d	on d.KOD = json_value(i.jTag, '$.Diag')
                join dbo.s_Docum		as do   on k.Doc = do.KOD 
                left join dbo.KLADR		as kl
                    on concat(right(concat('00', k.Obl),2), 
			                right(concat('000', k.KR), 3),
			                right(concat('000', k.Gorod), 3), 
			                right(concat('000', k.NasP), 3), '00') = kl.CODE
                left join dbo.ErrorASU as e on json_value(i.jTag, '$.Error') = e.Cod
                where NumShablon = 20001
                {pWerePar}                                                               -- условия параклиника (гистология)
                order by FAM, I, O, DR";

            return _Return;
        }

        /// <summary>Загружаем данные поликлиники Apac 1</summary>
        /// <param name="pWerePol">Фрагмента sql для поликлинники</param>
        public static string ReApac_Select_1(string pWerePol)
        {
            string _Return = $@"
                    use Bazis

                    select cast(a.Cod as decimal) as Cod 
                          ,3 as LPU_ST
                          ,a.Scom                          
                          ,a1.Age
                          ,a1.DP as DN	                     
                          ,a.DP as DK                      
                          ,a.REZOBR
                          ,a.SN
                          ,a.SS
                          ,v.IDDOKT
                          ,v.Podrazd
                          ,v.PRVS                                                 -- специальность	
	                      ,a.NumberFirstTap    
                          ,s.PROFIL                                               -- профиль                         
                          ,s.VIDPOM                                               -- вид помощи
                          ,isnull(s.Tarif, 0) as Tarif                            -- тариф  
	                      ,iif(a.Cod <> a.NumberFirstTap, (select count(*) 
                                                           from dbo.APAC as a2
                                                           where a2.NumberFirstTap = a.NumberFirstTap), 1) as uet3   
                           ,iif(a.Cod <> a.NumberFirstTap, '3.0', '1.0') as P_CEL  -- цель посещения  
	                       ,iif(json_value (d.xInfo, '$.character') = 'ostr', 1,		-- 1 - острый характер заболевания
								iif(exists(select a2.Cod from dbo.APAC as a2 with (nolock) where a2.KL = a.KL and a2.D = a.D and year(a.DP) >  year(a2.DP)), 3,  -- 3 - хронический повторный, если был в прошлом году в поликлинике
									iif(exists(select a3.IND from dbo.APSTAC as a3 with (nolock) where a3.KL = a.KL and a3.D = a.D and year(a.DP) >  year(a3.DK)), 3, 2)))   as C_Zab  -- 3 - хронический повторный, если был в прошлом году в стационаре, иначе 2 - хронический впервые
                    from dbo.APAC               as a                                 -- поликлиника
                    join dbo.APAC               as a1   on a.NumberFirstTap = a1.Cod                  
                    left join dbo.s_VrachPol    as v    on a.KV = v.KOD
                    left join dbo.StrahStacSv   as s    on v.PRVS = s.PRVS and a.DP between s.DateN and s.DateK
		                and ((s.Flag = 2 and a.Cod = a.NumberFirstTap) or (s.Flag = 8 and a.Cod <> a.NumberFirstTap))
						and ((a1.Age > 17 and s.Child = 0) or (a1.Age < 18 and s.Child = 1))
                    left join dbo.s_Diag        as d     on d.KOD = a.D
                    where {pWerePol}";

            return _Return;
        }

        /// <summary>Загружаем данные поликлиники Apac 2</summary>
        /// <param name="pWerePol">Фрагмента sql для поликлинники</param>
        public static string ReApac_Select_2(string pWerePol)
        {
            string _Return = $@"
                     select t.Num	                  
	                  ,t.NumberFirstTap
	                  ,t.DP
	                  ,t.PRVS as PRVS_Usl
	                  ,t.IDDOKT as CODE_MD
	                  ,t.uet3
                      ,d.DStrah as D
	                  ,s.CODE_USL
	                  ,s.VID_VME
                      ,iif(t.NPR_MO = 0, 555509, t.NPR_MO) as NPR_MO
                      ,t.NPR_DATE
                      ,t.jTag
                from (
	                select row_number() over(partition by a2.NumberFirstTap order by a2.DP) as Num
		                  ,a2.DP
	                      ,a2.Cod
                          ,a2.NumberFirstTap
                          ,a2.D
	                      ,v.PRVS
		                  ,v.IDDOKT
		                  ,iif(a.Cod <> a.NumberFirstTap, (select count(*) from dbo.APAC as a1 where a1.NumberFirstTap = a.NumberFirstTap), 1) as uet3
		                  ,a.DateCloseTap
                          ,json_value(a2.xInfo, '$.LPUNapr') as NPR_MO          -- ЛПУ направления
                          ,try_cast(json_value(a2.xInfo, '$.DateNapr') as date) as NPR_DATE       -- дата направления
                          ,ki.jTag
	                from dbo.APAC as a2
	                left join dbo.s_VrachPol as v        on a2.KV = v.KOD
	                join dbo.APAC            as a        on {pWerePol} and a.NumberFirstTap = a2.NumberFirstTap and a.IsCloseTap = 2
                    left join dbo.kbolInfo   as ki       on ki.Tab = 'pol' and a2.Cod = ki.CodZap 
                ) as t
                left join dbo.StrahStacSv   as s                                                                          
                  on t.PRVS = s.PRVS and t.DateCloseTap between s.DateN and s.DateK
	                and ((s.Flag = 2 and uet3 = 1)
                      or (s.Flag = 8 and uet3 > 1 and Num = 1)
	                  or (s.Flag = 9 and uet3 > 1 and Num > 1))
                left join dbo.s_Diag as d
                  on d.KOD = t.D
                order by t.NumberFirstTap, t.DP";

            return _Return;
        }

        /// <summary>Загружаем данные поликлиники Apac 3 (консилиумы)</summary>
        /// <param name="pWerePol">Фрагмента sql для поликлинники</param>
        public static string ReApac_Select_3 (string pWerePol)
        {
            string _Return = $@"
                -- Дата и цель последнего консилиума
                use Bazis
                select	 cast(a.Cod as decimal) as Cod
		                ,iif(json_value(jTag, '$.Taktika_1') is not null, 1, iif(json_value(jTag, '$.Taktika_2') is not null, 2, 3)) as PR_CONS
		                ,isnull(isnull(json_value(jTag, '$.Taktika_1'), json_value(jTag, '$.Taktika_2')), json_value(jTag, '$.Taktika_3')) as DT_CONS		
                from (
	                select   a.Cod
			                ,kp.jTag
			                ,row_number() over(partition by a.Cod order by isnull(a1.DP, s1.DK) desc) as Num
	                from dbo.APAC               as a                                 
	                left join dbo.kbolInfo as kp on kp.KL = a.KL
	                left join dbo.APAC as a1 on kp.Tab = 'pol' and kp.CodZap = a1.Cod and a.DP >= a1.DP
	                left join dbo.APSTAC as s1 on kp.Tab = 'stac' and kp.CodZap = s1.IND and a.DP >= s1.DK
	                where                                                         
		                {pWerePol}
		                and isjson(kp.jTag) > 0
		                and kp.jTag like '%Taktika_%'
		                and (a1.DP is not null or s1.DK is not null)
                        and datediff(m, isnull(a1.DP, s1.DK), a.DP) < 7
                ) as a
                where Num = 1
                order by a.Cod";

            return _Return;
        }

        /// <summary>Загружаем данные стационара Apstac 1</summary>
        /// <param name="pWereStac">Фрагмента sql для стационара </param>
        public static string ReApstac_Select_1(string pWereStac)
        {
            string _Return = $@"
                use Bazis;

                select *
                    from (
                        select dense_rank() over(PARTITION BY a.IND order by s.Tarif desc, v.Nom) as Rang         
                            ,a.IND
                            ,iif(o.Tip = 1, 1, 2) as LPU_ST
                            ,iif(a.OtdIn = 0, a.DN, isnull(cast(json_value(a.xInfo, '$.DN') as date), a.DN)) as Dn
                            ,a.DK
                            ,a.Age
                            ,a.ScomEnd
                            ,a.SNEnd
                            ,a.SSEnd
                            ,a.FlagOut                                          -- результат обращения 1-выписан, 2-переведен, 3-умер
                            ,a.OtdIn                                            -- откуда прибыл пациент
                            ,a.ISXOD                                            -- исход госпитализации
                            ,iif(a.OtdIn = 0, a.UET3, isnull(json_value(a.xInfo, '$.Uet3'), a.UET3)) as Uet3 -- койко дни
                            ,o.Korpus                                           -- корпус отделения 1-главный, 2-филиал
                            ,o.Depart                                           -- тип отделения 1-кр. стационар, 2-дн. стационар при стац, 3-дн. стационар при пол.
                            ,o.PODR                                             -- код отделения
                            ,s.PROFIL                                           -- профиль врача
                            ,s.PROFIL_K                                         -- профиль койки
                            ,v.PRVSs                                            -- специальность врача                  
                            ,s.CODE_USL                                         -- код услуги
                            ,s.VID_VME                                          -- вид медицинского вмешательства
                            ,v.IDDOKT                                           -- код врача
                            ,s.Tarif                                            -- тариф                           
                            ,p.Cod as MetVMP                                    -- код протокола ВМП (если есть)
                            ,'' as VID_HMP
                            ,'' as METOD_HMP
                            ,'' as TAL_NUM
                            ,a.DN as TAL_D                            
                            ,555509 as NPR_MO                                   -- ЛПУ направления
                            ,cast(json_value(a.xInfo, '$.DateNapr') as date) as NPR_DATE       -- дата направления
                            ,ki.jTag
		                    ,iif(json_value (d.xInfo, '$.character') = 'ostr', 1,		-- 1 - острый характер заболевания
			                    iif(exists(select a2.Cod from dbo.APAC as a2 with (nolock) where a2.KL = a.KL and a2.D = a.D and year(a.DK) >  year(a2.DP)), 3,  -- 3 - хронический повторный, если был в прошлом году в поликлинике
				                    iif(exists(select a3.IND from dbo.APSTAC as a3 with (nolock) where a3.KL = a.KL and a3.D = a.D and year(a.DK) >  year(a3.DK)), 3, 2)))   as C_Zab  -- 3 - хронический повторный, если был в прошлом году в стационаре, иначе 2 - хронический впервые
                         from dbo.APSTAC as a                                   -- стационар   
                         left join dbo.ErrorASU	as e	on a.PD = e.Cod         -- справочник ошибок                                                  
                         join dbo.s_Otdel		as o    on o.KOD = a.otd        -- отделения
                         left join dbo.astProtokol   as p                       -- находим протокол Метод ВМП (если есть)
                           on p.CodApstac = a.IND and p.NumShablon = 8013 and isnull(p.xDelete,0) = 0
                         left join (select KOD, Nom, PRVSs, IDDOKT
                               from (select KOD, IDDOKT, PRVS, PRVS_1, PRVS_2, PRVS_3, PRVS_4
                                     from dbo.s_VrachStac) as pr 
                               unpivot
                               (PRVSs for Nom in (PRVS, PRVS_1, PRVS_2, PRVS_3, PRVS_4)) as un
                                where  PRVSs <> ''   ) as v 
                           on v.KOD = a.KV 
                              and not((a.OTD in (17, 18) and PRVSs = 41)        -- это радиологические отделения им не показываем онкологическую специальность, только радиологию 102 
                                   or (a.Age > 17 and PRVSs = 19)               -- если взрослый, то не берем специальность детскую онкологию 
                                   or (p.Cod is not null and PRVSs <> 41))      -- отсекаем для ВМП все специальности, кроме онкологии                                                                                  
                         left join dbo.StrahStacSv   as s                                                                          
                           on v.PRVSs = s.PRVS and a.DK between s.DateN and s.DateK   
							and ((s.Flag = 0 and o.Depart = 1) or (s.Flag = 1 and o.Depart in (2, 3)))   
                         left join dbo.kbolInfo as ki
                           on ki.Tab = 'stac' and a.IND = ki.CodZap    
                         left join dbo.s_Diag        as d     on d.KOD = a.D                 
                         {pWereStac}
                         ) as dd     
                    where Rang = 1  
                    order by IND";

            return _Return;
        }

        /// <summary>Загружаем данные кодов КСГ в файл стационара Apstac 2</summary>
        /// <param name="pWereStac">Фрагмента sql для стационара </param>
        public static string ReApstac_Select_2(string pWereStac)
        {
            string _Return = $@"
                use Bazis;

                declare @A as table (
                                    IND decimal,		-- Код стационара			
                                    D  nvarchar(5),		-- Диагноз стационара
					                Dat date,			-- Дата выписки                    
                                    UslOk tinyint,		-- 1 - круглосуточный, 2 - дневной
                                    Det tinyint  		-- 5 - ребенок до 18 лет, 6 - взрослый					
					                ) 
                insert @A
                select a.IND
	                    ,a.D
	                    ,a.DK
	                    ,iif(a.Dnevnoi = 1, 1, 2) as UslOk
	                    ,iif(a.Age < 18, 5, 6) as Det   
                from dbo.APSTAC as a
                join dbo.s_Otdel as o                                  
                    on o.KOD = a.otd
                left join dbo.astProtokol   as p                       -- находим протокол Метод ВМП (если есть)
                    on p.CodApstac = a.IND and p.NumShablon = 8013 and isnull(p.xDelete,0) = 0
                --where a.DK between '01/01/2019' and '01/21/2019' and a.OMS = 1 and a.FlagOut > 0  -- для проверки запроса
                --  and (left(a.D, 1) = 'C' or left(a.D, 2) = 'D0') -- or left(a.D, 3) = 'D70'
                {pWereStac}

                                       
                declare @T as table (
                                    IND decimal,		-- Код стационара
					                Tip nvarchar(4),	-- Тип услуги	
					                NomUsl int,			-- Номер услуги стационара (1 - только диагноз, 11 - услуга)
                                    D  nvarchar(5),		-- Диагноз стационара
                                    Usl nvarchar(15),	-- Услуга стационара
					                DopUsl nvarchar(15),-- Дополнительная услуга схема для лучевойхимии
					                Frakc int,			-- Фракции, для радиологии
                                    UslOk tinyint,		-- 1 - круглосуточный, 2 - дневной
                                    Det tinyint,		-- 5 - ребенок до 18 лет, 6 - взрослый
					                Dat date,   		-- Дата оказания услуги
                                    DK date,            -- Дата выписки
                                    xInfo nvarchar(max), -- поле xInfo из операций
					                DayHim int          -- Плановое количество дней введения химии
					                )    

                insert @T
                select IND, Tip, NomUsl, D, Usl, DopUsl, Frakc, UslOk, DET, Dat, DK, xInfo, DayHim
                from (
                -- Код услуги стационара
                select IND
	                    ,1 as NomUsl     
		                ,'диаг' as Tip
                        ,D					  
                        ,'' as Usl
		                ,'' as DopUsl
		                ,null as Frakc
                        ,UslOk
                        ,Det
	                    ,null as Dat
                        ,Dat as DK
                        ,'' as xInfo
                        ,0 as DayHim
                from @A 

                union
                -- Код Лечения
                select r.IND 
	                    ,row_number() over (partition by r.IND order by o.Cod) + 10 as NomUsl
		                ,'опер' as Tip     
                        ,r.D
                        ,o.OPER as Usl
		                ,isnull(json_value(o.xInfo, '$.Prep_HL'), '') as DopUsl
		                ,json_value(o.xInfo, '$.Frakci') as Frakc
                        ,r.UslOk		
                        ,r.Det
	                    ,o.Dat
                        ,r.Dat as DK
                        ,o.xInfo
                        ,iif(isjson(vo.xInfo ) > 0, isnull(json_value(vo.xInfo, '$.dayv'), 0), 0) as DayHim
                from @A as r
                join dbo.Oper as o          on r.IND = o.IND  
                join dbo.s_VidOper as vo    on o.Oper = vo.KOP and o.DAT between vo.xBeginDate and vo.xEndDate
                where isnull(o.xDelete, 0) = 0  
	                and isjson(o.xInfo )  > 0
	                and json_value(o.xInfo, '$.Himia') is null		-- убираем химию, которая не идет в оплату
                ) as r


                select IND, Tip, Nom, Usl, Dat, DopUsl, FrakcT
                    , KSG, Factor, UprFactor, KUSmo, Day3, xInfo, Frakc as FrakcText, DayHim
                    -- ,Num2, D1, NomUsl, DopUslT,  -- для тестов
                from(
                select  row_number() over(partition by t.IND order by k.Factor desc, len(g.D1) desc) as Nom                            
                         ,row_number() over(partition by t.IND, t.NomUsl order by k.Factor desc, len(g.D1) desc) as Num2
	                    -- старая строка, должны потом скорей всего вернуть
                        -- если есть и диагноз и услуга, а напротив только услуга, то берем ту где и услуга и диагноз
                        -- ,row_number() over(partition by t.IND, t.NomUsl order by iif(len(g.D1) > 0 and len(g.Usl) > 0, 0, 1), k.Factor desc, len(g.D1) desc) as Num2
                        ,t.Tip
                        ,t.IND
                        ,t.NomUsl
                        ,t.Dat
                        ,iif(t.Usl = '', g.D1, t.Usl) as Usl
                        ,g.Age
                        ,g.D1
                        ,g.ksg
                        ,g.DopUsl
                        ,g.Frakc
                        ,k.Factor
                        ,k.UprFactor
                        ,isnull(k.KUSmo, 0) as KUSmo
                        ,k.Day3
                        ,t.xInfo
                        ,t.DopUsl as DopUslT
                        ,t.Frakc as FrakcT
                        ,t.DayHim
                from @T as t
                left join dbo.StrahKsgGroup2019 as g
                    on t.UslOk = g.UslOk and t.Usl = g.Usl
                        and t.DopUsl = g.DopUsl
                        and(g.D1 = '' or t.D = g.D1 or left(t.D, 1) + '.' = g.D1 or left(t.D, 3) = g.D1 or 
                            (left(t.D, 1) = 'C' and g.D1 = 'C00-C80') or (left(t.D, 1) = 'D' and g.D1 = 'D00-D09'))
                        and t.DK between g.xBeginDate and g.xEndDate
                left join dbo.StrahKsg as k
                    on k.UslOk = t.UslOk and k.KSG = g.KSG and t.DK between k.xBeginDate and k.xEndDate
                where(t.Det = g.Age or g.Age is null)
                        and not(g.D1 = 'C00-C80' and not left(t.D, 3) between 'C00' and 'C80') -- отсекаем химию с диагнозами, которые не входят в диапазон C00 - C80
                        and not(g.D1 = 'D00-D09' and not left(t.D, 3) between 'D00' and 'D09')
                        and (g.Frakc = '' or (t.Frakc between left(right(g.Frakc, 5), 2) and right(g.Frakc, 2)))
                ) as f
                where not (NomUsl = 1 and Nom <> 1)     -- удаляем КСГ чисто по диагнозу, если они не выигрывали тендер по цене
                    and Num2 = 1                        -- оставляем для каждой услуги, только выигрышную КСГ
                order by IND, Nom";

            return _Return;
        }

        /// <summary>Загружаем данные стационара Apstac 3 (консилиумы)</summary>
        /// <param name="pWereStac">Фрагмента sql для стационара </param>
        public static string ReApstac_Select_3(string pWereStac)
        {
            string _Return = $@"
                -- Дата и цель последнего консилиума
                use Bazis
                select	 IND
		                ,iif(json_value(jTag, '$.Taktika_1') is not null, 1, iif(json_value(jTag, '$.Taktika_2') is not null, 2, 3)) as PR_CONS
		                ,isnull(isnull(json_value(jTag, '$.Taktika_1'), json_value(jTag, '$.Taktika_2')), json_value(jTag, '$.Taktika_3')) as DT_CONS		
                from (
	                select   a.IND
			                ,kp.jTag
			                ,row_number() over(partition by a.IND order by isnull(a1.DP, s1.DK) desc) as Num
	                from dbo.APSTAC               as a  
                    left join dbo.ErrorASU	as e	on a.PD = e.Cod         -- справочник ошибок                                                  
                    join dbo.s_Otdel		as o    on o.KOD = a.otd        -- отделения
	                left join dbo.kbolInfo as kp on kp.KL = a.KL
	                left join dbo.APAC as a1 on kp.Tab = 'pol' and kp.CodZap = a1.Cod and a.DK >= a1.DP
	                left join dbo.APSTAC as s1 on kp.Tab = 'stac' and kp.CodZap = s1.IND and a.DK >= s1.DK
                    left join dbo.astProtokol   as p                       -- находим протокол Метод ВМП (если есть)
                      on p.CodApstac = a.IND and p.NumShablon = 8013 and isnull(p.xDelete,0) = 0
	                {pWereStac}
		                and isjson(kp.jTag) > 0
		                and kp.jTag like '%Taktika_%'
		                and (a1.DP is not null or s1.DK is not null)
                        and datediff(m, isnull(a1.DP, s1.DK), a.DK) < 7
                ) as a
                where Num = 1
                order by a.IND";

            return _Return;
        }

        /// <summary>Загружаем данные параклиники parObsledov</summary>
        /// <param name="pWerePar">Фрагмента sql для параклиники </param>
        public static string RePar_Select_1(string pWerePar)
        {
            string _Return = $@"
                use Bazis;

                select cast(i.Cod as decimal) as Cod
                    ,4 as LPU_ST
	                ,k.SCom
	                ,k.SN
	                ,k.SS
	                ,i.jTag
	                ,json_value(d.xInfo, '$.vrach')	as IDDOKT
                    ,datediff(year, k.DR, p.pDate) as Age
	                ,s.PRVS
	                ,s.PROFIL
	                ,s.CODE_USL
	                ,s.VID_VME
	                ,s.VidPom
	                ,s.Tarif
	                ,isnull(l.KodOKPO, '555509') as NPR_MO
                    ,p.pDate as NPR_DATE
                from dbo.parProtokol	as p
                join dbo.parObsledov	as o	on p.CodApstac = o.Cod
                join dbo.kbol			as k	on p.KL = k.KL
                join dbo.kbolInfo		as i	on i.Tab = 'par' and o.Cod = i.CodZap  
                join dbo.s_UsersDostup  as d	on p.xUserUp = d.UserCod and isjson(d.xInfo) > 0 and json_value(d.xInfo, '$.modul') = 15 and json_value(d.xInfo, '$.element') in (1, 6, 13)
                join dbo.StrahStacSv	as s	on s.Flag = 10 and p.pDate between s.DateN and s.DateK and json_value(i.jTag, '$.Usl') = s.CODE_USL
                left join dbo.s_LPU		as l	on k.StrLPU = l.StrLPU 
                where NumShablon in (515, 518, 519, 101, 102, 103, 1301)
                {pWerePar}
                union all
                select cast(i.Cod as decimal) as Cod
                        ,5 as LPU_ST
		                ,k.SCom
	                    ,k.SN
	                    ,k.SS
		                ,i.jTag
		                ,json_value(d.xInfo, '$.vrach') as IDDOKT
		                ,datediff(year, k.DR, p.pDate) as Age
		                ,s.PRVS
	                    ,s.PROFIL
	                    ,s.CODE_USL
	                    ,s.VID_VME
	                    ,s.VidPom
	                    ,s.Tarif
		                ,json_value(i.jTag, '$.NPR_MO') as NPR_MO
		                ,convert(date, json_value(i.jTag, '$.NPR_DATE'), 104) as NPR_DATE
                from dbo.kdlProtokol    as p
                join dbo.kbol           as k on p.KL = k.KL
                join dbo.kbolInfo       as i on i.Tab = 'kdl' and i.CodZap = p.Cod
                join dbo.s_UsersDostup as d    on p.xUserUp = d.UserCod and isjson(d.xInfo) > 0 and json_value(d.xInfo, '$.userrigth') = 'histologydoctor' and json_value(d.xInfo, '$.vrach') is not null
                join dbo.StrahStacSv as s    on s.Flag = 11 and p.pDate between s.DateN and s.DateK and json_value(i.jTag, '$.Usl') = s.VID_VME
                where p.NumShablon = 20001
                {pWerePar}";

            return _Return;
        }

        /// <summary>Загружаем данные по пациенту ВМП</summary>
        /// <param name="pIND">Код стационара</param>
        public static string ReVMP_Select_1(string pIND)
        {
            string _Return = $@"
                select s.PROFIL                                             -- профиль врача
                        ,s.PROFIL_K                                         -- профиль койки  
                        ,s.CODE_USL                                         -- код услуги
                        ,sv.IDHM as VID_VME                                 -- вид медицинского вмешательства                                               
                        ,t.Tarif                                            -- тариф                                    
                        ,sv.IDHM as METOD_HMP                               -- код метода ВМП
                        ,sv.HVid as VID_HMP                                 -- код вида ВМП
                        ,dbo.GetPole(8, p.Protokol) as TAL_NUM                   -- номер талона ВМП
                        ,cast(dbo.GetPoleD(9, p.Protokol) as date) as TAL_D      -- дата талона ВМП   
                from dbo.APSTAC as a                                        -- стационар   
                left join dbo.astProtokol   as p    on p.CodApstac = a.IND and p.NumShablon = 8013  
                left join dbo.StrahVMP      as sv   on p.Cod is not null and left(dbo.GetPole(6, p.Protokol), 3) = sv.IDHM    --  метод ВМП находиться в 6м вопросе 8013 шаблона стационара                                                    
                left join dbo.Tarif         as t    on p.Cod is not null and t.Flag = 7 and t.TCod = sv.HVid                        
                left join dbo.StrahStacSv   as s    on s.PRVS=41 and t.Flag = s.Flag and a.DK between s.DateN and s.DateK                                                                                           
                where t.DateN <= a.DK and (t.DateK >= a.DK or t.DateK is null) and a.IND = {pIND}"; 
 
            return _Return;
        }

        /// <summary>Загружаем Страховые компании</summary>
        public static string ReStrahComp_Select_1()
        {
            string _Return = @"
                select s.KOD    as Cod
                      ,s.OGRN   as OGRN
                      ,s.TKOD   as Names
                      ,o.NewKod as OKATO  
                from dbo.s_StrahComp as s
                join dbo.s_Oblast    as o    
                  on o.kod = s.KodReg";

            return _Return;
        }
        #endregion                                                           
    }
}
