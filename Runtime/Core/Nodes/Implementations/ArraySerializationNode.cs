using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class ArraySerializationNode<T> : SerializationNodeBase<T>, IArraySerializationNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public ArraySerializationNode(
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
