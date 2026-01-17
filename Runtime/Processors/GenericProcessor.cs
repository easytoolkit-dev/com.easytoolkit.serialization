using System;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization.Processors
{
    [ProcessorConfiguration(ProcessorPriorityLevel.Generic)]
    public class GenericProcessor<T> : SerializationProcessor<T>
    {
        private static readonly Func<T> Constructor = CreateConstructor();

        public override bool CanProcess(Type valueType)
        {
            return !valueType.IsBasicValueType() && !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        protected override void Process(string name, ref T value, IDataFormatter formatter)
        {
            if (value == null)
            {
                if (Constructor == null)
                {
                    throw new ArgumentException($"Type '{typeof(T)}' does not have a default constructor.");
                }

                value = Constructor();
            }

            var node = (ISerializationStructuralNode)Environment.GetFactory<ISerializationNodeFactory>().BuildNode<T>();
            var members = node.Members;

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

                    var boxedValue = (object)value;
                    memberValue = getter(ref boxedValue);
                }

                memberNode.Process(memberNode.Definition.Name, ref memberValue, formatter);

                if (formatter.Operation == FormatterOperation.Read)
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
