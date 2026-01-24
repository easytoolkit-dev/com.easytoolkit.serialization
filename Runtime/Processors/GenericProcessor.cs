using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class GenericProcessor<T> : SerializationProcessor<T>
    {
        private ISerializationStructuralNode _node;

        public override bool CanProcess(Type valueType)
        {
            if (!valueType.IsBasicValueType() &&
                !valueType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                valueType.IsDefined<SerializableAttribute>())
            {
                return true;
            }

            return SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType) != null;
        }

        protected override void Initialize()
        {
            _node = (ISerializationStructuralNode)Environment.GetFactory<ISerializationNodeFactory>()
                .BuildNode(typeof(T));
        }

        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            using var objectScope = formatter.EnterObject();

            var members = _node.Members;

            foreach (var memberNode in members)
            {
                object memberValue = null;

                if (formatter.Operation == FormatterOperation.Write)
                {
                    var getter = memberNode.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{memberNode.Definition.Name}' is not readable!");
                    }

                    memberValue = getter(value);
                }

                memberNode.Processor.ProcessUntyped(memberNode.Definition.Name, ref memberValue, formatter);

                if (formatter.Operation == FormatterOperation.Read)
                {
                    var setter = memberNode.ValueSetter;
                    if (setter == null)
                    {
                        throw new ArgumentException($"Member '{memberNode.Definition.Name}' is not writable!");
                    }

                    setter(value, memberValue);
                }
            }
        }
    }
}
