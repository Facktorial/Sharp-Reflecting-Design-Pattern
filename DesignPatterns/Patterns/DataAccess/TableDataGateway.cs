using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    public class TableDataGateway<T> : PatternPrototype<IdWrapper<T>>, ICRUDObj<IdWrapper<T>>
        where T : new()
    {
        public new T DomainObject
        {
            get { throw new Exception("Access to this field is disabled"); }
        }

        public TableDataGateway(SqlConnector sql) : base(sql) { }

        public void Save(IdWrapper<T> obj)
        {
            string sql = $"INSERT INTO {SQLHelper<T>.TableName()} ({SQLHelper<T>.GetColumnNames()}) VALUES ({SQLHelper<T>.GetParameterNames()});";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }
        public void Update(IdWrapper<T> obj)
        {
            string sql = $"UPDATE {SQLHelper<T>.TableName()} SET {SQLHelper<T>.GetSetExpressions()} WHERE Id = @Id;";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }

        public void Delete(IdWrapper<T> obj)
        {
            string sql = $"DELETE FROM {SQLHelper<T>.TableName()} WHERE Id = @Id;";
            var fn = (object _) => obj.Id;
            SQLAction(sql, obj, fn);
        }

        public IdWrapper<T>? Find(int id)
        {
            PropertyInfo prop = typeof(IdWrapper<T>).GetProperty("Id");

            var fn = GetObject(SqlConnect, id);
            return FindInner(id, SqlConnect, fn);
        }

        public List<IdWrapper<T>> FindByProperty(string propertyName, object value)
        {
            PropertyInfo prop = typeof(T).GetProperty(propertyName);

            var fn = GetList(SqlConnect, prop, value);
            return FindInner<List<IdWrapper<T>>>(SqlConnect, prop, value, fn);
        }

        private static Func<SQLTypes, IdWrapper<T>> GetObject(SqlConnector conn, int value)
        {
            return (SQLTypes sql) =>
            {
                if (!sql.Reader.Read()) { return new IdWrapper<T>(new T()); }

                IdWrapper<T> record = new IdWrapper<T>(new T());
                SQLHelper<IdWrapper<T>>.MapProperties(sql.Reader, record, value);
                return record;
            };
        }
        private static Func<SQLTypes, List<IdWrapper<T>>> GetList(
            SqlConnector conn, PropertyInfo prop, object value
        )
        { 
            return (SQLTypes sql) => {
                var ls = new List<IdWrapper<T>>();

                while (sql.Reader.Read())
                {
                    IdWrapper<T> record = new IdWrapper<T>(new T());
                    SQLHelper<IdWrapper<T>>.MapProperties(sql.Reader, record);
                    prop.SetValue(record, value);

                    ls.Add(record);
                }
                
                return ls;
            };
        }

        public override string ToString()
        {
            return ToString(this.GetType().Name.Split('`')[0]);
        }
    }
}
