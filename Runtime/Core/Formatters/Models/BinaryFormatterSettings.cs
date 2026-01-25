using System;

namespace EasyToolKit.Serialization.Formatters
{
    /// <summary>
    /// Provides configuration settings for binary formatter operations.
    /// </summary>
    public class BinaryFormatterSettings : DataFormatterSettings
    {
        /// <summary>
        /// Gets or sets the formatter options that control which features are enabled.
        /// </summary>
        public BinaryFormatterOptions Options { get; set; } = BinaryFormatterOptions.Default;
    }
}
