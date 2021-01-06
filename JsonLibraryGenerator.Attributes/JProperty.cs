using System;

namespace JsonLibraryGenerator.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JProperty:Attribute
    {
        public JProperty(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
