using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class SerializationArrayNode<T> : SerializationNodeBase<T>, ISerializationArrayNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public SerializationArrayNode(
            int rank,
            SerializationMemberDefinition memberDefinition,
            ISerializationNode parent = null,
            int index = -1,
            ISerializationProcessor<T> serializer = null)
            : base(memberDefinition, serializer, parent, index)
        {
            Rank = rank;
        }

        public override NodeType NodeType => NodeType.Array;

        public int Rank { get; }
    }
}
