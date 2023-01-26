using System;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BackingFieldAttribute : Attribute
    {
        public BackingFieldAttribute(string fieldName)
        {
            fieldName = fieldName;
        }

        public string fieldName { get; }
    }
}
