using System;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Serialization.Formatters;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Processors
{
    public abstract class SerializationProcessor<T> : ISerializationProcessor<T>
    {
        private static readonly bool IsClassType = typeof(T).IsClass && typeof(T) != typeof(string);
        private static readonly bool IsInstantiableType = typeof(T).IsInstantiable(allowLenient: true);
        [CanBeNull] private static readonly ParameterlessConstructorInvoker<T> ConstructorInvoker;

        static SerializationProcessor()
        {
            if (IsClassType && IsInstantiableType)
            {
                foreach (var constructorInfo in typeof(T).GetConstructors(MemberAccessFlags.AllInstance))
                {
                    try
                    {
                        ConstructorInvoker = ReflectionCompiler.CreateParameterlessConstructorInvoker<T>(
                            constructorInfo, autoFillParameters: true
                        );
                        break;
                    }
                    catch (Exception e)
                    {
                        // ignored
                    }
                }
            }
        }

        private bool _isInitialized;
        private bool _isRoot;

        /// <summary>
        /// Gets the value type this serializer handles.
        /// </summary>
        public Type ValueType => typeof(T);

        public bool IsRoot => _isRoot;

        /// <summary>
        /// Determines whether the specified value type can be serialized.
        /// Default implementation uses exact type matching.
        /// </summary>
        /// <param name="valueType">The type to validate for serialization support.</param>
        /// <returns>True if the type can be serialized; otherwise, false.</returns>
        public virtual bool CanProcess(Type valueType) => true;

        /// <summary>
        /// Processes a strongly-typed value during serialization or deserialization.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="formatter">The data formatter to use for processing.</param>
        public virtual void Process(ref T value, IDataFormatter formatter)
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

        void ISerializationProcessor.ProcessUntyped(ref object value, IDataFormatter formatter)
        {
            EnsureInitialize();
            if (value == null && ConstructorInvoker != null && formatter.Operation == FormatterOperation.Read)
            {
                value = ConstructorInvoker();
            }

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
            if (value == null && ConstructorInvoker != null && formatter.Operation == FormatterOperation.Read)
            {
                value = ConstructorInvoker();
            }

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
