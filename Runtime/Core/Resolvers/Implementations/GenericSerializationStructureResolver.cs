using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Processors;
using EasyToolkit.Serialization.Utilities;
using UnityEngine;

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

        /// <inheritdoc/>
        public SerializationMemberDefinition[] Resolve(Type valueType, SerializationContext context, ISerializationProcessor parent)
        {
            var easySerializableAttribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType);

            // Priority: Attribute explicit setting > Context
            var serializableMemberFlags = easySerializableAttribute is { IsDefinedMemberFlags: true }
                ? easySerializableAttribute.MemberFlags
                : context.MemberFlags;

            var requireSerializeField = easySerializableAttribute is { IsDefinedRequireSerializeFieldOnNonPublic: true }
                ? easySerializableAttribute.RequireSerializeFieldOnNonPublic
                : context.RequireSerializeFieldOnNonPublic;

            var excludeNonSerializedMembers = easySerializableAttribute is { IsDefinedExcludeNonSerializedMembers: true }
                ? easySerializableAttribute.ExcludeNonSerializedMembers
                : context.ExcludeNonSerializedMembers;

            var allowAnonymousTypes = easySerializableAttribute is { IsDefinedAllowAnonymousTypes: true }
                ? easySerializableAttribute.AllowAnonymousTypes
                : context.AllowAnonymousTypes;

            var allowNonSerializableTypes = easySerializableAttribute is { IsDefinedAllowNonSerializableTypes: true }
                ? easySerializableAttribute.AllowNonSerializableTypes
                : context.AllowNonSerializableTypes;

            var allowUnmarkedStructs = easySerializableAttribute is { IsDefinedAllowUnmarkedStructs: true }
                ? easySerializableAttribute.AllowUnmarkedStructs
                : context.AllowUnmarkedStructs;

            var members = new List<SerializationMemberDefinition>();

            var memberInfos = valueType.GetAllMembers(MemberAccessFlags.AllInstance)
                .Where(memberInfo => (memberInfo is FieldInfo fieldInfo && !fieldInfo.IsBackingField()) || memberInfo is PropertyInfo)
                .Where(memberInfo => ShouldIncludeMember(memberInfo, serializableMemberFlags, requireSerializeField, excludeNonSerializedMembers, allowAnonymousTypes, allowUnmarkedStructs))
                .ToList();

            for (int i = 0; i < memberInfos.Count; i++)
            {
                var memberInfo = memberInfos[i];
                var memberType = GetMemberType(memberInfo);

                // Get custom serialization name from EasySerializeFieldAttribute
                var serializeFieldAttribute = memberInfo.GetCustomAttributes(typeof(EasySerializeFieldAttribute), inherit: true)
                    .FirstOrDefault() as EasySerializeFieldAttribute;
                string serializedName = serializeFieldAttribute?.Name;
                if (string.IsNullOrEmpty(serializedName))
                {
                    serializedName = memberInfo.Name;
                }

                ISerializationProcessor processor = null;
                SerializationException serializationException = null;
                try
                {
                    processor = SerializationProcessorFactory.CreateProcessor(memberType, context, parent);
                }
                catch (SerializationException exception)
                {
                    serializationException = exception;
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
                    Processor = processor,
                    SerializationException = serializationException,
                    UseRuntimeType = !memberType.IsValueType && !memberType.IsSealed && memberType != typeof(string),
                    AllowNonSerializableTypes = allowNonSerializableTypes,
                    AllowAnonymousTypes = allowAnonymousTypes,
                    AllowUnmarkedStructs = allowUnmarkedStructs
                };

                members.Add(memberDefinition);
            }

            return members.ToArray();
        }

        /// <summary>
        /// Determines whether a member should be included based on the specified flags.
        /// </summary>
        private static bool ShouldIncludeMember(MemberInfo memberInfo, SerializableMemberFlags flags,
            bool requireSerializeFieldOnNonPublic, bool excludeNonSerializedMembers, bool allowAnonymousTypes, bool allowUnmarkedStructs)
        {
            if (!memberInfo.TryGetMemberType(out var memberType))
            {
                return false;
            }

            if (memberType.IsAnonymousType())
            {
                if (!allowAnonymousTypes)
                {
                    return false;
                }
            }

            {
                var hasSerializableAttribute = memberType.IsDefined(typeof(SerializableAttribute), inherit: false);
                var easySerializableAttribute = SerializedTypeUtility.GetDefinedEasySerializableAttribute(memberType);

                if (memberType.IsStructType())
                {
                    // Check if struct types require serialization attributes
                    if (!allowUnmarkedStructs)
                    {

                        if (!hasSerializableAttribute && easySerializableAttribute == null)
                        {
                            return false;
                        }
                    }
                }
            }

            // Check for EasySerializeFieldAttribute with Ignore flag
            var serializeFieldAttribute = memberInfo.GetCustomAttribute<EasySerializeFieldAttribute>(inherit: true);
            if (serializeFieldAttribute is { Ignore: true })
            {
                return false;
            }

            var hasAnySerializeFieldAttribute = serializeFieldAttribute != null ||
                                             memberInfo.IsDefined(typeof(SerializeField), inherit: true);

            // Check member type (Field vs Property)
            var isField = memberInfo is FieldInfo;
            var isProperty = memberInfo is PropertyInfo;

            // Check for NonSerializedAttribute on fields
            if (excludeNonSerializedMembers && isField)
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
            var isPublic = IsPublicMember(memberInfo);
            var includePublic = flags.HasFlag(SerializableMemberFlags.Public);
            var includeNonPublic = flags.HasFlag(SerializableMemberFlags.NonPublic);

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
                if (!hasAnySerializeFieldAttribute)
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
            try
            {
                return memberInfo.MemberType switch
                {
                    MemberTypes.Field => ReflectionCompiler.CreateInstanceFieldGetter((FieldInfo)memberInfo),
                    MemberTypes.Property => ReflectionCompiler.CreateInstancePropertyGetter((PropertyInfo)memberInfo),
                    _ => throw new NotSupportedException($"Unsupported member type: {memberInfo.MemberType}")
                };
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private InstanceSetter CreateValueSetter(MemberInfo memberInfo)
        {
            try
            {
                return memberInfo.MemberType switch
                {
                    MemberTypes.Field => ReflectionCompiler.CreateInstanceFieldSetter((FieldInfo)memberInfo),
                    MemberTypes.Property => ReflectionCompiler.CreateInstancePropertySetter((PropertyInfo)memberInfo),
                    _ => throw new NotSupportedException($"Unsupported member type: {memberInfo.MemberType}")
                };
            }
            catch (NotSupportedException)
            {
                throw;
            }
            catch (Exception)
            {
                if (memberInfo is PropertyInfo propertyInfo)
                {
                    if (propertyInfo.TryGetBackingField(out var backingField))
                    {
                        try
                        {
                            return ReflectionCompiler.CreateInstanceFieldSetter(backingField);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                }
                return null;
            }
        }
    }
}
