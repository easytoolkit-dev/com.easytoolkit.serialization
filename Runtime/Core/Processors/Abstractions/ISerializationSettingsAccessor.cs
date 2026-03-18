namespace EasyToolkit.Serialization.Processors
{
    /// <summary>
    /// Provides read-only access to serialization settings during the serialization process.
    /// </summary>
    /// <remarks>
    /// This interface is implemented by serialization processors to access runtime configuration
    /// settings from the <see cref="SerializationContext"/> or <see cref="EasySerializableAttribute"/>.
    /// Processors can check these settings to adjust their behavior during serialization,
    /// such as whether to use runtime types for polymorphic serialization.
    /// </remarks>
    public interface ISerializationSettingsAccessor
    {
        /// <summary>
        /// Gets the flags that control which members are filtered for serialization.
        /// </summary>
        /// <remarks>
        /// This setting controls which types of members (fields/properties, public/non-public)
        /// are included in serialization by default. The value is determined by
        /// <see cref="EasySerializableAttribute"/> if defined, otherwise falls back to
        /// <see cref="SerializationContext.MemberFlags"/>.
        /// </remarks>
        SerializableMemberFlags MemberFlags { get; }

        /// <summary>
        /// Gets whether non-public fields must have the <c>SerializeField</c> attribute to be serialized.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, non-public fields are only serialized if they are explicitly marked
        /// with <c>UnityEngine.SerializeField</c>. This mimics Unity's serialization behavior.
        /// The value is determined by <see cref="EasySerializableAttribute"/> if defined,
        /// otherwise falls back to <see cref="SerializationContext.RequireSerializeFieldOnNonPublic"/>.
        /// </remarks>
        bool RequireSerializeFieldOnNonPublic { get; }

        /// <summary>
        /// Gets whether to exclude members marked with <c>NonSerializedAttribute</c> from serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, fields marked with <c>System.NonSerializedAttribute</c> are excluded
        /// from serialization regardless of other settings. The value is determined by
        /// <see cref="EasySerializableAttribute"/> if defined, otherwise falls back to
        /// <see cref="SerializationContext.ExcludeNonSerialized"/>.
        /// </remarks>
        bool ExcludeNonSerialized { get; }

        /// <summary>
        /// Gets whether to allow anonymous types to be serialized.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, anonymous types (compiler-generated types with names containing
        /// "&lt;&gt;f__AnonymousType") can be serialized. The value is determined by
        /// <see cref="EasySerializableAttribute"/> if defined, otherwise falls back to
        /// <see cref="SerializationContext.AllowAnonymousTypes"/>.
        /// </remarks>
        bool AllowAnonymousTypes { get; }

        /// <summary>
        /// Gets whether to allow struct types without <see cref="SerializableAttribute"/>
        /// or <see cref="EasySerializableAttribute"/> to be serialized.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, struct types can be serialized even without explicit serialization
        /// attributes. The value is determined by <see cref="EasySerializableAttribute"/> if defined,
        /// otherwise falls back to <see cref="SerializationContext.AllowUnmarkedStructs"/>.
        /// </remarks>
        bool AllowUnmarkedStructs { get; }

        /// <summary>
        /// Gets whether to use runtime type instead of declared type for reference type serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, reference type members are serialized using their runtime type rather than
        /// their declared type. This enables polymorphic serialization where derived types are serialized
        /// with their specific type information. When <c>false</c>, members are serialized based on their
        /// declared type for better performance.
        /// <para>This setting only affects reference types (classes). Value types (structs) are always
        /// serialized based on their declared type since they cannot participate in inheritance.</para>
        /// <para>The value is determined by <see cref="EasySerializableAttribute"/> if defined,
        /// otherwise falls back to <see cref="SerializationContext.UseRuntimeTypeSerialization"/>.</para>
        /// </remarks>
        bool UseRuntimeTypeSerialization { get; }
    }
}
