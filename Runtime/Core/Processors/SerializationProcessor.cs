using System;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Utilities;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Processors
{
    public abstract class SerializationProcessor<T> : ISerializationProcessor<T>
    {
        private bool _isInitialized;
        [CanBeNull] private EasySerializableAttribute _attribute;
        private bool _useRuntimeTypeSerialization;
        private SerializableMemberFlags _memberFlags;
        private bool _requireSerializeFieldOnNonPublic;
        private bool _excludeNonSerialized;
        private bool _allowAnonymousTypes;
        private bool _allowUnmarkedStructs;

        /// <inheritdoc/>
        public Type ValueType => typeof(T);

        /// <inheritdoc/>
        public ISerializationProcessor Parent { get; set; }

        /// <inheritdoc/>
        public SerializationContext Context { get; set; }

        /// <inheritdoc/>
        public bool UseRuntimeTypeSerialization => _useRuntimeTypeSerialization;

        /// <inheritdoc/>
        public SerializableMemberFlags MemberFlags => _memberFlags;

        /// <inheritdoc/>
        public bool RequireSerializeFieldOnNonPublic => _requireSerializeFieldOnNonPublic;

        /// <inheritdoc/>
        public bool ExcludeNonSerialized => _excludeNonSerialized;

        /// <inheritdoc/>
        public bool AllowAnonymousTypes => _allowAnonymousTypes;

        /// <inheritdoc/>
        public bool AllowUnmarkedStructs => _allowUnmarkedStructs;

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
            if (Parent != null)
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
                _attribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(typeof(T));
                _useRuntimeTypeSerialization = _attribute is { IsDefinedUseRuntimeTypeSerialization: true }
                    ? _attribute.UseRuntimeTypeSerialization
                    : Context.UseRuntimeTypeSerialization;
                _memberFlags = _attribute is { IsDefinedMemberFlags: true }
                    ? _attribute.MemberFlags
                    : Context.MemberFlags;
                _requireSerializeFieldOnNonPublic = _attribute is { IsDefinedRequireSerializeFieldOnNonPublic: true }
                    ? _attribute.RequireSerializeFieldOnNonPublic
                    : Context.RequireSerializeFieldOnNonPublic;
                _excludeNonSerialized = _attribute is { IsDefinedExcludeNonSerialized: true }
                    ? _attribute.ExcludeNonSerialized
                    : Context.ExcludeNonSerialized;
                _allowAnonymousTypes = _attribute is { IsDefinedAllowAnonymousTypes: true }
                    ? _attribute.AllowAnonymousTypes
                    : Context.AllowAnonymousTypes;
                _allowUnmarkedStructs = _attribute is { IsDefinedAllowUnmarkedStructs: true }
                    ? _attribute.AllowUnmarkedStructs
                    : Context.AllowUnmarkedStructs;

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
