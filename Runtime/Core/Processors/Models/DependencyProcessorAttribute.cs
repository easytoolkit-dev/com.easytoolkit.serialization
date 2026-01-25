using System;

namespace EasyToolKit.Serialization.Processors
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DependencyProcessorAttribute : Attribute
    {
    }
}
