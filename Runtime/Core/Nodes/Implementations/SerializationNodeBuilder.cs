using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class SerializationNodeBuilder : ISerializationNodeBuilder
    {
        private ISerializationStructureResolverFactory _resolverFactory;

        /// <summary>
        /// Builds a root node for the specified type.
        /// </summary>
        public IStructSerializationNode BuildNode(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            _resolverFactory ??= SerializationGlobalContext.Instance
                                     .GetService<ISerializationStructureResolverFactory>()
                                 ?? throw new InvalidOperationException(
                                     "ISerializationStructureResolverFactory is not registered.");

            // Root node validation
            ValidateRootNode(type);

            // Create root node member definition
            var memberDefinition = new SerializationMemberDefinition
            {
                Name = string.Empty,
                MemberType = type,
                MemberInfo = null,
                IsRequired = false,
                DefaultValue = null
            };

            // Create and bind serializer
            var serializer = EasySerializerUtility.GetSerializer(type);
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find serializer for type '{type.FullName}'.");
            }

            // Create root node (must have structure resolver)
            var rootNode = new StructSerializationNode(
                valueType: type,
                nodeBuilder: this,
                memberDefinition: memberDefinition,
                parent: null,
                index: -1,
                serializer: serializer);

            // Bind serializer to root node
            serializer.Node = rootNode;

            return rootNode;
        }

        /// <summary>
        /// Builds a child node for the specified type with additional context.
        /// </summary>
        public ISerializationNode BuildNode(
            Type type,
            MemberInfo memberInfo,
            int index,
            ISerializationNode parent)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            _resolverFactory ??= SerializationGlobalContext.Instance
                                     .GetService<ISerializationStructureResolverFactory>()
                                 ?? throw new InvalidOperationException(
                                     "ISerializationStructureResolverFactory is not registered.");

            // Create child node member definition
            var memberDefinition = new SerializationMemberDefinition
            {
                Name = memberInfo.Name,
                MemberType = type,
                MemberInfo = memberInfo,
                IsRequired = false,
                DefaultValue = null
            };

            // Create serializer
            var serializer = EasySerializerUtility.GetSerializer(type);
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find serializer for type '{type.FullName}'.");
            }

            // Check if has structure resolver
            var resolver = _resolverFactory.CreateResolver(type);
            if (resolver != null)
            {
                return new StructSerializationNode(
                    valueType: type,
                    nodeBuilder: this,
                    memberDefinition: memberDefinition,
                    parent: parent,
                    index: index,
                    serializer: serializer);
            }

            // Array type
            if (type.IsArray)
            {
                return new ArraySerializationNode(
                    valueType: type,
                    rank: type.GetArrayRank(),
                    memberDefinition: memberDefinition,
                    parent: parent,
                    index: index,
                    serializer: serializer);
            }

            // Atomic type
            return new AtomicSerializationNode(
                valueType: type,
                memberDefinition: memberDefinition,
                parent: parent,
                index: index,
                serializer: serializer);
        }

        private void ValidateRootNode(Type type)
        {
            if (type.GetCustomAttribute<SerializableAttribute>() == null)
            {
                throw new InvalidOperationException(
                    $"Root type must have [Serializable] attribute. Type: {type.FullName}");
            }

            if (type.IsArray)
            {
                throw new InvalidOperationException(
                    $"Root type cannot be an array. Type: {type.FullName}");
            }

            if (type.IsGenericType && type.IsImplementsOpenGenericType(typeof(IList<>)))
            {
                throw new InvalidOperationException(
                    $"Root type cannot be List. Type: {type.FullName}");
            }
        }
    }
}
