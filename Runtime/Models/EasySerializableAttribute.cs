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

        /// <summary>
        /// Gets or sets the flags that control which members are filtered for serialization.
        /// </summary>
        public SerializableMemberFlags MemberFlags { get; set; } = SerializableMemberFlags.Default;

        /// <summary>
        /// Gets or sets whether non-public fields must have the <c>SerializeField</c> attribute
        /// to be serialized.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, non-public fields are only serialized if they are explicitly marked
        /// with <c>UnityEngine.SerializeField</c>. This mimics Unity's serialization behavior.
        /// When <c>false</c>, non-public fields are serialized based on <c>MemberFlags</c> alone.
        /// </remarks>
        public bool RequireSerializeFieldOnNonPublic { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to exclude members marked with <c>NonSerializedAttribute</c>
        /// from serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, fields marked with <c>System.NonSerializedAttribute</c> are excluded
        /// from serialization regardless of other settings. When <c>false</c>, the <c>NonSerialized</c>
        /// attribute is ignored and members are serialized based on other flags.
        /// </remarks>
        public bool ExcludeNonSerialized { get; set; } = true;
    }
}
