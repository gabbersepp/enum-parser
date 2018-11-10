using System;

namespace EnumParser
{
    public class EnumNameAttribute : Attribute
    {
        public string Value { get; }

        public EnumNameAttribute(string value = null)
        {
            Value = value;
        }
    }
}