﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace wpfStatic
{
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Data;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ComponentModel;
    using System;






    [global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Bazis")]
    public partial class BazisDataContext : System.Data.Linq.DataContext
    {



        private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();



    #region Определения метода расширяемости
    partial void OnCreated();
    partial void Inserts_Department(s_Department instance);
    partial void Updates_Department(s_Department instance);
    partial void Deletes_Department(s_Department instance);
    #endregion



        public BazisDataContext() :

                base(global::wpfStatic.Properties.Settings.Default.BazisConnectionString, mappingSource)
        {
            OnCreated();
        }



        public BazisDataContext(string connection) :

                base(connection, mappingSource)
        {
            OnCreated();
        }



        public BazisDataContext(System.Data.IDbConnection connection) :

                base(connection, mappingSource)
        {
            OnCreated();
        }



        public BazisDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) :

                base(connection, mappingSource)
        {
            OnCreated();
        }



        public BazisDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) :

                base(connection, mappingSource)
        {
            OnCreated();
        }



        public System.Data.Linq.Table<s_Department> s_Department
        {
            get
            {
                return this.GetTable<s_Department>();
            }
        }
    }



    [global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.s_Department")]
    public partial class s_Department : INotifyPropertyChanging, INotifyPropertyChanged
    {



        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);



        private int _Cod;



        private string _Names;



        private byte _Korpus;



        private byte _Tip;



        private System.Nullable<byte> _xDelete;



        private System.Nullable<byte> _xImport;



    #region Определения метода расширяемости
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnCodChanging(int value);
    partial void OnCodChanged();
    partial void OnNamesChanging(string value);
    partial void OnNamesChanged();
    partial void OnKorpusChanging(byte value);
    partial void OnKorpusChanged();
    partial void OnTipChanging(byte value);
    partial void OnTipChanged();
    partial void OnxDeleteChanging(System.Nullable<byte> value);
    partial void OnxDeleteChanged();
    partial void OnxImportChanging(System.Nullable<byte> value);
    partial void OnxImportChanged();
    #endregion



        public s_Department()
        {
            OnCreated();
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Cod", DbType="Int NOT NULL", IsPrimaryKey=true)]
        public int Cod
        {
            get
            {
                return this._Cod;
            }
            set
            {
                if ((this._Cod != value))
                {
                    this.OnCodChanging(value);
                    this.SendPropertyChanging();
                    this._Cod = value;
                    this.SendPropertyChanged("Cod");
                    this.OnCodChanged();
                }
            }
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Names", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
        public string Names
        {
            get
            {
                return this._Names;
            }
            set
            {
                if ((this._Names != value))
                {
                    this.OnNamesChanging(value);
                    this.SendPropertyChanging();
                    this._Names = value;
                    this.SendPropertyChanged("Names");
                    this.OnNamesChanged();
                }
            }
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Korpus", DbType="TinyInt NOT NULL")]
        public byte Korpus
        {
            get
            {
                return this._Korpus;
            }
            set
            {
                if ((this._Korpus != value))
                {
                    this.OnKorpusChanging(value);
                    this.SendPropertyChanging();
                    this._Korpus = value;
                    this.SendPropertyChanged("Korpus");
                    this.OnKorpusChanged();
                }
            }
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Tip", DbType="TinyInt NOT NULL")]
        public byte Tip
        {
            get
            {
                return this._Tip;
            }
            set
            {
                if ((this._Tip != value))
                {
                    this.OnTipChanging(value);
                    this.SendPropertyChanging();
                    this._Tip = value;
                    this.SendPropertyChanged("Tip");
                    this.OnTipChanged();
                }
            }
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_xDelete", DbType="TinyInt")]
        public System.Nullable<byte> xDelete
        {
            get
            {
                return this._xDelete;
            }
            set
            {
                if ((this._xDelete != value))
                {
                    this.OnxDeleteChanging(value);
                    this.SendPropertyChanging();
                    this._xDelete = value;
                    this.SendPropertyChanged("xDelete");
                    this.OnxDeleteChanged();
                }
            }
        }



        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_xImport", DbType="TinyInt")]
        public System.Nullable<byte> xImport
        {
            get
            {
                return this._xImport;
            }
            set
            {
                if ((this._xImport != value))
                {
                    this.OnxImportChanging(value);
                    this.SendPropertyChanging();
                    this._xImport = value;
                    this.SendPropertyChanged("xImport");
                    this.OnxImportChanged();
                }
            }
        }



        public event PropertyChangingEventHandler PropertyChanging;



        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, emptyChangingEventArgs);
            }
        }



        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
#pragma warning restore 1591