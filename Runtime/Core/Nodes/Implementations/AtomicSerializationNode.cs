using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class AtomicSerializationNode : SerializationNodeBase, IAtomicSerializationNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public AtomicSerializationNode(
            Type valueType,
            SerializationMemberDefinition memberDefinition,
            ISerializationNode parent = null,
            int index = -1,
            IEasySerializer serializer = null)
            : base(valueType, memberDefinition, serializer, parent, index)
        {
        }

        public override NodeType NodeType => NodeType.Atomic;
    }
}
