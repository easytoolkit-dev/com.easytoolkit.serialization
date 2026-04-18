using System;

namespace EasyToolkit.Serialization
{
    /// <summary>
    /// Marks a type as serializable by the EasyToolkit serialization system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class EasySerializableAttribute : Attribute
    {
        private bool? _excludeNonSerializedMembers;
        private bool? _allowAnonymousTypes;
        private bool? _allowNonSerializableTypes;
        private bool? _allowUnmarkedStructs;
        private bool? _requireSerializeFieldOnNonPublic;
        private SerializableMemberFlags? _memberFlags;

        /// <summary>
        /// Gets or sets whether derived classes inherit serializability when this attribute
        /// is applied only to a base class.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, derived classes are automatically serializable without requiring
        /// the attribute to be applied directly.
        /// When <c>false</c>, only the class with this attribute applied is serializable.
        /// </remarks>
        public bool AllocInherit { get; set; }

        /// <summary>
        /// Gets or sets whether this type should be ignored during serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, this type is excluded from serialization processing. Any fields
        /// or properties of this type will be skipped during serialization structure resolution.
        /// This provides a way to prevent specific types from being serialized, useful for
        /// types that should not participate in the serialization system.
        /// </remarks>
        public bool Ignore { get; set; }

        /// <summary>
        /// Gets or sets the flags that control which members are filtered for serialization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedMemberFlags"/> before accessing.
        /// </exception>
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
        /// with <see cref="UnityEngine.SerializeField"/>. This mimics Unity's serialization behavior.
        /// When <c>false</c>, non-public fields are serialized based on <see cref="MemberFlags"/> alone.
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

        /// <summary>
        /// Gets or sets whether to exclude members marked with <see cref="NonSerializedAttribute"/>
        /// from serialization.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedExcludeNonSerializedMembers"/> before accessing.
        /// </exception>
        /// <remarks>
        /// When <c>true</c>, fields marked with <see cref="NonSerializedAttribute"/> are excluded
        /// from serialization regardless of other settings. When <c>false</c>, the <see cref="NonSerializedAttribute"/>
        /// is ignored and members are serialized based on other flags.
        /// </remarks>
        public bool ExcludeNonSerializedMembers
        {
            get => _excludeNonSerializedMembers ?? throw new InvalidOperationException(
                "Cannot access ExcludeNonSerialized property because it has not been set.");
            set => _excludeNonSerializedMembers = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ExcludeNonSerializedMembers"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="ExcludeNonSerializedMembers"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.ExcludeNonSerializedMembers"/> value is used instead.
        /// </remarks>
        public bool IsDefinedExcludeNonSerializedMembers => _excludeNonSerializedMembers.HasValue;

        /// <summary>
        /// Gets or sets whether to allow anonymous types to be serialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedAllowAnonymousTypes"/> before accessing.
        /// </exception>
        /// <remarks>
        /// <para>
        /// When <c>true</c>, anonymous types (compiler-generated types with names containing
        /// "&lt;&gt;f__AnonymousType") can be serialized. When <c>false</c>, anonymous types
        /// are excluded from serialization.
        /// When this property is explicitly set, it takes precedence over the
        /// <see cref="SerializationContext.AllowAnonymousTypes"/> setting.
        /// </para>
        /// <para>
        /// <b>Important:</b>
        /// </para>
        /// <para>
        /// When enabling this setting, ensure that
        /// <see cref="MemberFlags"/> includes <see cref="SerializableMemberFlags.PublicProperties"/>.
        /// Anonymous types only expose read-only properties with no fields,
        /// so serialization will fail without property support.
        /// </para>
        /// </remarks>
        public bool AllowAnonymousTypes
        {
            get => _allowAnonymousTypes ?? throw new InvalidOperationException(
                "Cannot access AllowAnonymousTypes property because it has not been set.");
            set => _allowAnonymousTypes = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AllowAnonymousTypes"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="AllowAnonymousTypes"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.AllowAnonymousTypes"/> value is used instead.
        /// </remarks>
        public bool IsDefinedAllowAnonymousTypes => _allowAnonymousTypes.HasValue;

        /// <summary>
        /// Gets or sets whether to allow reference types without <see cref="SerializableAttribute"/>
        /// or <see cref="EasySerializableAttribute"/> to be serialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedAllowNonSerializableTypes"/> before accessing.
        /// </exception>
        /// <remarks>
        /// <para>
        /// When <c>true</c>, class and interface types can be serialized even when they do not
        /// declare any serialization attribute. When <c>false</c>, reference types must be marked with
        /// either <see cref="SerializableAttribute"/> or <see cref="EasySerializableAttribute"/>.
        /// </para>
        /// <para>
        /// When this property is explicitly set, it takes precedence over the
        /// <see cref="SerializationContext.AllowNonSerializableTypes"/> setting.
        /// </para>
        /// </remarks>
        public bool AllowNonSerializableTypes
        {
            get => _allowNonSerializableTypes ?? throw new InvalidOperationException(
                "Cannot access AllowNonSerializableTypes property because it has not been set.");
            set => _allowNonSerializableTypes = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AllowNonSerializableTypes"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="AllowNonSerializableTypes"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.AllowNonSerializableTypes"/> value is used instead.
        /// </remarks>
        public bool IsDefinedAllowNonSerializableTypes => _allowNonSerializableTypes.HasValue;

        /// <summary>
        /// Gets or sets whether to allow struct types without <see cref="SerializableAttribute"/>
        /// or <see cref="EasySerializableAttribute"/> to be serialized.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedAllowUnmarkedStructs"/> before accessing.
        /// </exception>
        /// <remarks>
        /// <para>
        /// When <c>true</c>, struct types can be serialized even without explicit serialization
        /// attributes. When <c>false</c>, structs must be marked with either
        /// <see cref="SerializableAttribute"/> or <see cref="EasySerializableAttribute"/> to be serialized.
        /// </para>
        /// <para>
        /// This setting only performs static member type checking based on declared types.
        /// Runtime member types are not checked for struct serialization compatibility to avoid performance overhead.
        /// </para>
        /// </remarks>
        public bool AllowUnmarkedStructs
        {
            get => _allowUnmarkedStructs ?? throw new InvalidOperationException(
                "Cannot access AllowUnmarkedStructs property because it has not been set.");
            set => _allowUnmarkedStructs = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AllowUnmarkedStructs"/> property
        /// has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the attribute's <see cref="AllowUnmarkedStructs"/> value is used.
        /// When <c>false</c>, the <see cref="SerializationContext.AllowUnmarkedStructs"/> value is used instead.
        /// </remarks>
        public bool IsDefinedAllowUnmarkedStructs => _allowUnmarkedStructs.HasValue;
    }
}
