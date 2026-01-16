using System;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization
{
    [SerializerConfiguration(SerializerPriorityLevel.Generic)]
    public class GenericSerializer<T> : EasySerializer<T>
    {
        private static readonly bool IsNode = IsNodeImpl(typeof(T));
        private static readonly Func<T> Constructor = CreateConstructor();

        public override bool CanSerialize(Type valueType)
        {
            return !valueType.IsBasicValueType() && !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            if (value == null)
            {
                if (Constructor == null)
                {
                    throw new ArgumentException($"Type '{typeof(T)}' does not have a default constructor.");
                }

                value = Constructor();
            }

            if (Node.Parent == null)
            {
                if (IsNode)
                {
                    formatter.BeginMember(name);
                    formatter.BeginObject();
                }
            }

            // Use Node.Members (triggers lazy initialization)
            var members = ((IStructSerializationNode)Node).Members;

            var direction = formatter.Direction;
            foreach (var memberNode in members)
            {
                var memberSerializer = memberNode.Serializer;
                object memberValue = null;

                if (direction == FormatterDirection.Output)
                {
                    var getter = memberNode.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{memberNode.Definition.Name}' is not readable!");
                    }

                    var boxedValue = (object)value;
                    memberValue = getter(ref boxedValue);
                }

                memberSerializer.Process(memberNode.Definition.Name, ref memberValue, formatter);

                if (direction == FormatterDirection.Input)
                {
                    var setter = memberNode.ValueSetter;
                    if (setter == null)
                    {
                        throw new ArgumentException($"Member '{memberNode.Definition.Name}' is not writable!");
                    }

                    var boxedValue = (object)value;
                    setter(ref boxedValue, memberValue);
                    value = (T)boxedValue;
                }
            }

            if (Node.Parent == null)
            {
                if (IsNode)
                {
                    formatter.EndObject();
                    formatter.EndMember();
                }
            }
        }

        private static bool IsNodeImpl(Type type)
        {
            return (type.IsClass && type != typeof(string)) ||
                   (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
        }

        private static Func<T> CreateConstructor()
        {
            try
            {
                return () => (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
