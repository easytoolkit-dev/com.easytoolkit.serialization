namespace EasyToolKit.Serialization.Formatters
{
    /// <summary>
    /// Serves as the abstract base class for formatter-specific configuration settings.
    /// </summary>
    public abstract class DataFormatterSettings
    {
        /// <summary>
        /// Gets or sets the format string used for generating anonymous member names.
        /// The format must contain a single placeholder "{0}" that will be replaced with the member ID.
        /// Default value is "${0}" which generates names like "$0", "$1", "$2", etc.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown when setting a format that does not contain "{0}".</exception>
        public string AnonymousMemberNameFormat
        {
            get => _anonymousMemberNameFormat;
            set
            {
                if (string.IsNullOrEmpty(value) || !value.Contains("{0}"))
                {
                    throw new System.InvalidOperationException(
                        "AnonymousMemberNameFormat must contain a \"{0}\" placeholder for the member ID.");
                }
                _anonymousMemberNameFormat = value;
            }
        }

        private string _anonymousMemberNameFormat = "${0}";

        /// <summary>
        /// Gets or sets a value indicating whether to return a default value instead of throwing
        /// an exception when the reading formatter reaches the end of the stream.
        /// </summary>
        public bool ReturnDefaultOnStreamEnd { get; set; } = true;
    }
}
