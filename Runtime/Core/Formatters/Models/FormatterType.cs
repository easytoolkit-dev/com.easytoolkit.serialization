namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the supported formatter format types.
    /// </summary>
    public enum FormatterType
    {
        /// <summary>Binary format with varint encoding.</summary>
        Binary,

        /// <summary>JSON text format.</summary>
        Json,

        /// <summary>XML text format.</summary>
        Xml,

        /// <summary>YAML text format.</summary>
        Yaml
    }
}
