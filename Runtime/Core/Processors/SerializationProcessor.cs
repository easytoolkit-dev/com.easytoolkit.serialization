using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    public abstract class SerializationProcessor<T> : ISerializationProcessor<T>
    {
        private bool _isInitialized;
        private bool _isRoot;

        /// <summary>
        /// Gets the value type this serializer handles.
        /// </summary>
        public Type ValueType => typeof(T);

        public bool IsRoot => _isRoot;

        public SerializationContext Context { get; private set; }

        /// <inheritdoc/>
        SerializationContext ISerializationProcessor.Context
        {
            get => Context;
            set => Context = value;
        }

        public virtual bool CanProcess(Type valueType, SerializationContext context) => true;

        /// <summary>
        /// Processes a strongly-typed value during serialization or deserialization.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        protected abstract void Process(ref T value, IDataFormatter formatter);

        /// <summary>
        /// Processes a strongly-typed value with a member name during serialization or deserialization.
        /// </summary>
        /// <param name="name">The member name being processed.</param>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        protected virtual void Process(string name, ref T value, IDataFormatter formatter)
        {
            if (!IsRoot)
            {
                formatter.BeginMember(name);
            }
            Process(ref value, formatter);
        }

        protected virtual void Initialize()
        {
        }

        private void EnsureInitialize()
        {
            if (!_isInitialized)
            {
                Initialize();
                _isInitialized = true;
            }
        }

        bool ISerializationProcessor.IsRoot
        {
            get => _isRoot;
            set => _isRoot = value;
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

        void ISerializationProcessor.ProcessUntyped(ref object value, IDataFormatter formatter)
        {
            EnsureInitialize();
            T castedValue = default;
            if (value != null)
            {
                castedValue = (T)value;
            }
            Process(ref castedValue, formatter);
            value = castedValue;
        }

        void ISerializationProcessor.ProcessUntyped(string name, ref object value, IDataFormatter formatter)
        {
            EnsureInitialize();
            T castedValue = default;
            if (value != null)
            {
                castedValue = (T)value;
            }
            Process(name, ref castedValue, formatter);
            value = castedValue;
        }
    }
}
