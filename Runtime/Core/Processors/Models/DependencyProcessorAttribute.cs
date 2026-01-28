using System;

namespace EasyToolkit.Serialization.Processors
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DependencyProcessorAttribute : Attribute
    {
    }
}
