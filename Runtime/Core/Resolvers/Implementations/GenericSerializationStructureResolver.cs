using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Processors;
using EasyToolkit.Serialization.Utilities;

namespace EasyToolkit.Serialization.Resolvers.Implementations
{
    /// <summary>
    /// Resolves serialization structure for types marked with [Serializable] attribute.
    /// </summary>
    [SerializationResolverPriority(10.0)]
    public sealed class GenericSerializationStructureResolver : ISerializationStructureResolver
    {
        public bool CanResolve(Type valueType)
        {
            return !valueType.IsBasicValueType() &&
                   !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        public SerializationMemberDefinition[] Resolve(Type valueType)
        {
            var attribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType);
            var memberFlags = attribute?.MemberFlags ?? SerializableMemberFlags.Default;
            var requireSerializeFieldOnNonPublic = attribute?.RequireSerializeFieldOnNonPublic ?? false;
            var excludeNonSerialized = attribute?.ExcludeNonSerialized ?? true;

            var members = new List<SerializationMemberDefinition>();

            var memberInfos = valueType.GetMembers(MemberAccessFlags.AllInstance)
                .Where(memberInfo => memberInfo is FieldInfo || memberInfo is PropertyInfo)
                .Where(memberInfo => ShouldIncludeMember(memberInfo, memberFlags, requireSerializeFieldOnNonPublic, excludeNonSerialized))
                .ToList();

            for (int i = 0; i < memberInfos.Count; i++)
            {
                var memberInfo = memberInfos[i];
                Type memberType = GetMemberType(memberInfo);

                // Get custom serialization name from EasySerializeFieldAttribute
                var serializeFieldAttribute = memberInfo.GetCustomAttributes(typeof(EasySerializeFieldAttribute), inherit: true)
                    .FirstOrDefault() as EasySerializeFieldAttribute;
                string serializedName = serializeFieldAttribute?.Name;
                if (string.IsNullOrEmpty(serializedName))
                {
                    serializedName = memberInfo.Name;
                }

                var memberDefinition = new SerializationMemberDefinition
                {
                    Name = serializedName,
                    MemberType = memberType,
                    MemberInfo = memberInfo,
                    IsRequired = false,
                    DefaultValue = null,
                    ValueGetter = CreateValueGetter(memberInfo),
                    ValueSetter = CreateValueSetter(memberInfo),
                    Processor = SerializationProcessorFactory.GetProcessor(memberType)
                };

                members.Add(memberDefinition);
            }

            return members.ToArray();
        }

        /// <summary>
        /// Determines whether a member should be included based on the specified flags.
        /// </summary>
        private static bool ShouldIncludeMember(MemberInfo memberInfo, SerializableMemberFlags flags,
            bool requireSerializeFieldOnNonPublic, bool excludeNonSerialized)
        {
            // Check for EasySerializeFieldAttribute with Ignore flag
            var serializeFieldAttribute = memberInfo.GetCustomAttribute<EasySerializeFieldAttribute>(inherit: true);
            if (serializeFieldAttribute != null && serializeFieldAttribute.Ignore)
            {
                return false;
            }

            // Check member type (Field vs Property)
            bool isField = memberInfo is FieldInfo;
            bool isProperty = memberInfo is PropertyInfo;

            // Check for NonSerializedAttribute on fields
            if (excludeNonSerialized && isField)
            {
                var nonSerializedAttributes = memberInfo.GetCustomAttributes(typeof(NonSerializedAttribute), inherit: true);
                if (nonSerializedAttributes.Length > 0)
                {
                    return false;
                }
            }

            if (isField && !flags.HasFlag(SerializableMemberFlags.Field))
            {
                return false;
            }

            if (isProperty && !flags.HasFlag(SerializableMemberFlags.Property))
            {
                return false;
            }

            // Check visibility (Public vs NonPublic)
            bool isPublic = IsPublicMember(memberInfo);
            bool includePublic = flags.HasFlag(SerializableMemberFlags.Public);
            bool includeNonPublic = flags.HasFlag(SerializableMemberFlags.NonPublic);

            if (isPublic && !includePublic)
            {
                return false;
            }

            if (!isPublic && !includeNonPublic)
            {
                return false;
            }

            // Check if non-public field requires SerializeField attribute
            if (!isPublic && isField && requireSerializeFieldOnNonPublic)
            {
                var fieldInfo = (FieldInfo)memberInfo;
                var serializeFieldAttributes =
                    fieldInfo.GetCustomAttributes(typeof(UnityEngine.SerializeField), inherit: true);
                if (serializeFieldAttributes.Length == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether a member is public.
        /// </summary>
        private static bool IsPublicMember(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)memberInfo).IsPublic,
                MemberTypes.Property => IsPropertyPublic((PropertyInfo)memberInfo),
                _ => false
            };
        }

        /// <summary>
        /// Determines whether a property is public (has at least public getter or setter).
        /// </summary>
        private static bool IsPropertyPublic(PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod(nonPublic: true);
            var setMethod = propertyInfo.GetSetMethod(nonPublic: true);

            return (getMethod != null && getMethod.IsPublic) || (setMethod != null && setMethod.IsPublic);
        }

        private static Type GetMemberType(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)memberInfo).FieldType,
                MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType,
                _ => throw new ArgumentException($"Unsupported member type: {memberInfo.MemberType}")
            };
        }

        private InstanceGetter CreateValueGetter(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ReflectionCompiler.CreateInstanceFieldGetter((FieldInfo)memberInfo),
                MemberTypes.Property => ReflectionCompiler.CreateInstancePropertyGetter((PropertyInfo)memberInfo),
                _ => throw new ArgumentException($"Unsupported member type: {memberInfo.MemberType}")
            };
        }

        private InstanceSetter CreateValueSetter(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ReflectionCompiler.CreateInstanceFieldSetter((FieldInfo)memberInfo),
                MemberTypes.Property => ReflectionCompiler.CreateInstancePropertySetter((PropertyInfo)memberInfo),
                _ => throw new ArgumentException($"Unsupported member type: {memberInfo.MemberType}")
            };
        }
    }
}
