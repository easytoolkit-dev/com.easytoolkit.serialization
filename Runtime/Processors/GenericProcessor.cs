using System;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Serialization.Formatters;
using EasyToolKit.Serialization.Resolvers;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class GenericProcessor<T> : SerializationProcessor<T>
    {
        private SerializationMemberDefinition[] _memberDefinitions;

        public override bool CanProcess(Type valueType)
        {
            return SerializationStructureResolverFactory.GetResolver(valueType) != null &&
                   (valueType.IsDefined<SerializableAttribute>() ||
                    SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType) != null);
        }

        protected override void Initialize()
        {
            _memberDefinitions = SerializationStructureResolverFactory.GetResolver(typeof(T)).Resolve(typeof(T));
        }

        public override void Process(string name, ref T value, IDataFormatter formatter)
        {
            formatter.BeginMember(name);
            using var objectScope = formatter.EnterObject();

            foreach (var memberDefinition in _memberDefinitions)
            {
                object memberValue = null;

                if (formatter.Operation == FormatterOperation.Write)
                {
                    var getter = memberDefinition.ValueGetter;
                    if (getter == null)
                    {
                        throw new ArgumentException($"Member '{memberDefinition.Name}' is not readable!");
                    }

                    memberValue = getter(value);
                }

                memberDefinition.Processor.ProcessUntyped(memberDefinition.Name, ref memberValue, formatter);

                if (formatter.Operation == FormatterOperation.Read)
                {
                    var setter = memberDefinition.ValueSetter;
                    if (setter == null)
                    {
                        throw new ArgumentException($"Member '{memberDefinition.Name}' is not writable!");
                    }

                    setter(value, memberValue);
                }
            }
        }
    }
}
