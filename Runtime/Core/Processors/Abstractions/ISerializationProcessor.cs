using System;

namespace EasyToolKit.Serialization
{
    public interface ISerializationProcessor
    {
        /// <summary>
        /// Gets the value type this serializer is designed for.
        /// </summary>
        Type ValueType { get; }

        bool CanProcess(Type valueType);

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
