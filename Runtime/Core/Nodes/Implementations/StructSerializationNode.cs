using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class StructSerializationNode : SerializationNodeBase, IStructSerializationNode
    {
        private readonly ISerializationNodeBuilder _nodeBuilder;
        private readonly ISerializationStructureResolverFactory _resolverFactory;
        private readonly EasySerializeSettings _settings;
        private IReadOnlyList<ISerializationNode> _members;
        private bool _isResolved;

        public StructSerializationNode(
            Type valueType,
            EasySerializeSettings settings,
            ISerializationNodeBuilder nodeBuilder,
            SerializationMemberDefinition memberDefinition,
            IEasySerializer serializer,
            ISerializationNode parent = null,
            int index = -1)
            : base(valueType, memberDefinition, serializer, parent, index)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _nodeBuilder = nodeBuilder ?? throw new ArgumentNullException(nameof(nodeBuilder));
            _resolverFactory = settings.SharedContext
                .GetService<ISerializationStructureResolverFactory>()
                ?? throw new InvalidOperationException(
                    "ISerializationStructureResolverFactory is not registered in SerializationSharedContext.");

            // Lazy initialization: Members will be resolved on first access
            _isResolved = false;
            _members = Array.Empty<ISerializationNode>();
        }

        public override NodeType NodeType => NodeType.Struct;

        /// <summary>
        /// Gets the members of this structure node. Lazy-initialized on first access.
        /// </summary>
        public IReadOnlyList<ISerializationNode> Members
        {
            get
            {
                if (!_isResolved)
                {
                    _members = ResolveMembers();
                    _isResolved = true;
                }
                return _members;
            }
        }

        private IReadOnlyList<ISerializationNode> ResolveMembers()
        {
            var resolver = _resolverFactory.CreateResolver(ValueType);
            if (resolver == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find resolver for type '{ValueType.FullName}'.");
            }

            var memberDefinitions = resolver.Resolve(ValueType);
            if (memberDefinitions.Length == 0)
            {
                return Array.Empty<ISerializationNode>();
            }

            var children = new List<ISerializationNode>(memberDefinitions.Length);

            for (int i = 0; i < memberDefinitions.Length; i++)
            {
                var definition = memberDefinitions[i];
                if (definition == null) continue;

                var childNode = _nodeBuilder.BuildNode(
                    definition.MemberType,
                    _settings,
                    definition.MemberInfo,
                    i,
                    this);
                children.Add(childNode);
            }

            return children;
        }

        public bool TryGetMember(string name, out ISerializationNode member)
        {
            var members = Members; // Ensure members are resolved

            foreach (var m in members)
            {
                if (m.Definition.Name == name)
                {
                    member = m;
                    return true;
                }
            }

            member = null;
            return false;
        }
    }
}
