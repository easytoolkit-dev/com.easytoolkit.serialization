using System;
using System.Reflection;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Processors;

namespace EasyToolkit.Serialization.Resolvers
{
    /// <summary>
    /// Defines the metadata for a serializable member.
    /// Contains static information about the member without access delegates.
    /// </summary>
    public sealed class SerializationMemberDefinition
    {
        /// <summary>
        /// Gets or sets the member name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the member type.
        /// </summary>
        public Type MemberType { get; set; }

        /// <summary>
        /// Gets or sets the member information (FieldInfo or PropertyInfo).
        /// </summary>
        public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// Gets or sets whether the value is required during deserialization.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the default value for the member.
        /// </summary>
        public object DefaultValue { get; set; }

        public InstanceGetter ValueGetter { get; set; }
        public InstanceSetter ValueSetter { get; set; }

        public ISerializationProcessor Processor { get; set; }
    }
}
