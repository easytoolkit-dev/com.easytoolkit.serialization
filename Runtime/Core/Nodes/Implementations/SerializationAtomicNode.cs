using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class SerializationAtomicNode : SerializationNodeBase, ISerializationAtomicNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public SerializationAtomicNode(
            Type valueType,
            SerializationMemberDefinition memberDefinition,
            ISerializationNode parent = null,
            int index = -1,
            ISerializationProcessor serializer = null)
            : base(valueType, memberDefinition, serializer, parent, index)
        {
        }

        public override NodeType NodeType => NodeType.Atomic;
    }
}
