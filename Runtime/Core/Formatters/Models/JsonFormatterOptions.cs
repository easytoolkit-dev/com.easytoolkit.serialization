using System;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Defines formatting options for JSON serialization format.
    /// These flags control which formatter features are enabled and can be combined.
    /// </summary>
    [Flags]
    public enum JsonFormatterOptions
    {
        /// <summary>
        /// No formatter options enabled. All features are disabled.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enables writing type information before object begin markers.
        /// When enabled, object type full name is serialized as a "__meta_type__" field
        /// for polymorphic deserialization.
        /// When disabled, objects are deserialized using the known type structure.
        /// </summary>
        IncludeObjectType = 1 << 0,

        /// <summary>
        /// Default configuration with object type enabled.
        /// Provides a good balance between compatibility and functionality.
        /// </summary>
        Default = IncludeObjectType
    }
}
