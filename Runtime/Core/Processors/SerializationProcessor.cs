using System;

namespace EasyToolKit.Serialization
{
    public abstract class SerializationProcessor<T> : ISerializationProcessor<T>
    {
        /// <summary>
        /// Gets the shared serialization context for accessing services.
        /// </summary>
        protected static SerializationEnvironment Environment => SerializationEnvironment.Instance;

        private bool _isInitialized;

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
        public virtual bool CanProcess(Type valueType) => valueType == typeof(T);

        /// <summary>
        /// Processes a strongly-typed value during serialization or deserialization.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        protected virtual void Process(ref T value, IDataFormatter formatter)
        {
            Process(null, ref value, formatter);
        }

        /// <summary>
        /// Processes a strongly-typed value with a member name during serialization or deserialization.
        /// </summary>
        /// <param name="name">The member name being processed.</param>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        protected abstract void Process(string name, ref T value, IDataFormatter formatter);

        protected virtual void Initialize() { }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        void ISerializationProcessor<T>.Process(ref T value, IDataFormatter formatter)
        {
            EnsureInitialize();
            Process(ref value, formatter);
        }

        void ISerializationProcessor<T>.Process(string name, ref T value, IDataFormatter formatter)
        {
            EnsureInitialize();
            Process(name, ref value, formatter);
        }
    }
}
