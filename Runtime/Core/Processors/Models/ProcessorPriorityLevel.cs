namespace EasyToolKit.Serialization.Processors
{
    public enum ProcessorPriorityLevel
    {
        /// <summary>
        /// Generic serializer with lowest priority (fallback option).
        /// </summary>
        Generic = -5000,

        /// <summary>
        /// System types serializer.
        /// </summary>
        System = -4000,

        /// <summary>
        /// Collection types serializer.
        /// </summary>
        Collection = -3000,

        /// <summary>
        /// Unity types serializer.
        /// </summary>
        Unity = -2000,

        /// <summary>
        /// Primitive types serializer.
        /// </summary>
        Primitive = -1000,

        /// <summary>
        /// Custom serializer with default priority.
        /// </summary>
        Custom = 0,
    }
}
