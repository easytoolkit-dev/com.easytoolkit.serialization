using System;

namespace EasyToolkit.Serialization
{
    /// <summary>
    /// Defines flags for filtering which members should be serialized.
    /// </summary>
    [Flags]
    public enum SerializableMemberFlags
    {
        /// <summary>
        /// No members are serialized.
        /// </summary>
        None = 0,

        /// <summary>
        /// Include public members.
        /// </summary>
        Public = 1 << 0,

        /// <summary>
        /// Include non-public (private, protected, internal) members.
        /// </summary>
        NonPublic = 1 << 1,

        /// <summary>
        /// Include fields.
        /// </summary>
        Field = 1 << 2,

        /// <summary>
        /// Include properties.
        /// </summary>
        Property = 1 << 3,

        /// <summary>
        /// Include public fields.
        /// </summary>
        PublicFields = Public | Field,

        /// <summary>
        /// Include public properties.
        /// </summary>
        PublicProperties = Public | Property,

        /// <summary>
        /// Include non-public fields.
        /// </summary>
        NonPublicFields = NonPublic | Field,

        /// <summary>
        /// Include non-public properties.
        /// </summary>
        NonPublicProperties = NonPublic | Property,

        /// <summary>
        /// Include all public members (fields and properties).
        /// </summary>
        AllPublic = Public | Field | Property,

        /// <summary>
        /// Include all non-public members (fields and properties).
        /// </summary>
        AllNonPublic = NonPublic | Field | Property,

        /// <summary>
        /// Include all fields (public and non-public).
        /// </summary>
        AllFields = Public | NonPublic | Field,

        /// <summary>
        /// Include all properties (public and non-public).
        /// </summary>
        AllProperties = Public | NonPublic | Property,

        /// <summary>
        /// Include all members (all flags enabled).
        /// </summary>
        All = Public | NonPublic | Field | Property,

        /// <summary>
        /// Default serialization filter.
        /// </summary>
        Default = AllFields,
    }
}
