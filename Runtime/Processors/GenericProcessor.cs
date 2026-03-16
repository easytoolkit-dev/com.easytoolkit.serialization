using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Resolvers;
using EasyToolkit.Serialization.Utilities;

namespace EasyToolkit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class GenericProcessor<T> : SerializationProcessor<T>
    {
        private SerializationMemberDefinition[] _memberDefinitions;

        /// <inheritdoc/>
        protected override bool AutoConstruct => true;

        public override bool CanProcess(Type valueType, SerializationContext context)
        {
            if (SerializationStructureResolverFactory.GetResolver(valueType) == null)
            {
                return false;
            }

            if (valueType.IsAnonymousType() && context.AllowAnonymousTypes)
            {
                return true;
            }

            if (!valueType.IsDefined<SerializableAttribute>()
                && SerializedTypeUtility.GetDefinedEasySerializableAttribute(valueType) == null)
            {
                if (context.AllowUnmarkedStructs && valueType.IsStructType())
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        protected override void Initialize()
        {
            _memberDefinitions = SerializationStructureResolverFactory.GetResolver(typeof(T)).Resolve(typeof(T), Context);
        }

        protected override void Process(ref T value, IDataFormatter formatter)
        {
            formatter.BeginObject();

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

                    object boxedValue = value;
                    setter(ref boxedValue, memberValue);
                    value = (T)boxedValue;
                }
            }

            formatter.EndObject();
        }
    }
}
