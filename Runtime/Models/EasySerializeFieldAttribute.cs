using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Provides configuration for individual field or property serialization behavior.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to fields or properties to customize their serialization behavior,
    /// such as using a different name in serialized data or excluding specific members from serialization.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class EasySerializeFieldAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the custom name to use for serialization.
        /// </summary>
        /// <remarks>
        /// When specified, this name will be used in the serialized data instead of the member's
        /// actual name. This is useful for mapping to different naming conventions (e.g., converting
        /// from camelCase to snake_case) or for backward compatibility when renaming members.
        /// If <c>null</c> or empty, the member's actual name is used.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether to ignore this member during serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, this member is excluded from serialization regardless of other settings.
        /// This provides fine-grained control over which members are serialized, complementing the
        /// class-level <c>EasySerializableAttribute</c> configuration.
        /// </remarks>
        public bool Ignore { get; set; }
    }
}
