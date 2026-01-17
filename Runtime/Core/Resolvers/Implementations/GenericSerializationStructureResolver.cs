using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Resolves serialization structure for types marked with [Serializable] attribute.
    /// </summary>
    [SerializationResolverPriority(10.0)]
    internal sealed class GenericSerializationStructureResolver : ISerializationStructureResolver
    {
        public bool CanResolve(Type type)
        {
            // Only resolve types with [Serializable] attribute
            return type.GetCustomAttribute<SerializableAttribute>() != null;
        }

        public SerializationMemberDefinition[] Resolve(Type type)
        {
            if (!CanResolve(type))
            {
                return Array.Empty<SerializationMemberDefinition>();
            }

            var members = new List<SerializationMemberDefinition>();

            var memberInfos = type.GetMembers(MemberAccessFlags.AllInstance)
                .Where(memberInfo => memberInfo is FieldInfo || memberInfo is PropertyInfo)
                .ToList();

            for (int i = 0; i < memberInfos.Count; i++)
            {
                var memberInfo = memberInfos[i];
                Type memberType = GetMemberType(memberInfo);

                var memberDefinition = new SerializationMemberDefinition
                {
                    Name = memberInfo.Name,
                    MemberType = memberType,
                    MemberInfo = memberInfo,
                    IsRequired = false,
                    DefaultValue = null
                };

                members.Add(memberDefinition);
            }

            return members.ToArray();
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
    }
}
