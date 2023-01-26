using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatterns
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NullableColumnAttribute : Attribute
    {
        public bool IsNullable { get; set; } = true;

        public NullableColumnAttribute(bool isNullable)
        {
            IsNullable = isNullable;
        }
    }
}
