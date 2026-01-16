using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class AtomicSerializationNode<T> : SerializationNodeBase<T>, IAtomicSerializationNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public AtomicSerializationNode(
            SerializationMemberDefinition memberDefinition,
            ISerializationNode parent = null,
            int index = -1,
            ISerializationProcessor<T> serializer = null)
            : base(memberDefinition, serializer, parent, index)
        {
        }

        public override NodeType NodeType => NodeType.Atomic;
    }
}
