using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core.Reflection;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class SerializationNodeFactory : ISerializationNodeFactory
    {
        private ISerializationStructureResolverFactory _resolverFactory;
        private ISerializationProcessorFactory _serializationProcessorFactory;

        /// <summary>
        /// Builds a root node for the specified type.
        /// </summary>
        public ISerializationNode BuildNode<T>()
        {
            return BuildRootNodeImpl<T>(null, -1, null);
        }

        /// <summary>
        /// Builds a child node for the specified type with additional context.
        /// </summary>
        public ISerializationNode BuildNode<T>(
            MemberInfo memberInfo,
            int index,
            ISerializationNode parent)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            return BuildRootNodeImpl<T>(memberInfo, index, parent);
        }

        private ISerializationNode BuildRootNodeImpl<T>(
            [CanBeNull] MemberInfo memberInfo,
            int index,
            [CanBeNull] ISerializationNode parent)
        {
            _resolverFactory ??= SerializationEnvironment.Instance
                                     .GetFactory<ISerializationStructureResolverFactory>()
                                 ?? throw new InvalidOperationException(
                                     "ISerializationStructureResolverFactory is not registered.");

            _serializationProcessorFactory ??= SerializationEnvironment.Instance
                                       .GetFactory<ISerializationProcessorFactory>()
                                   ?? throw new InvalidOperationException(
                                       "ISerializerFactory is not registered.");

            // Create child node member definition
            var memberDefinition = new SerializationMemberDefinition
            {
                Name = memberInfo?.Name,
                MemberType = typeof(T),
                MemberInfo = memberInfo,
                IsRequired = false,
                DefaultValue = null
            };

            // Create serializer
            var serializer = _serializationProcessorFactory.GetSerializer<T>();
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find serializer for type '{typeof(T).FullName}'.");
            }

            // Check if has structure resolver
            var resolver = _resolverFactory.CreateResolver(typeof(T));
            if (resolver != null)
            {
                return new SerializationStructuralNode<T>(
                    nodeFactory: this,
                    memberDefinition: memberDefinition,
                    parent: parent,
                    index: index,
                    serializer: serializer);
            }

            // Array type
            if (typeof(T).IsArray)
            {
                return new SerializationArrayNode<T>(
                    rank: typeof(T).GetArrayRank(),
                    memberDefinition: memberDefinition,
                    parent: parent,
                    index: index,
                    serializer: serializer);
            }

            // Atomic type
            return new SerializationAtomicNode<T>(
                memberDefinition: memberDefinition,
                parent: parent,
                index: index,
                serializer: serializer);
        }
    }
}
