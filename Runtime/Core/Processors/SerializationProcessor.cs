using System;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Utilities;

namespace EasyToolkit.Serialization.Processors
{
    public abstract class SerializationProcessor<T> : ISerializationProcessor<T>
    {
        private bool _isInitialized;
        private bool _allowAnonymousTypes;
        private bool _allowNonSerializableTypes;
        private bool _allowUnmarkedStructs;
        private bool _excludeNonSerializedMembers;
        private bool _requireSerializeFieldOnNonPublic;
        private SerializableMemberFlags _memberFlags;

        /// <summary>
        /// Gets a value indicating whether anonymous types are allowed for the current processor scope.
        /// </summary>
        protected bool AllowAnonymousTypes => _allowAnonymousTypes;

        /// <summary>
        /// Gets a value indicating whether unmarked reference types are allowed for the current processor scope.
        /// </summary>
        protected bool AllowNonSerializableTypes => _allowNonSerializableTypes;

        /// <summary>
        /// Gets a value indicating whether unmarked struct types are allowed for the current processor scope.
        /// </summary>
        protected bool AllowUnmarkedStructs => _allowUnmarkedStructs;

        /// <summary>
        /// Gets a value indicating whether members marked with <see cref="NonSerializedAttribute"/> are excluded.
        /// </summary>
        protected bool ExcludeNonSerializedMembers => _excludeNonSerializedMembers;

        /// <summary>
        /// Gets a value indicating whether non-public fields require <c>SerializeField</c> in the current processor scope.
        /// </summary>
        protected bool RequireSerializeFieldOnNonPublic => _requireSerializeFieldOnNonPublic;

        /// <summary>
        /// Gets the member filtering flags for the current processor scope.
        /// </summary>
        protected SerializableMemberFlags MemberFlags => _memberFlags;

        /// <inheritdoc/>
        public Type ValueType => typeof(T);

        /// <inheritdoc/>
        public ISerializationProcessor Parent { get; set; }

        /// <inheritdoc/>
        public SerializationContext Context { get; set; }

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
                EasySerializableAttribute parentAttribute = null;
                if (Parent != null)
                {
                    parentAttribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(Parent.ValueType);
                }

                _memberFlags = parentAttribute is { IsDefinedMemberFlags: true }
                    ? parentAttribute.MemberFlags
                    : Context.MemberFlags;

                _requireSerializeFieldOnNonPublic = parentAttribute is { IsDefinedRequireSerializeFieldOnNonPublic: true }
                    ? parentAttribute.RequireSerializeFieldOnNonPublic
                    : Context.RequireSerializeFieldOnNonPublic;

                _excludeNonSerializedMembers = parentAttribute is { IsDefinedExcludeNonSerializedMembers: true }
                    ? parentAttribute.ExcludeNonSerializedMembers
                    : Context.ExcludeNonSerializedMembers;

                _allowAnonymousTypes = parentAttribute is { IsDefinedAllowAnonymousTypes: true }
                    ? parentAttribute.AllowAnonymousTypes
                    : Context.AllowAnonymousTypes;

                _allowNonSerializableTypes = parentAttribute is { IsDefinedAllowNonSerializableTypes: true }
                    ? parentAttribute.AllowNonSerializableTypes
                    : Context.AllowNonSerializableTypes;

                _allowUnmarkedStructs = parentAttribute is { IsDefinedAllowUnmarkedStructs: true }
                    ? parentAttribute.AllowUnmarkedStructs
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
