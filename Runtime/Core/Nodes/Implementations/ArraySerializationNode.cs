using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    internal sealed class ArraySerializationNode : SerializationNodeBase, IArraySerializationNode
    {
        private InstanceGetter _valueGetter;
        private InstanceSetter _valueSetter;

        public ArraySerializationNode(
            Type valueType,
            int rank,
            SerializationMemberDefinition memberDefinition,
            ISerializationNode parent = null,
            int index = -1,
            IEasySerializer serializer = null)
            : base(valueType, memberDefinition, serializer, parent, index)
        {
            Rank = rank;
        }

        public override NodeType NodeType => NodeType.Array;

        public int Rank { get; }
    }
}
