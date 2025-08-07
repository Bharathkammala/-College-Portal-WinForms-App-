using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CollegePortal
{
    public static class ListHelpers
    {
        public static List<T> ConvertToClass<T>(DataTable dataTable) where T : class, new()
        {
            if (!typeof(T).IsDefined(typeof(DataContractAttribute), true))
            {
                throw new InvalidOperationException($"The class {typeof(T).Name} must have a DataContract attribute.");
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => p.IsDefined(typeof(DataMemberAttribute), true))
                                      .ToList();

            var result = new List<T>();

            foreach (DataRow row in dataTable.Rows)
            {
                var instance = new T();

                foreach (var property in properties)
                {
                    var columnName = property.GetCustomAttribute<DataMemberAttribute>()?.Name ?? property.Name;

                    if (dataTable.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
                    {
                        property.SetValue(instance, Convert.ChangeType(row[columnName], property.PropertyType));
                    }
                }

                result.Add(instance);
            }

            return result;
        }
    }
}
// This code defines a static class ListHelpers with a method ConvertToClass that converts a DataTable to a list of objects of type T.