using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Data.Sqlite;

namespace DesignPatterns
{
    // FIXME
    using SQLCommand = SqliteCommand;
    using SQLReader = SqliteDataReader;

    public class SQLHelper<T>
    {
        public static string GetColumnNames()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string columnNames = "Id, " + string.Join(", ", Array.ConvertAll(
                properties, p => p.Name));

            return columnNames;
        }

        public static string GetParameterNames()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string parameterNames = "@Id, " + string.Join(", ", Array.ConvertAll(
                properties, p => "@" + p.Name));

            return parameterNames;
        }

        public static string GetSetExpressions()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            string[] setExpressions = Array.ConvertAll(
                properties, p => p.Name + " = @" + p.Name);

            return string.Join(", ", setExpressions);
        }

        public static string TableName() => typeof(T).Name;


        public static void MapParameters(SQLCommand command, T? obj, int id)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            Console.WriteLine("MapParameters: id: " + id);
            command.Parameters.AddWithValue("@" + "Id", id);
            Console.WriteLine("MapParameters: cmd: " + command.CommandText);

            if (obj == null)
            {
                Console.WriteLine("MapParameters: cmd: " + command.CommandText);
                return;
            }

            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj);
                command.Parameters.AddWithValue("@" + property.Name, value);
            }
        }

        public static void MapParameters(SQLCommand command, int id)
        {
            Console.WriteLine("no implementaion");
            //Console.WriteLine("MapParameters: id: " + id);
            //command.Parameters.AddWithValue("@" + "Id", id);
            //Console.WriteLine("MapParameters: cmd: " + command.CommandText);
        }

        public static void MapProperties(SQLReader reader, T obj)
        {
            // FIXME
            MapProperties(reader, obj, -1);
        }

        public static void MapProperties(SQLReader reader, T obj, int id)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                object value = reader[property.Name];
                //Console.WriteLine($"{property.Name} {value}");

                switch (value)
                {
                    case int intValue:
                        property.SetValue(obj, intValue);
                        break;
                    case long longValue:
                        if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(obj, (int)longValue);
                        }
                        else
                        {
                            property.SetValue(obj, longValue);
                        }
                        break;
                    case string stringValue:
                        property.SetValue(obj, stringValue);
                        break;
                        // Add cases for other data types as needed
                }
            }
        }
    }
}
