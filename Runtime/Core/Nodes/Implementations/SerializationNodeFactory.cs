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
        public ISerializationNode BuildNode(Type valueType)
        {
            return BuildRootNodeWithoutParametersCheck(valueType, null, -1, null);
        }

        /// <summary>
        /// Builds a child node for the specified type with additional context.
        /// </summary>
        public ISerializationNode BuildNode(
            Type valueType,
            MemberInfo memberInfo,
            int index,
            ISerializationNode parent)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            return BuildRootNodeWithoutParametersCheck(valueType, memberInfo, index, parent);
        }

        private ISerializationNode BuildRootNodeWithoutParametersCheck(
            Type valueType,
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
                MemberType = valueType,
                MemberInfo = memberInfo,
                IsRequired = false,
                DefaultValue = null
            };

            // Create serializer
            var serializer = _serializationProcessorFactory.GetProcessor(valueType);
            if (serializer == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find serializer for type '{valueType.FullName}'.");
            }

            // Array type
            if (valueType.IsImplementsGenericDefinition(typeof(IList<>)))
            {
                return new SerializationArrayNode(
                    valueType,
                    memberDefinition,
                    parent,
                    index,
                    serializer);
            }

            // Check if has structure resolver
            var resolver = _resolverFactory.CreateResolver(valueType);
            if (resolver != null)
            {
                return new SerializationStructuralNode(
                    valueType,
                    memberDefinition,
                    serializer,
                    parent,
                    index);
            }

            // Atomic type
            return new SerializationAtomicNode(
                valueType,
                memberDefinition,
                parent,
                index,
                serializer);
        }
    }
}
