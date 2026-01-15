using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Specifies the priority of a serialization structure resolver.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SerializationResolverPriorityAttribute : Attribute
    {
        /// <summary>
        /// Gets the priority value. Higher values are checked first.
        /// </summary>
        public double Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationResolverPriorityAttribute"/> class.
        /// </summary>
        /// <param name="priority">The priority value.</param>
        public SerializationResolverPriorityAttribute(double priority)
        {
            Priority = priority;
        }
    }
}
