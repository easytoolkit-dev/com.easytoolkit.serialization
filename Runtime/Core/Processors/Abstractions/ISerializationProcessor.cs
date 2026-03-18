using System;
using EasyToolkit.Serialization.Formatters;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    public interface ISerializationProcessor : ISerializationSettingsAccessor
    {
        /// <summary>
        /// Gets the value type this serializer is designed for.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets or sets the parent processor in the serialization hierarchy.
        /// </summary>
        /// <remarks>
        /// The parent is null for root processors. This allows processors to understand
        /// their position in the serialization hierarchy and adjust behavior accordingly.
        /// </remarks>
        [CanBeNull]
        ISerializationProcessor Parent { get; set; }

        /// <summary>
        /// Gets or sets the serialization context associated with this processor.
        /// </summary>
        /// <remarks>
        /// The context provides runtime configuration for serialization behavior,
        /// including reflection settings and processor cache. Processors within
        /// the same serialization hierarchy share the same context.
        /// </remarks>
        SerializationContext Context { get; set; }

        bool CanProcess(Type valueType, SerializationContext context);

        void ProcessUntyped(ref object value, IDataFormatter formatter);
        void ProcessUntyped(string name, ref object value, IDataFormatter formatter);
    }

    public interface ISerializationProcessor<T> : ISerializationProcessor
    {
        /// <summary>
        /// Processes a strongly-typed value during serialization or deserialization.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        void Process(ref T value, IDataFormatter formatter);

        /// <summary>
        /// Processes a strongly-typed value with a member name during serialization or deserialization.
        /// </summary>
        /// <param name="name">The member name being processed.</param>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        void Process(string name, ref T value, IDataFormatter formatter);
    }
}
