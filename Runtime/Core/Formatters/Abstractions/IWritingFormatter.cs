namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Writing/serialization formatter interface.
    /// Combines core formatter with object reference writing capabilities.
    /// </summary>
    public interface IWritingFormatter : IDataFormatter, IObjectReferenceWriter
    {
        /// <summary>Always returns Output direction for writing formatters.</summary>
        new FormatterDirection Direction { get; }
    }
}
