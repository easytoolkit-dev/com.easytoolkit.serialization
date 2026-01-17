using System;

namespace EasyToolKit.Serialization
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DependencyProcessorAttribute : Attribute
    {
    }
}
