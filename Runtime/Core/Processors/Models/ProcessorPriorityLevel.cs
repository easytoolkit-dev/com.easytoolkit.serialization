namespace EasyToolkit.Serialization.Processors
{
    public static class ProcessorPriorityLevel
    {
        /// <summary>
        /// Generic serializer with lowest priority (fallback option).
        /// </summary>
        public const double Generic = -5000.0;

        /// <summary>
        /// System types serializer.
        /// </summary>
        public const double System = -4000.0;

        /// <summary>
        /// Collection types serializer.
        /// </summary>
        public const double Collection = -3000.0;

        /// <summary>
        /// Unity types serializer.
        /// </summary>
        public const double Unity = -2000.0;

        /// <summary>
        /// Primitive types serializer.
        /// </summary>
        public const double Primitive = -1000.0;

        /// <summary>
        /// Custom serializer with default priority.
        /// </summary>
        public const double Custom = 0.0;
    }
}
