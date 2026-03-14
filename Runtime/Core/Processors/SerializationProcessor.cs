using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
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
                    catch
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

        public SerializationContext Context { get; private set; }

        /// <inheritdoc/>
        SerializationContext ISerializationProcessor.Context
        {
            get => Context;
            set => Context = value;
        }

        /// <summary>
        /// Gets whether null values should be automatically constructed during deserialization.
        /// </summary>
        /// <remarks>
        /// When enabled and a null value is encountered during deserialization, the processor
        /// attempts to create a new instance using an accessible parameterless constructor.
        /// This behavior only applies during read operations.
        /// </remarks>
        /// <value>
        /// <c>true</c> to automatically construct null values; <c>false</c> to keep null values as-is.
        /// Default is <c>false</c>.
        /// </value>
        protected virtual bool AutoConstruct => false;

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
            value = ConstructIfNecessary(value, formatter.Operation);
            Process(ref value, formatter);
        }

        void ISerializationProcessor<T>.Process(string name, ref T value, IDataFormatter formatter)
        {
            EnsureInitialize();
            value = ConstructIfNecessary(value, formatter.Operation);
            Process(name, ref value, formatter);
        }

        void ISerializationProcessor.ProcessUntyped(ref object value, IDataFormatter formatter)
        {
            EnsureInitialize();
            value = ConstructIfNecessary(value, formatter.Operation);

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
            value = ConstructIfNecessary(value, formatter.Operation);

            T castedValue = default;
            if (value != null)
            {
                castedValue = (T)value;
            }
            Process(name, ref castedValue, formatter);
            value = castedValue;
        }


        private T ConstructIfNecessary(object value, FormatterOperation operation)
        {
            return ConstructIfNecessary(value == null ? default : (T)value, operation);
        }

        private T ConstructIfNecessary(T value, FormatterOperation operation)
        {
            if (operation == FormatterOperation.Read)
            {
                if (value == null)
                {
                    // Check if auto-construction is enabled
                    if (!AutoConstruct)
                    {
                        return default;
                    }

                    if (ConstructorInvoker != null)
                    {
                        return ConstructorInvoker();
                    }

                    if (typeof(T).IsStringType())
                    {
                        return (T)(object)string.Empty;
                    }

                    throw new SerializationException(
                        $"Cannot construct instance of type '{typeof(T)}' during deserialization. " +
                        $"The type does not have an accessible parameterless constructor. " +
                        $"Ensure the type has a public parameterless constructor or mark fields with [SerializeField].");
                }
            }

            return value;
        }
    }
}
