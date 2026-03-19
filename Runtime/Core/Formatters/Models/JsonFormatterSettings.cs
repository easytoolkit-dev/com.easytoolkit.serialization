namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Provides configuration settings for JSON formatter operations.
    /// </summary>
    public class JsonFormatterSettings : DataFormatterSettings
    {
        /// <summary>
        /// Gets or sets the formatter options that control which features are enabled.
        /// Default value is <see cref="JsonFormatterOptions.Default"/>.
        /// </summary>
        public JsonFormatterOptions Options { get; set; } = JsonFormatterOptions.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically wrap atomic values
        /// in an array when no root object or array has been explicitly created.
        /// When enabled, atomic values (primitives, Guid, DateTime, etc.) will be
        /// automatically wrapped in a JSON array to produce valid JSON output.
        /// Default is false.
        /// </summary>
        /// <remarks>
        /// When this setting is enabled and an atomic value is formatted without
        /// calling BeginArray() or BeginObject() first, the formatter will automatically
        /// create a root array containing the atomic value. For example, formatting
        /// the integer 42 would produce "[42]" instead of throwing an exception.
        /// This applies to all atomic types including primitives, Guid, DateTime, etc.
        /// </remarks>
        public bool AutoWrapAtomicValueInArray { get; set; } = true;

        /// <summary>
        /// Gets or sets the field name used for storing type information in JSON objects.
        /// When <see cref="JsonFormatterOptions.IncludeObjectType"/> is enabled, this field
        /// name will be used as the key for storing the serialized type name.
        /// Default value is "__meta_type__".
        /// </summary>
        /// <remarks>
        /// This field name must be consistent between serialization and deserialization.
        /// Changing this value will affect both the writing and reading of type metadata.
        /// Choose a name that does not conflict with your actual data fields.
        /// </remarks>
        public string TypeNameField { get; set; } = "__meta_type__";
    }
}
