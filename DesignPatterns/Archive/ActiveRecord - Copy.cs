using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Data.Sqlite;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace DesignPatterns
{
    using SQLConnection = SqliteConnection;
    using SQLReader = SqliteDataReader;
    using SQLCommand = SqliteCommand;

    public class MethodCopy
    {
        public MethodCopy(string str) { Name = str; }
        public string Name { get; set; }
    }

    public class ActiveRecordCopy<T>
        //: TypeGenerator<T>
        : ICRUD<T>, IIdableObject
        where T : new()
    {
        public int Id { get; set; }

        //[BackingField("_domainObject")]
        public T DomainObject; // { get; set; }

        public T Obj { get { return DomainObject; } set { this.DomainObject = value; } }

        public SqlConnector SqlConnect { get; set; }

        private FieldInfo _wrappedField;
        private object _activeRecordInstance;

        private static object _lock = new object();

        private static Type? _activeRecordType;
        private static Type defaultType = typeof(ActiveRecord<T>);

        public ActiveRecordCopy()
        {
            //Type tmp = T.GetType();
            Console.WriteLine(typeof(T));
            Obj = new T ();
            Id = -1;
            SqlConnect = new SqlConnector("new to be properly initialized");

            if (_activeRecordType == null)
            {
                _wrappedField = defaultType.GetField("DomainObject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }

            TypeCreation();
        }

        public ActiveRecordCopy(T obj, SqlConnector sql)
        {
            Id = -1;
            DomainObject = obj;
            SqlConnect = sql;

            if (_activeRecordType == null)
            {
                _wrappedField = defaultType.GetField("DomainObject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }
            //_wrappedField = defaultType.GetField("DomainObject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            //FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            //_wrappedField = fields.FirstOrDefault(f => f.Name == "DomainObject");
            //_activeRecordType = CreateType(_lock, _activeRecordType, this.GetType().Name, _wrappedField, DomainObject, nameof(DomainObject));
            //TypeCreation();
        }

        public ActiveRecordCopy(SqlConnector sql, params object[] attributeValues)
        {
            Id = -1;
            SqlConnect = sql;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            if (properties.Length != attributeValues.Length )
            {
                throw new ArgumentException("Number of attribute values does not match number of properties in domain object.");
            }

            DomainObject = (T) Activator.CreateInstance(type);
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i].SetValue(DomainObject, attributeValues[i]);
            }

            if (_activeRecordType== null)
            {
                _wrappedField = defaultType.GetField("DomainObject", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }

            //_activeRecordType = CreateType(_lock, _activeRecordType, this.GetType().Name, _wrappedField, DomainObject, nameof(DomainObject));
            //TypeCreation();
        }

        private void TypeCreation()
        {
            if (_activeRecordType != null) { CreateActiveRecordInstance(); return;  }
            lock (_lock)
            {
                if (_activeRecordType != null) { return; }
                // Create wrapper properties for all of the properties of the DomainObject field
                //PropertyInfo[] domainObjectProperties = typeof(T).GetProperties();
                //string[] propertyNames = domainObjectProperties.Select(p => p.Name).ToArray();
                //_activeRecordType = TypeGenerator.CreateType(
                //    this.GetType(), this.GetType().Name, _wrappedField,
                //    DomainObject, nameof(DomainObject), propertyNames
                //);
                _activeRecordType = TypeGenerator<T>.CreateType(
                    this.GetType().Name, _wrappedField,
                    DomainObject, this.GetType()
                );
                //object activeRecordInstance = Activator.CreateInstance(_activeRecordType, new object[] { });
                ////DomainObject, SqlConnect });
                //((ActiveRecord<T>)activeRecordInstance).Obj = DomainObject;
                //((ActiveRecord<T>)activeRecordInstance).SqlConnect = SqlConnect;
                //_wrappedField.SetValue(this, activeRecordInstance);
            }
            CreateActiveRecordInstance();
        }

        private void CreateActiveRecordInstance()
        {
            _activeRecordInstance = Activator.CreateInstance(_activeRecordType, new object[] { });
            ((ActiveRecord<T>)_activeRecordInstance).DomainObject = DomainObject;
            ((ActiveRecord<T>)_activeRecordInstance).SqlConnect = SqlConnect;
            _wrappedField.SetValue(this, _activeRecordInstance);
        }

        public object this[string propertyName]
        {
            get
            {
                PropertyInfo property = DomainObject.GetType().GetProperty(propertyName);
                return property.GetValue(DomainObject);
            }
            set
            {
                PropertyInfo property = DomainObject.GetType().GetProperty(propertyName);
                property.SetValue(DomainObject, value);
            }
        }

        public Func<object[], object> this[Method methodName]
        {
            get
            {
                var method = typeof(T).GetMethod(methodName.Name);
                return (object[] args) => InvokeMethod(method, args);
            }
        }

        public object InvokeMethod(MethodInfo method, params object[] args)
        {
            return method.Invoke(DomainObject, args);
        }

        public static int GetMaxId(SQLConnection connect)
        {
            string tableName = typeof(T).Name;
            string sql = $"SELECT MAX(Id) FROM {tableName}";
            SqliteCommand command = new SqliteCommand(sql, connect);
            object maxID = command.ExecuteScalar();
            maxID = maxID == "" ? "0" : maxID;
                
            return (maxID == DBNull.Value) ? 1 : Convert.ToInt32(maxID) + 1;
        }

        private void SQLAction(string sql, Func<SQLConnection, int>? getID = null)
        {
            using (SQLConnection connection = new SQLConnection(SqlConnect.ConnectionString))
            {
                connection.Open();

                if (getID != null)
                {
                    int index = getID(connection);
                    PropertyInfo idProperty = DomainObject.GetType().GetProperty("Id");
                    Id = index;
                }
             
                SQLCommand command = new SQLCommand(sql, connection);
                SQLHelper<T>.MapParameters(command, DomainObject, Id);
                Console.WriteLine("command: " + command.CommandText);
                command.ExecuteNonQuery();
            }
        }

        public void Save()
        {
            string sql = $"INSERT INTO {SQLHelper<T>.TableName()} ({SQLHelper<T>.GetColumnNames()}) VALUES ({SQLHelper<T>.GetParameterNames()});";
            SQLAction(sql, GetMaxId);
        }

        public void Update()
        {
            string sql = $"UPDATE {SQLHelper<T>.TableName()} SET {SQLHelper<T>.GetSetExpressions()} WHERE Id = @Id;";
            SQLAction(sql);
        }

        public void Delete()
        {
            string sql = $"DELETE FROM {SQLHelper<T>.TableName()} WHERE Id = @Id;";
            SQLAction(sql);
        }

        public static ActiveRecord<T> Find(int id, SqlConnector conn) // SqlConnect.ConnectionString
        {
            using (SQLConnection connection = new SQLConnection(conn.ConnectionString))
            {
                connection.Open();

                string tableName = typeof(T).Name;
                string sql = $"SELECT * FROM {tableName} WHERE Id = @Id";

                SQLCommand command = new SQLCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);

                using (SQLReader reader = command.ExecuteReader())
                {
                    Console.WriteLine(reader.ToString());

                    if (!reader.Read()) { return new ActiveRecord<T>(new T(), conn); }

                    T activeRecord = new T();
                    ActiveRecord<T> tmp = new ActiveRecord<T>(activeRecord, conn);
                    SQLHelper<T>.MapProperties(reader, tmp.DomainObject, GetMaxId(connection));
                    return tmp;
                }
            }
        }
    }
}
