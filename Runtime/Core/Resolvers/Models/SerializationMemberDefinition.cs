using System;
using System.Reflection;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Processors;
using JetBrains.Annotations;

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

        [CanBeNull] public InstanceGetter ValueGetter { get; set; }
        [CanBeNull] public InstanceSetter ValueSetter { get; set; }

        [CanBeNull] public ISerializationProcessor Processor { get; set; }

        /// <summary>
        /// Gets or sets whether to use runtime type for processor lookup during serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the processor will use the runtime type of the value instead of the
        /// declared member type to find the appropriate serialization processor. This enables
        /// polymorphic serialization for reference types. Only applicable to reference types.
        /// </remarks>
        public bool UseRuntimeType { get; set; }

        /// <summary>
        /// Gets or sets whether to allow unmarked struct types during serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, struct types that are not marked with <see cref="SerializableAttribute"/>
        /// or <see cref="EasySerializableAttribute"/> will still be serialized. This is useful for
        /// third-party structs or simple value types that cannot be modified. Only applicable to struct types.
        /// </remarks>
        public bool AllowUnmarkedStructs { get; set; }

        /// <summary>
        /// Gets or sets whether to allow anonymous types during serialization.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, anonymous types will be serialized. Anonymous types are compiler-generated
        /// types that cannot be marked with serialization attributes. This enables serialization of
        /// LINQ query results and other anonymous type instances.
        /// </remarks>
        public bool AllowAnonymousTypes { get; set; }
    }
}
