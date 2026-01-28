using System;
using EasyToolkit.Core.Mathematics;

namespace EasyToolkit.Serialization.Processors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ProcessorConfigurationAttribute : Attribute
    {
        private static readonly ProcessorConfigurationAttribute DefaultImpl = new ProcessorConfigurationAttribute();

        /// <summary>
        /// Gets the default configuration instance.
        /// </summary>
        public static ProcessorConfigurationAttribute Default => DefaultImpl;

        /// <summary>
        /// Gets the priority level for this serializer.
        /// Higher priority serializers are evaluated first during type matching.
        /// </summary>
        public OrderPriority Priority { get; }

        public bool AllowTypeArgumentInheritance { get; set; }

        /// <summary>
        /// Initializes a new instance with Custom priority level.
        /// </summary>
        public ProcessorConfigurationAttribute()
            : this(ProcessorPriorityLevel.Custom)
        {
        }

        /// <summary>
        /// Initializes a new instance with the specified priority and inheritance behavior.
        /// </summary>
        public ProcessorConfigurationAttribute(double priority)
        {
            Priority = new OrderPriority(priority);
        }
    }
}
