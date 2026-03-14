using System;

namespace EasyToolkit.Serialization
{
    /// <summary>
    /// Marks a type as serializable by the EasyToolkit serialization system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class EasySerializableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets whether derived classes inherit serializability when this attribute
        /// is applied only to a base class.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, derived classes are automatically serializable without requiring
        /// the attribute to be applied directly. When <c>false</c>, only the class with this
        /// attribute applied is serializable.
        /// </remarks>
        public bool AllocInherit { get; set; }

        private SerializableMemberFlags? _memberFlags;

        /// <summary>
        /// Gets or sets the flags that control which members are filtered for serialization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedMemberFlags"/> before accessing.
        /// </exception>
        /// <remarks>
        /// When this property is explicitly set, it takes precedence over the
        /// <see cref="SerializationContext.MemberFlags"/> setting. When not set,
        /// the context's value is used instead.
        /// </remarks>
        public SerializableMemberFlags MemberFlags
        {
            get => _memberFlags ?? throw new InvalidOperationException(
                "Cannot access MemberFlags property because it has not been set. " +
                "Check IsDefinedMemberFlags before accessing MemberFlags, or set a value first.");
            set => _memberFlags = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MemberFlags"/> property has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="MemberFlags"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.MemberFlags"/> value is used instead.
        /// </remarks>
        public bool IsDefinedMemberFlags => _memberFlags.HasValue;

        private bool? _requireSerializeFieldOnNonPublic;

        /// <summary>
        /// Gets or sets whether non-public fields must have the <c>SerializeField</c> attribute
        /// to be serialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedRequireSerializeFieldOnNonPublic"/> before accessing.
        /// </exception>
        /// <remarks>
        /// When <c>true</c>, non-public fields are only serialized if they are explicitly marked
        /// with <c>UnityEngine.SerializeField</c>. This mimics Unity's serialization behavior.
        /// When <c>false</c>, non-public fields are serialized based on <c>MemberFlags</c> alone.
        /// When this property is explicitly set, it takes precedence over the
        /// <see cref="SerializationContext.RequireSerializeFieldOnNonPublic"/> setting.
        /// </remarks>
        public bool RequireSerializeFieldOnNonPublic
        {
            get => _requireSerializeFieldOnNonPublic ?? throw new InvalidOperationException(
                "Cannot access RequireSerializeFieldOnNonPublic property because it has not been set.");
            set => _requireSerializeFieldOnNonPublic = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RequireSerializeFieldOnNonPublic"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="RequireSerializeFieldOnNonPublic"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.RequireSerializeFieldOnNonPublic"/> value is used instead.
        /// </remarks>
        public bool IsDefinedRequireSerializeFieldOnNonPublic => _requireSerializeFieldOnNonPublic.HasValue;

        private bool? _excludeNonSerialized;

        /// <summary>
        /// Gets or sets whether to exclude members marked with <c>NonSerializedAttribute</c>
        /// from serialization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedExcludeNonSerialized"/> before accessing.
        /// </exception>
        /// <remarks>
        /// When <c>true</c>, fields marked with <c>System.NonSerializedAttribute</c> are excluded
        /// from serialization regardless of other settings. When <c>false</c>, the <c>NonSerialized</c>
        /// attribute is ignored and members are serialized based on other flags.
        /// When this property is explicitly set, it takes precedence over the
        /// <see cref="SerializationContext.ExcludeNonSerialized"/> setting.
        /// </remarks>
        public bool ExcludeNonSerialized
        {
            get => _excludeNonSerialized ?? throw new InvalidOperationException(
                "Cannot access ExcludeNonSerialized property because it has not been set.");
            set => _excludeNonSerialized = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ExcludeNonSerialized"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="ExcludeNonSerialized"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.ExcludeNonSerialized"/> value is used instead.
        /// </remarks>
        public bool IsDefinedExcludeNonSerialized => _excludeNonSerialized.HasValue;
    }
}
