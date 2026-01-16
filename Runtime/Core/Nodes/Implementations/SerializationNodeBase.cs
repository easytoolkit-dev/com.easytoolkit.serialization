using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Base class for serialization nodes, providing common properties and methods.
    /// </summary>
    public abstract class SerializationNodeBase : ISerializationNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationNodeBase"/> class.
        /// </summary>
        protected SerializationNodeBase(
            Type valueType,
            SerializationMemberDefinition memberDefinition,
            IEasySerializer serializer,
            ISerializationNode parent,
            int index)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            Definition = memberDefinition ?? throw new ArgumentNullException(nameof(memberDefinition));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Parent = parent;
            Index = index;

            // Create delegates if not provided
            if (memberDefinition.MemberInfo != null)
            {
                ValueGetter = CreateValueGetter(memberDefinition.MemberInfo);
            }
            if (memberDefinition.MemberInfo != null)
            {
                ValueSetter = CreateValueSetter(memberDefinition.MemberInfo);
            }
        }

        public SerializationMemberDefinition Definition { get; }

        public abstract NodeType NodeType { get; }

        public Type ValueType { get; }

        public IEasySerializer Serializer { get; }

        public InstanceGetter ValueGetter { get; }

        public InstanceSetter ValueSetter { get; }

        public ISerializationNode Parent { get; }

        public int Index { get; }

        public string Path => Parent != null ? $"{Parent.Path}.{Definition.Name}" : Definition.Name;

        public bool IsChildOf(ISerializationNode node)
        {
            var current = Parent;
            while (current != null)
            {
                if (current == node) return true;
                current = current.Parent;
            }
            return false;
        }

        public abstract void Process(string name, ref object value, IDataFormatter formatter);

        private InstanceGetter CreateValueGetter(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ReflectionUtility.CreateInstanceFieldGetter((FieldInfo)memberInfo),
                MemberTypes.Property => ReflectionUtility.CreateInstancePropertyGetter((PropertyInfo)memberInfo),
                _ => throw new ArgumentException($"Unsupported member type: {memberInfo.MemberType}")
            };
        }

        private InstanceSetter CreateValueSetter(MemberInfo memberInfo)
        {
            return memberInfo.MemberType switch
            {
                MemberTypes.Field => ReflectionUtility.CreateInstanceFieldSetter((FieldInfo)memberInfo),
                MemberTypes.Property => ReflectionUtility.CreateInstancePropertySetter((PropertyInfo)memberInfo),
                _ => throw new ArgumentException($"Unsupported member type: {memberInfo.MemberType}")
            };
        }
    }

    public abstract class SerializationNodeBase<T> : SerializationNodeBase
    {
        public new IEasySerializer<T> Serializer { get; }

        protected SerializationNodeBase(
            SerializationMemberDefinition memberDefinition,
            IEasySerializer<T> serializer,
            ISerializationNode parent,
            int index) : base(typeof(T), memberDefinition, serializer, parent, index)
        {
            Serializer = serializer;
        }

        public override void Process(string name, ref object value, IDataFormatter formatter)
        {
            var castedValue = value != null ? (T)value : default;
            Serializer.Process(name, ref castedValue, formatter);
            value = castedValue;
        }
    }
}
