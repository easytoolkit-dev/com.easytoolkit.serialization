using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Abstract base class for strongly-typed serializers.
    /// Provides default implementations and common functionality.
    /// </summary>
    /// <typeparam name="T">The type this serializer handles.</typeparam>
    public abstract class EasySerializer<T> : IEasySerializer<T>
    {
        /// <summary>
        /// Gets the shared serialization context for accessing services.
        /// </summary>
        protected static SerializationEnvironment Environment => SerializationEnvironment.Instance;

        /// <summary>
        /// Gets the value type this serializer handles.
        /// </summary>
        public Type ValueType => typeof(T);

        /// <summary>
        /// Determines whether the specified value type can be serialized.
        /// Default implementation uses exact type matching.
        /// </summary>
        /// <param name="valueType">The type to validate for serialization support.</param>
        /// <returns>True if the type can be serialized; otherwise, false.</returns>
        public virtual bool CanSerialize(Type valueType) => valueType == typeof(T);

        /// <summary>
        /// Processes a strongly-typed value during serialization or deserialization.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        public void Process(ref T value, IDataFormatter formatter)
        {
            Process(null, ref value, formatter);
        }

        /// <summary>
        /// Processes a strongly-typed value with a member name during serialization or deserialization.
        /// </summary>
        /// <param name="name">The member name being processed.</param>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        public abstract void Process(string name, ref T value, IDataFormatter formatter);
    }
}
